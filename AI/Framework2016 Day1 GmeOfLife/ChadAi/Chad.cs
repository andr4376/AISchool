using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AIFramework;
using AIFramework.Actions;
using AIFramework.Entities;
using System.Diagnostics;


namespace ChadAi
{
    public class Chad : Agent
    {
        int chadCount = 0;

        Random rnd;

        AIVector lastPosition = new AIVector(0, 0);

        AIVector lastPos = new AIVector(0, 0);

        public AIVector LastPos
        {
            get { return lastPos; }
            set { lastPos = value; }
        }

        public List<Plant> plantS = new List<Plant>();

        int actionCount, rememberPositionCount = 0;

        //Only for randomization of movement
        float moveX = 0;
        float moveY = 0;

        AIVector direction = new AIVector(0, 0);
        private int waitTime = 200;

        public Chad(IPropertyStorage propertyStorage)
            : base(propertyStorage)
        {
            rnd = new Random();
            //MovementSpeed = 144;
            //Strength = 35;
            //Health = 36;
            //Eyesight = 35;
            //Endurance = 0;
            Dodge = 0;



            while (MovementSpeed + Strength + Health + Eyesight + Endurance != 250)
            {

                MovementSpeed = rnd.Next(10, 90);
                Strength = rnd.Next(10, 90);
                Health = rnd.Next(10, 90);
                Eyesight = rnd.Next(10, 90);
                Endurance = rnd.Next(10, 90);

                Console.WriteLine((MovementSpeed + Strength + Health + Eyesight + Endurance));
            }

            chadCount++;
            Console.WriteLine(chadCount);
            moveX = rnd.Next(-1, 2);
            moveY = rnd.Next(-1, 2);

            direction = new AIVector(moveX, moveY);

            string ddd = this.GetType().FullName;
        }



        public override IAction GetNextAction(List<IEntity> otherEntities)
        {
            actionCount++;

            List<Agent> agents = otherEntities.FindAll(a => a is Agent && a != this).ConvertAll<Agent>(a => (Agent)a);
            List<Plant> plantsInOtherEntities = otherEntities.FindAll(a => a is Plant).ConvertAll<Plant>(a => (Plant)a);

            plantS = new List<Plant>(plantsInOtherEntities);


            //procreate
            List<Chad> nearChadsInProcreationRange = agents.FindAll(otherAgent => otherAgent.GetType() == typeof(Chad)
    && otherAgent is Agent && AIVector.Distance(Position, otherAgent.Position) <= AIModifiers.maxProcreateRange)
    .ConvertAll<Chad>(a => (Chad)a);

            if (nearChadsInProcreationRange.Count > 0 && ProcreationCountDown <= 0)
            {
                Chad closestChad = nearChadsInProcreationRange[0];

                foreach (Chad chad in nearChadsInProcreationRange)
                {
                    if (AIVector.Distance(Position, chad.Position) <
                        AIVector.Distance(Position, closestChad.Position) &&
                        (chad as Agent).ProcreationCountDown <= 0)
                    {
                        closestChad = chad;
                    }
                }


                if ((closestChad as Agent).ProcreationCountDown <= 0 &&
                    AIVector.Distance(Position, closestChad.Position) <= AIModifiers.maxProcreateRange &&
                    closestChad != this)
                {
                    Console.WriteLine("Procreate");

                    RememberPosition();
                    return new Procreate(nearChadsInProcreationRange[0]);
                }

            }



            //attack
            List<Agent> nearEnemiesInMeleeRange = agents.FindAll(otherAgent =>
        otherAgent.GetType() != typeof(Chad)
        && otherAgent is Agent && AIVector.Distance(Position, otherAgent.Position)
        <= AIModifiers.maxMeleeAttackRange);

            if (nearEnemiesInMeleeRange.Count > 0)
            {
                Agent closestEnemy = nearEnemiesInMeleeRange[0];

                foreach (Agent agent in nearEnemiesInMeleeRange)
                {
                    if (AIVector.Distance(Position, agent.Position) <=
                        AIVector.Distance(Position, closestEnemy.Position))
                    {
                        closestEnemy = agent;
                    }
                }


                RememberPosition();
                return new Attack(closestEnemy);




            }
            //feed
            List<Plant> nearbyPlantsInRange = plantsInOtherEntities.FindAll(plant => plant
       is Plant && AIVector.Distance(Position, plant.Position) <= AIModifiers.maxFeedingRange);

            if (nearbyPlantsInRange.Count > 0)
            {
                if ((this as Agent).Hunger > 20)
                {
                    RememberPosition();
                    return new Feed(nearbyPlantsInRange[0]);
                }


            }

            //find procreation partner
            List<Chad> nearChadsOutOfProcreationRange = agents.FindAll(otherAgent => otherAgent.GetType() == typeof(Chad)
&& otherAgent is Chad && AIVector.Distance(Position, otherAgent.Position) > AIModifiers.maxProcreateRange).ConvertAll<Chad>(a => (Chad)a);

            if (nearChadsOutOfProcreationRange.Count > 0 && ProcreationCountDown <= 5)
            {

                Chad closestChad = nearChadsOutOfProcreationRange[0];

                foreach (Chad chad in nearChadsOutOfProcreationRange)
                {
                    if (AIVector.Distance(Position, chad.Position) <
                        AIVector.Distance(Position, closestChad.Position) &&
                        (chad as Agent).ProcreationCountDown <= closestChad.ProcreationCountDown)
                    {
                        closestChad = chad;
                    }
                }

                if (closestChad.ProcreationCountDown <= 5
                    && ProcreationCountDown <= 5 &&
                    closestChad != this)
                {


                    moveX = closestChad.Position.X - Position.X;
                    moveY = closestChad.Position.Y - Position.Y;

                    direction = new AIVector(moveX, moveY);

                    RememberPosition();

                    return new Move(direction);
                }

            }
            //avoid
            if (nearChadsOutOfProcreationRange.Count > 0 && ProcreationCountDown >= 10)
            {

                Chad closestChadoutOfRange = nearChadsOutOfProcreationRange[0];

                foreach (Chad chad in nearChadsOutOfProcreationRange)
                {
                    if (AIVector.Distance(Position, chad.Position) <
                        AIVector.Distance(Position, closestChadoutOfRange.Position))

                    {
                        closestChadoutOfRange = chad;
                    }

                    moveX = Position.X - closestChadoutOfRange.Position.X;
                    moveY = Position.Y - closestChadoutOfRange.Position.Y;

                    direction = new AIVector(moveX, moveY);
                }
            }











            //find target to attack
            if (Hunger < 80)
            {

                List<Agent> nearEnemiesOutOfRange = agents.FindAll(otherAgent => otherAgent.GetType() != typeof(Chad)
                && otherAgent is Agent && AIVector.Distance(Position, otherAgent.Position) > AIModifiers.maxMeleeAttackRange);



                if (nearEnemiesOutOfRange.Count > 0)
                {
                    Agent closestAgent = nearEnemiesOutOfRange[0];


                    foreach (Agent agent in nearEnemiesOutOfRange)
                    {
                        if (AIVector.Distance(Position, agent.Position) <
                            AIVector.Distance(Position, closestAgent.Position))
                        {
                            if ((Health + Strength) > (closestAgent.Health + closestAgent.Strength))
                            {
                                closestAgent = agent;
                            }

                        }
                    }


                    if (closestAgent != null && ProcreationCountDown > 0 && (Health + Strength) > (closestAgent.Health + closestAgent.Strength))
                    {
                        moveX = closestAgent.Position.X - Position.X;
                        moveY = closestAgent.Position.Y - Position.Y;

                        direction = new AIVector(moveX, moveY);

                    }

                    //Runaway
                    else if (closestAgent != null && Health < 5 && nearEnemiesOutOfRange.Count > 0 && MovementSpeed >= (closestAgent.MovementSpeed * 0.8f))
                    {
                        moveX = Position.X - closestAgent.Position.X;
                        moveY = Position.Y - closestAgent.Position.Y;

                        direction = new AIVector(moveX, moveY);
                        RememberPosition();

                        return new Move(direction);
                    }
                }
            }


            //find flower to eat
            if (Hunger > 40)
            {

                List<Plant> nearbyPlantsOutOfRange = plantsInOtherEntities.FindAll(plant => plant
            is Plant && AIVector.Distance(Position, plant.Position) > AIModifiers.maxFeedingRange);

                if (nearbyPlantsOutOfRange.Count > 0)
                {

                    bool saveForTeam = false;




                    Plant closestPlant = nearbyPlantsOutOfRange[0];

                    foreach (Plant plant in nearbyPlantsOutOfRange)
                    {
                        if (AIVector.Distance(Position, plant.Position) <
                            AIVector.Distance(Position, closestPlant.Position))
                        {
                            closestPlant = plant;
                        }
                    }

                    foreach (Chad chad in nearChadsOutOfProcreationRange)
                    {
                        if (chad.Hunger > Hunger && chad.plantS.Contains(closestPlant))
                        {
                            saveForTeam = true;
                        }
                    }

                    if (!saveForTeam)
                    {

                        moveX = closestPlant.Position.X - Position.X;
                        moveY = closestPlant.Position.Y - Position.Y;

                        direction = new AIVector(moveX, moveY);

                    }
                    if (Hunger > 75 && !saveForTeam)
                    {

                        RememberPosition();

                        return new Move(direction);
                    }
                }

            }






            if (actionCount > waitTime)
            {
                waitTime = rnd.Next(100, 1000);

                direction.X = rnd.Next(-1, 2);
                direction.Y = rnd.Next(-1, 2);

                while (direction.X == 0 && direction.Y == 0)
                {
                    direction.X = rnd.Next(-1, 2);
                    direction.Y = rnd.Next(-1, 2);
                }

                actionCount = 0;
            }

            //if (Bounce())
            //{
            //    //  actionCount /= 2;
            //}

            Bounce();

            RememberPosition();

            return new Move(direction);

        }

        private bool Bounce()
        {
            bool hasBounced = false;

            if (LastPos != null && rememberPositionCount > 0)
            {


                //to far ´left
                if (Position.X == lastPos.X)
                {
                    if (direction.X != 0)
                    {

                        direction.X *= -1;

                        while (direction.X == 0 && direction.Y == 0)
                        {
                            direction.X = rnd.Next(-1, 2);
                        }
                    }

                    hasBounced = true;

                }
                //down
                if (Position.Y == lastPos.Y && direction.Y != 0)
                {
                    direction.Y *= -1;

                    while (direction.X == 0 && direction.Y == 0)
                    {
                        direction.Y = rnd.Next(-1, 2);
                    }
                    hasBounced = true;


                }

            }

            return hasBounced;
        }


        private void RememberPosition()
        {
            if (rememberPositionCount >= 10)
            {
                if (LastPos != this.Position)
                {


                    AIVector tmp = new AIVector(Position.X, Position.Y);
                    LastPos = tmp;
                    rememberPositionCount = 0;

                }
            }

            rememberPositionCount++;
        }




        public override void ActionResultCallback(bool success)
        {
            //Do nothing - AI dont take success of an action into account
        }


        public override string ToString()
        {
            return "Chad #" + Id;
        }
    }
}
