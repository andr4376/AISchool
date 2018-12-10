using AIFramework;
using AIFramework.Actions;
using AIFramework.Entities;
using ReflectionChecker;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace GameOfLife
{
    class World : IDrawable
    {
        Graphics graphics;
        BufferedGraphicsContext context;
        BufferedGraphics buffer;

        List<AgentFactory> factories;
        List<IEntity> entities;
        List<IEntity> toBeAdded;
        List<IEntity> toBeRemoved;
        List<VisualEffect> effects;

        Stack<Agent> deadAgents;

        float runningSeconds;
        float plantCounter;

        bool gameRunning;

        Dictionary<string, Image> typeTextures;

        Thread gameLoopThread;

        float surfaceWidth;
        float surfaceHeight;

        Mutex mutex = new Mutex();

        Dictionary<string, PropertyProvider> pproviders = new Dictionary<string, PropertyProvider>();
        List<AppDomain> domains = new List<AppDomain>();

        public GameInfo GameStatus
        {
            get {
                try{
                    mutex.WaitOne();
                    GameInfo g = new GameInfo();
                    g.GameFinished = !gameRunning;
                    g.Won = LeadingAgents(out g.LeadingAgents, out g.LeadingAgentsAmount, out g.LeadingCreators);


                    g.AmountOfAgents = entities.Count(a => a.GetType() == typeof(Agent));
                    g.SecondsLeft = (int)(AIModifiers.maxRunningTimeInSeconds - runningSeconds);
                    return g;
                }finally
                {
                    mutex.ReleaseMutex();
                }
            }
        }

        private bool LeadingAgents(out string agentType, out int amount, out string creators)
        {
            bool result = false;
            agentType = "";
            amount = 0;
            creators = "";
            foreach (AgentFactory f in factories)
            {

                int count = entities.Count(a => a.EntityType == f.ProvidedAgentTypeName);
                if (amount < count)
                {
                    amount = count;
                    agentType = f.ProvidedAgentTypeName;
                    creators = f.Creators;
                    result = true;
                }


            }
            return result;
        }
        

        public World(Graphics graphics, Rectangle displayRectangle)
        {
            this.graphics = graphics;
            surfaceWidth = displayRectangle.Width - 20;
            surfaceHeight = displayRectangle.Height - 20;

            context = BufferedGraphicsManager.Current;
            buffer = context.Allocate(graphics, displayRectangle);


        }

        public void Start()
        {
            factories = new List<AgentFactory>();
            entities = new List<IEntity>();
            toBeAdded = new List<IEntity>();
            toBeRemoved = new List<IEntity>();
            effects = new List<VisualEffect>();

            deadAgents = new Stack<Agent>();

            runningSeconds = 0f;
            plantCounter = 0f;

            gameRunning = true;

            typeTextures = new Dictionary<string, Image>();

            typeTextures.Add("Plant", Image.FromFile("Plant.png"));

            LoadPlugins();
            CreateAgents();
            gameLoopThread = new Thread(GameLoop);
            gameLoopThread.IsBackground = true;
            gameLoopThread.Start();
        }

        public void Stop()
        {
            gameRunning = false;
        }


        private void GameLoop()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Stop();
            TimeSpan parsedTime = TimeSpan.FromSeconds(0.001); 

            while(gameRunning)
            {
                stopWatch.Start();
                try
                {
                    mutex.WaitOne();
                    Update(parsedTime);
                    Draw(parsedTime, buffer.Graphics);
                }
                finally
                {
                    mutex.ReleaseMutex();
                }
                buffer.Render();
                stopWatch.Stop();
 
                parsedTime = stopWatch.Elapsed;
                stopWatch.Reset();
            }

        }

        private bool IsAssemblyOK(string plugin, Type t)
        {
            AppDomainSetup setup = new AppDomainSetup();
            setup.ApplicationBase = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "");
            AppDomain appDomain = AppDomain.CreateDomain("AppHostDomain", null, setup);
            AgentFactory factory = (AgentFactory)(appDomain.CreateInstanceFromAndUnwrap(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, plugin), t.FullName));



            NameSpaceChecker tmp = (NameSpaceChecker)(appDomain.CreateInstanceFromAndUnwrap(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ReflectionChecker.dll"), "ReflectionChecker.NameSpaceChecker"));
            string error = "";
            if (tmp.IsAddonInvalid(out error))
            {
                return false;
            }
            return true;
        }

        private void LoadPlugins()
        {
            string[] plugins = Directory.GetFiles("plugins");
            foreach (string plugin in plugins)
            {

                if (plugin.EndsWith(".dll"))
                {

                 
                    Assembly assembly = Assembly.LoadFrom(plugin);

                    Type[] providedTypes = assembly.GetTypes();
                    foreach (Type t in providedTypes)
                    {
                        if (t.Name != "AgentFactory" && t.BaseType != null && t.BaseType.Name == "AgentFactory" && IsAssemblyOK(plugin, t))
                        {
                            AgentFactory factory = (AgentFactory)Activator.CreateInstance(t);
                            try
                            {
                                string tmp = factory.Creators;
                                typeTextures.Add(factory.ProvidedAgentTypeName, Image.FromFile(plugin.Replace(".dll", "") + ".png"));
                                factories.Add(factory);
                            }
                            catch { 
                            
                            }
                        
                        }
                    }



                }
            }
        }


        private void CreateAgents()
        {
            Random rnd = new Random();



            for (int i = 0; i < AIModifiers.initialAgentCount; i++)
            {

                foreach (AgentFactory x in factories)
                {

                    PropertyProvider p = new PropertyProvider();
                    pproviders.Add(p.Id.ToString(), p);
                    Agent a = x.CreateAgent(p);

                    p.Hitpoints = a.Health;
                    p.Hunger = 0;
                    p.ProcreationCountDown = AIModifiers.initialProcreationCount;
                    if (a.Invarient())
                    {
                        p.Hitpoints = a.Health;
                        p.Hunger = 0;
                        p.ProcreationCountDown = AIModifiers.initialProcreationCount;
                        p.Position = new AIVector(rnd.Next((int)surfaceWidth), rnd.Next((int)surfaceHeight));
                        entities.Add(a);
                    }
                }
                //Added to make more randomness between agents
                Thread.Sleep(rnd.Next(10));

            }

            for (int i = 0; i < AIModifiers.initialPlants * factories.Count; i++)
            {
                Plant p = new Plant();
                p.Position = new AIVector(rnd.Next((int)surfaceWidth), rnd.Next((int)surfaceHeight));
                entities.Add(p);
            }

        }




        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        void Update(TimeSpan gameTime)
        {
            effects.RemoveAll(e => e.ShouldBeRemoved);

            float secondsSinceLastFrame = (float)gameTime.TotalSeconds;
            plantCounter += secondsSinceLastFrame;
            runningSeconds += secondsSinceLastFrame; 


            List<IEntity> agents = entities.FindAll(e => e is Agent);


            if (!gameRunning)
            {
                return;
            }
            else if (gameRunning && (runningSeconds > AIModifiers.maxRunningTimeInSeconds || agents.Count < 1))
            {

                gameRunning = false;
                agents.ForEach(a=>deadAgents.Push((Agent)a));

                Dictionary<AgentFactory, int> endingAgents = new Dictionary<AgentFactory, int>();

                foreach (Agent a in agents)
                {
                    foreach (AgentFactory f in factories)
                    {
                        if (f.ProvidedAgentTypeName == a.EntityType)
                        {
                            if (endingAgents.ContainsKey(f))
                            {
                                endingAgents[f] += 1;
                            }
                            else
                            {
                                endingAgents.Add(f, 1);
                            }
                        }
                    }
                }
                List<Agent> dead = deadAgents.ToList();
                Agent winningAgent = deadAgents.Peek();
                AgentFactory winningAgentFactory = null;
                int currentCount = 0;


                foreach (AgentFactory f in factories)
                {
                    try
                    {
                        f.RegisterWinners(dead);
                    }
                    catch
                    {
                    }
                }
               
                foreach (KeyValuePair<AgentFactory,int> f in endingAgents)
                {
                    if (f.Value > currentCount)
                    {
                        winningAgentFactory = f.Key;
                        currentCount = f.Value;
                    }
                }

                return;
            }
           



           //Reset tmp lists
           toBeAdded.Clear();
           toBeRemoved.Clear();

           Random rnd = new Random();
           
            //Create plant
           if (plantCounter > 1)
           {
               plantCounter = 0;
               for (int i = 0; i < AIModifiers.newPlantsPerSecond; i++)
               {
                   Plant p = new Plant();
                   p.Position = new AIVector(rnd.Next((int)surfaceWidth), rnd.Next((int)surfaceHeight));
                   entities.Add(p);
               }
           }


           int agentCount = 0;
           foreach (IEntity e in agents)
           {

               agentCount++;
               Agent a = (Agent)e;
               PropertyProvider p = pproviders[a.Id.ToString()];
               //Increment hunger
               p.Hunger += AIModifiers.hungerIncrementPerSecond * secondsSinceLastFrame;

               //Do action
               a.ActionResultCallback(DoAction(a, gameTime));

               //Reduce hitpoints due to hunger
               if (p.Hunger > AIModifiers.maxHungerBeforeHitpointsDamage)
               {
                   p.Hitpoints -= AIModifiers.hungerHitpointsDamagePerSecond * secondsSinceLastFrame;
               }
               
               //Remove agent if under 0 hitpoints = agent is dead
               if (p.Hitpoints <= 0)
               {
                   toBeRemoved.Add(a);
               } else if (p.Hunger <= p.Endurance)//Hitpoint regenration due to endurance higher then hunger
               {
                   p.Hitpoints += AIModifiers.hitpointRegenPerSecond * secondsSinceLastFrame;
                   if (p.Hitpoints > p.Health)
                   {
                       p.Hitpoints = p.Health;
                   }
               }

               //Procreation reduction
               if (p.ProcreationCountDown > 0)
               {
                   p.ProcreationCountDown -= AIModifiers.procreationReductionPerSecond * secondsSinceLastFrame;
                   if (p.ProcreationCountDown < 0)
                   {
                       p.ProcreationCountDown = 0;
                   }
               }

              
           }


                entities.AddRange(toBeAdded);
                toBeRemoved.ForEach(a => { entities.Remove(a); deadAgents.Push((Agent)a); });


        }
        private bool DoAction(Agent a, TimeSpan gameTime)
        {

            PropertyProvider p = pproviders[a.Id.ToString()];
            p.Defending = false;
            bool status = false;


            List<IEntity> sightEntities = entities.FindAll(e => AIVector.Distance(p.Position, e.Position) <= a.Eyesight);
            IAction action = a.GetNextAction(sightEntities);
            switch (action.GetType().Name)
            {
                case "Attack":
                    {
                        Attack att = (Attack)action;
                        if (att.Defender != null)
                        {
                            PropertyProvider pDefender = pproviders[att.Defender.Id.ToString()];
                            if (AIVector.Distance(p.Position, pDefender.Position) <= AIModifiers.maxMeleeAttackRange) //If 
                            {

                                Random rnd = new Random();
                                int currentSuccessChance = AIModifiers.baseChanceOfAttackSuccess - (pDefender.Defending ? pDefender.Dodge : 0);
                                if (currentSuccessChance < AIModifiers.minChanceOfAttackSuccess)
                                {
                                    currentSuccessChance = AIModifiers.minChanceOfAttackSuccess;
                                }
                                if (rnd.Next(100) < currentSuccessChance) //Attack was an success
                                {


                                    pDefender.Hitpoints -= p.Strength * (float)gameTime.TotalSeconds;
                                    status = true;
                                }
                            }
                        }
                    }
                    break;
                case "Move":
                    {
                        Move m = (Move)action;
                        AIVector v = m.Direction.Normalize();

                        if (float.IsNaN(v.X) || float.IsNaN(v.Y))
                        {
                            v.X = 0;
                            v.Y = 0;
                        }

                        v = v * p.MovementSpeed * (float)gameTime.TotalSeconds;
                        p.Position.X += v.X;
                        p.Position.Y += v.Y;

                        status = true;
                        if (p.Position.X < 0)
                        {
                            status = false;
                            p.Position.X = 0;
                        }
                        if (p.Position.X > (int)surfaceWidth)
                        {
                            status = false;
                            p.Position.X = (int)surfaceWidth;
                        }
                        if (p.Position.Y < 0)
                        {
                            status = false;
                            p.Position.Y = 0;
                        }
                        if (p.Position.Y > (int)surfaceHeight)
                        {
                            status = false;
                            p.Position.Y = (int)surfaceHeight;
                        }
                    }
                    break;
                case "Procreate":
                    {
                        Procreate pc = (Procreate)action;
                        if (pc.Mate != null)
                        {
                            PropertyProvider pMate = pproviders[pc.Mate.Id.ToString()];
                            //Both agents should be ready to procreate and within distance
                            if (a != pc.Mate && a.GetType() == pc.Mate.GetType() && p.ProcreationCountDown <= 0 && pMate.ProcreationCountDown <= 0 && AIVector.Distance(p.Position, pMate.Position) <= AIModifiers.maxProcreateRange)
                            {
                                //A random parent factory is chosen
                                Random rnd = new Random();

                                string randomParentType = rnd.Next(2) == 0 ? a.EntityType : pc.Mate.EntityType;
                                foreach (AgentFactory f in factories)
                                {
                                    if (f.ProvidedAgentTypeName == randomParentType)
                                    {
                                        p.ProcreationCountDown = 100;


                                        pMate.ProcreationCountDown = 100;

                                        PropertyProvider pNew = new PropertyProvider();
                                        pproviders.Add(pNew.Id.ToString(), pNew);
                                        Agent newAgent = f.CreateAgent(a, pc.Mate, pNew);
                                        if (newAgent.Invarient())
                                        {
                                            pNew.Hitpoints = newAgent.Health;
                                            pNew.Hunger = 0;
                                            pNew.ProcreationCountDown = AIModifiers.initialProcreationCount;
                                            pNew.Position = new AIVector(a.Position.X, a.Position.Y);
                                            toBeAdded.Add(newAgent);
                                            status = true;
                                        }
                                        else
                                        {
                                            //throw new Exception("Invarient exception");
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    break;

                case "Feed":
                    {
                        Feed f = (Feed)action;
                        if (f.FoodSource != null)
                        {
                            if (AIVector.Distance(a.Position, f.FoodSource.Position) <= AIModifiers.maxFeedingRange)
                            {
                                int removed = entities.RemoveAll(x => x.Id.ToString() == f.FoodSource.Id.ToString());
                              
                                if (removed > 0)
                                { 
                                    p.Hunger -= AIModifiers.hungerReductionPerFeeding;
                                    if (p.Hunger < 0)
                                    {
                                        p.Hunger = 0;
                                    }
                                    status = true;
                                }
                            }
                        }
                    }
                    break;
                case "Defend":
                    p.Defending = true;
                    status = true;
                    break;

                default:
                    break;
            }

            if (status)
            {
                effects.Add(new VisualEffect(a, action));
            }

            return status;

        }




        /// <summary>
       /// This is called when the game should draw itself.
        /// </summary>
       /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// <param name="surface">Provides the surface to drawupon</param>
       public void Draw(TimeSpan gameTime, Graphics surface)
       {
           surface.Clear(Color.White);
           foreach (IEntity e in entities)
           {
               Image texture = typeTextures[e.EntityType];
               surface.DrawImage(texture, e.Position.X, e.Position.Y, texture.Width, texture.Height);

           }

           effects.ForEach(e=>e.Draw(gameTime,surface));
       }
    }


    
}
