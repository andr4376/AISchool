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

    enum ExploringGoal {Food, Enemy, Friend, Random}
    class Exploring : IState<StateBasedAgent>
    {
       
        Random rnd;

        AIVector currentDirection;
        DateTime startTime;
        ExploringGoal goalType;

        public Exploring(ExploringGoal goal)
        {
            goalType = goal;
        }




        public void Enter(StateBasedAgent obj)
        {
            //Calculate new seed based upon hash of id and the current time
            int seed = obj.Id.GetHashCode() + (int) DateTime.Now.ToBinary();

            rnd = new Random(seed);

            currentDirection = new AIVector(rnd.Next(-1, 2), rnd.Next(-1, 2));
            startTime = DateTime.Now;
        }

        public void Execute(StateBasedAgent obj)
        {
            List<IEntity> inRange;
            switch (goalType)
            {
                case ExploringGoal.Enemy:
                    inRange = obj.ViewedEntities.FindAll(x => x.GetType() != typeof(StateBasedAgent) && x is Agent && AIVector.Distance(obj.Position, x.Position) < AIModifiers.maxMeleeAttackRange);
                    if (inRange.Count > 0)
                    {
                        obj.Fsm.ChangeState(Mature.Instance);
                        return;
                    } 
                    break;
                case ExploringGoal.Friend:
                    inRange = obj.ViewedEntities.FindAll(x => x.GetType() == obj.GetType() && AIVector.Distance(obj.Position, x.Position) < AIModifiers.maxMeleeAttackRange);
                    if (inRange.Count > 0)
                    {
                        obj.Fsm.ChangeState(Mature.Instance);
                        return;
                    } 
                    break;
                case ExploringGoal.Food:
                    inRange = obj.ViewedEntities.FindAll(x => x.GetType() == typeof(Plant) && AIVector.Distance(obj.Position, x.Position) < AIModifiers.maxFeedingRange);
                    if (inRange.Count > 0)
                    {
                        obj.Fsm.ChangeState(Hungry.Instance);
                        return;
                    } 
                    break;
                default:
                    break;
            }

            if ((DateTime.Now - startTime).TotalSeconds > 5)
            {
                obj.Fsm.ChangeState(Mature.Instance);
                return;
            }

            obj.NextAction = new Move(currentDirection);
        }

        public void Exit(StateBasedAgent obj)
        {

        }
    }
}
