using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AIFramework.Entities;
using AIFramework.Actions;
using AIFramework;

namespace StateAgent.States.AgentStates
{
    class Born : IState<StateBasedAgent>
    {
        private static Born _instance;

        Random rnd;

        public static Born Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Born();
                }
                return _instance;
            }
        }

        private Born()
        {
            rnd = new Random();
        }

        public void Enter(StateBasedAgent obj)
        {

        }

        public void Execute(StateBasedAgent obj)
        {
                obj.Fsm.ChangeState(Mature.Instance);
        }

        public void Exit(StateBasedAgent obj)
        {
                
        }
    }
}
