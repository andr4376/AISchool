using AIFramework;
using AIFramework.Actions;
using AIFramework.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StateAgent.States.AgentStates
{

class Hungry : IState<StateBasedAgent>
    {
       private static Hungry _instance;

       Random rnd;

       public static Hungry Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Hungry();
                }
                return _instance;
            }
        }

        private Hungry()
        {
            rnd = new Random();
        }

        public void Enter(StateBasedAgent obj)
        {

        }

        public void Execute(StateBasedAgent obj)
        {
            if (obj.Hunger < (AIModifiers.maxHungerBeforeHitpointsDamage -  AIModifiers.maxHungerBeforeHitpointsDamage * 0.2) )
            {
                obj.Fsm.ChangeState(Mature.Instance);
                return;
            }
            List<IEntity> inRange = obj.ViewedEntities.FindAll(x => x is Plant);
            inRange.Sort((x, y) => (int)(AIVector.Distance(obj.Position, x.Position) - AIVector.Distance(obj.Position, y.Position)));
            if (inRange.Count > 0)
            { 
                if (AIVector.Distance(obj.Position, inRange[0].Position) < AIModifiers.maxFeedingRange)
                {
                    obj.NextAction = new Feed((Plant)inRange[0]);
                    return;
                }
                else
                {
                    obj.NextAction = new Move((inRange[0].Position - obj.Position));
                    return;
                }
            }

            //Default
            obj.Fsm.ChangeState(new Exploring(ExploringGoal.Food));
        }

        public void Exit(StateBasedAgent obj)
        {

        }
    }
}
