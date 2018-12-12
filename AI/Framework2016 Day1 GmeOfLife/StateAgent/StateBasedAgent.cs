using AIFramework;
using AIFramework.Actions;
using AIFramework.Entities;
using StateAgent.States;
using StateAgent.States.AgentStates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StateAgent
{
    public class StateBasedAgent : Agent
    {
        FSM<StateBasedAgent> fsm;

        public FSM<StateBasedAgent> Fsm
        {
            get { return fsm; }
        }

        IAction nextAction;
        List<IEntity> viewedEntities;

        public List<IEntity> ViewedEntities
        {
            get { return viewedEntities; }
        }

        public IAction NextAction
        {
            get { return nextAction; }
            set { nextAction = value; }
        }


        public StateBasedAgent(IPropertyStorage propertyStorage)
            : base(propertyStorage)
        
        {
            MovementSpeed = 70;
            Strength = 25;
            Health = 10;
            Eyesight = 35;
            Endurance = 55;
            Dodge = 55;


            //Start state created
            fsm = new FSM<StateBasedAgent>(this);
            fsm.ChangeState(Born.Instance);

        }

        public override IAction GetNextAction(List<IEntity> otherEntities)
        {
            viewedEntities = otherEntities;

            nextAction = null;
            while (nextAction == null)
            {
                fsm.Update();
            }
            return nextAction;
        }

        public override void ActionResultCallback(bool success)
        {
            //throw new NotImplementedException();
        }
    }
}
