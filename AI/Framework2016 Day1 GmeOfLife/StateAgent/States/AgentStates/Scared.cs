using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AIFramework.Entities;
using AIFramework.Actions;
using AIFramework;

namespace StateAgent.States.AgentStates
{
    class Scared : IState<StateBasedAgent>
    {
        private static Scared _instance;

        public static Scared Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Scared();
                }
                return _instance;
            }
        }

        private Scared()
        {

        }

        public void Enter(StateBasedAgent obj)
        {

        }

        public void Execute(StateBasedAgent obj)
        {
            if (obj.Hunger >= AIModifiers.maxHungerBeforeHitpointsDamage)
            {
                obj.Fsm.ChangeState(Hungry.Instance);
                return;
            }


            // No other agent with range < AIModifiers.maxMeleeAttackRange
            List<IEntity> inRange = obj.ViewedEntities.FindAll(x => x.GetType() != typeof(StateBasedAgent) && x is Agent && AIVector.Distance(obj.Position, x.Position) < AIModifiers.maxMeleeAttackRange);
            if (inRange.Count == 0)
            {
                obj.Fsm.ChangeState(Mature.Instance);
                return;
            }


            //If speed is higher than any agent with AIModifiers.maxMeleeAttackRange
            bool speedIsHigher = true;
            float xFearCenter = 0;
            float yFearCenter = 0;
            foreach (Agent x in inRange)
            {
                if (x.MovementSpeed > obj.MovementSpeed)
                {
                    speedIsHigher = false;
                    break;
                }
                xFearCenter += x.Position.X;
                yFearCenter += x.Position.Y;
            }
            if (speedIsHigher)
            {
                AIVector fear = new AIVector(xFearCenter / inRange.Count, yFearCenter / inRange.Count);
                obj.NextAction = new Move((obj.Position - fear));
                return;
            }
            else
            {
                int i = 0;
                i++;
            }




            //Default
            obj.NextAction = new Defend();

        }

        public void Exit(StateBasedAgent obj)
        {
                
        }
    }
}
