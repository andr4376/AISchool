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
        Random rnd;

        //Only for randomization of movement
        float moveX = 0;
        float moveY = 0;

        AIVector direction = new AIVector(0, 0);

        public Chad(IPropertyStorage propertyStorage)
            : base(propertyStorage)
        {
            rnd = new Random();
            MovementSpeed = 35;
            Strength = 35;
            Health = 30;
            Eyesight = 70;
            Endurance = 25;
            Dodge = 55;



            moveX = rnd.Next(-1, 2);
            moveY = rnd.Next(-1, 2);

            direction = new AIVector(moveX, moveY);

            string ddd = this.GetType().FullName;
        }



        public override IAction GetNextAction(List<IEntity> otherEntities)
        {


            List<Agent> agents = otherEntities.FindAll(a => a is Agent).ConvertAll<Agent>(a => (Agent)a);
            List<IEntity> plantsasdasd = otherEntities.FindAll(a => a is Plant);


            //attack
            List<IEntity> nearEnemiesInMeleeRange = otherEntities.FindAll(otherAgent => otherAgent.GetType() != typeof(Chad)
            && otherAgent is Agent && AIVector.Distance(Position, otherAgent.Position) <= AIModifiers.maxMeleeAttackRange);

            if (nearEnemiesInMeleeRange.Count > 0)
            {
                return new Attack((Agent)nearEnemiesInMeleeRange[0]);
            }


            //feed
            List<IEntity> nearbyPlantsInRange = otherEntities.FindAll(plant => plant
       is Plant && AIVector.Distance(Position, plant.Position) <= AIModifiers.maxFeedingRange);

            if (nearbyPlantsInRange.Count > 0)
            {
                return new Feed((Plant)nearbyPlantsInRange[0]);
            }

            //procreate
            List<IEntity> nearChadsInProcreationRange = otherEntities.FindAll(otherAgent => otherAgent.GetType() == typeof(Chad)
&& otherAgent is Agent && AIVector.Distance(Position, otherAgent.Position) < AIModifiers.maxProcreateRange);

            if (nearChadsInProcreationRange.Count > 0 && ProcreationCountDown <= 0)
            {
                return new Procreate((Agent)nearChadsInProcreationRange[0]);
            }



            //find procreation partner
            List<IEntity> nearChadsOutOfProcreationRange = otherEntities.FindAll(otherAgent => otherAgent.GetType() == typeof(Chad)
&& otherAgent is Agent && AIVector.Distance(Position, otherAgent.Position) > AIModifiers.maxProcreateRange);

            if (nearChadsOutOfProcreationRange.Count > 0 && ProcreationCountDown <= 0)
            {
                moveX = nearChadsOutOfProcreationRange[0].Position.X - Position.X;
                moveY = nearChadsOutOfProcreationRange[0].Position.Y - Position.Y;

                direction = new AIVector(moveX, moveY);

                direction.Normalize();

                return new Move(direction);

            }




            //find target to attack
            List<IEntity> nearEnemiesOutOfRange = otherEntities.FindAll(otherAgent => otherAgent.GetType() != typeof(Chad)
            && otherAgent is Agent && AIVector.Distance(Position, otherAgent.Position) > AIModifiers.maxMeleeAttackRange);

            if (nearEnemiesOutOfRange.Count > 0)
            {
                moveX = nearEnemiesOutOfRange[0].Position.X - Position.X;
                moveY = nearEnemiesOutOfRange[0].Position.Y - Position.Y;

                direction = new AIVector(moveX, moveY);

                direction.Normalize();
            }

            //find flower to eat
            List<IEntity> nearbyPlantsOutOfRange = otherEntities.FindAll(plant => plant
            is Plant && AIVector.Distance(Position, plant.Position) > AIModifiers.maxFeedingRange);

            if (nearbyPlantsOutOfRange.Count > 0)
            {
                moveX = nearbyPlantsOutOfRange[0].Position.X - Position.X;
                moveY = nearbyPlantsOutOfRange[0].Position.Y - Position.Y;

                direction = new AIVector(moveX, moveY);

                direction.Normalize();
            }


            if (direction.X ==0 && direction.Y ==0)
            {
                direction = new AIVector(rnd.Next(-1,2), rnd.Next(-1, 2));

            }


            return new Move(direction);

        }

        private void CheckAttack(List<IEntity> otherEntities)
        {

        }




        public override void ActionResultCallback(bool success)
        {
            //Do nothing - AI dont take success of an action into account
        }
    }
}
