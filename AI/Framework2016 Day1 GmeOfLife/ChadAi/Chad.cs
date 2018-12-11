﻿using System;
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

        int height = 618;
        int width = 1028;

        List<AIVector> previousPositions = new List<AIVector>();

        AIVector lastPos = new AIVector(0, 0);

        int actionCount = 0;

        //Only for randomization of movement
        float moveX = 0;
        float moveY = 0;

        AIVector direction = new AIVector(0, 0);

        public Chad(IPropertyStorage propertyStorage)
            : base(propertyStorage)
        {
            rnd = new Random();
            MovementSpeed = 144;
            Strength = 35;
            Health = 36;
            Eyesight = 35;
            Endurance = 0;
            Dodge = 0;


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
                    RememberLocation();


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
                RememberLocation();


                return new Attack(closestEnemy);


            }
            //feed
            List<Plant> nearbyPlantsInRange = plantsInOtherEntities.FindAll(plant => plant
       is Plant && AIVector.Distance(Position, plant.Position) <= AIModifiers.maxFeedingRange);

            if (nearbyPlantsInRange.Count > 0)
            {
                RememberLocation();


                return new Feed(nearbyPlantsInRange[0]);
            }












            //find target to attack
            if ((this as Agent).Hunger < 80)
            {

                List<Agent> nearEnemiesOutOfRange = agents.FindAll(otherAgent => otherAgent.GetType() != typeof(Chad)
                && otherAgent is Agent && AIVector.Distance(Position, otherAgent.Position) > AIModifiers.maxMeleeAttackRange);

                if (nearEnemiesOutOfRange.Count > 0)
                {
                    moveX = nearEnemiesOutOfRange[0].Position.X - Position.X;
                    moveY = nearEnemiesOutOfRange[0].Position.Y - Position.Y;

                    direction = new AIVector(moveX, moveY);

                }
            }

            //find target to attack
            if ((this as Agent).Hunger > 30)
            {

                //find flower to eat
                List<Plant> nearbyPlantsOutOfRange = plantsInOtherEntities.FindAll(plant => plant
            is Plant && AIVector.Distance(Position, plant.Position) > AIModifiers.maxFeedingRange);

                if (nearbyPlantsOutOfRange.Count > 0)
                {


                    Plant closestPlant = nearbyPlantsOutOfRange[0];

                    foreach (Plant plant in nearbyPlantsOutOfRange)
                    {
                        if (AIVector.Distance(Position, plant.Position) <
                            AIVector.Distance(Position, closestPlant.Position))
                        {
                            closestPlant = plant;
                        }
                    }

                    moveX = closestPlant.Position.X - Position.X;
                    moveY = closestPlant.Position.Y - Position.Y;

                    direction = new AIVector(moveX, moveY);

                    if ((this as Agent).Hunger > 80)
                    {
                        RememberLocation();


                        return new Move(direction);
                    }
                }

            }

            //find procreation partner
            List<Chad> nearChadsOutOfProcreationRange = agents.FindAll(otherAgent => otherAgent.GetType() == typeof(Chad)
&& otherAgent is Chad && AIVector.Distance(Position, otherAgent.Position) > AIModifiers.maxProcreateRange).ConvertAll<Chad>(a => (Chad)a);

            if (nearChadsOutOfProcreationRange.Count > 0 && ProcreationCountDown <= 0)
            {

                Chad closestChad = nearChadsOutOfProcreationRange[0];

                foreach (Chad chad in nearChadsOutOfProcreationRange)
                {
                    if (AIVector.Distance(Position, chad.Position) <
                        AIVector.Distance(Position, closestChad.Position) &&
                        (chad as Agent).ProcreationCountDown <= 0)
                    {
                        closestChad = chad;
                    }
                }

                if ((closestChad as Agent).ProcreationCountDown <= 0
                    && (this as Agent).ProcreationCountDown <= 0 &&
                    closestChad != this)
                {


                    moveX = closestChad.Position.X - Position.X;
                    moveY = closestChad.Position.Y - Position.Y;

                    direction = new AIVector(moveX, moveY);

                    RememberLocation();

                    return new Move(direction);
                }
            }

            if (previousPositions.Count == 10)
            {

                if (Position == previousPositions[0])
                {
                    direction *= -1;

                    if (direction == new AIVector(0, 0))
                    {
                        direction.X = rnd.Next(-1, 2);
                        direction.Y = rnd.Next(-1, 2);

                    }
                }
            }


            //if (lastPos.X == Position.X)
            //{
            //    direction.X = direction.X * -1;

            //}
            //if (lastPos.Y == Position.Y)
            //{
            //    direction.Y = direction.Y * -1;
            //}

            if (actionCount > 700)
            {
                direction.X = rnd.Next(-1, 2);
                direction.Y = rnd.Next(-1, 2);

                actionCount = 0;
            }


            RememberLocation();

            return new Move(direction);

        }

        private void RememberLocation()
        {
            if (!previousPositions.Contains(Position))
            {
                previousPositions.Add(Position);

                if (previousPositions.Count > 10)
                {
                    previousPositions.Clear();
                }
            }
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
