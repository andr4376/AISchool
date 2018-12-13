using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AIFramework;
using AIFramework.Entities;
using System.Diagnostics;
using System.Reflection;

namespace ChadAi
{
    public class ChadFactory : AgentFactory
    {
        int chadCount = 0;

        int fastCount, slowCount = 0;


        public override Agent CreateAgent(IPropertyStorage propertyStorage)
        {
            chadCount++;

            if (chadCount ==1)
            {
                slowCount++;
                Console.WriteLine("Fast: " + fastCount + "  Slow: " + slowCount);
                return new Chad(propertyStorage);

            }
            else
            {
                fastCount++;
                Console.WriteLine("Fast: " + fastCount + "  Slow: " + slowCount);
                chadCount = 0;
                return new Chad(propertyStorage, 1337);

            }
        }

        public override Agent CreateAgent(Agent parent1, Agent parent2, IPropertyStorage propertyStorage)
        {
            chadCount++;

            if (chadCount ==1)

            {
                slowCount++;
                Console.WriteLine("Fast: " + fastCount + "  Slow: " + slowCount);
                return new Chad(propertyStorage);

            }
            else
            {
                fastCount++;
                Console.WriteLine("Fast: " + fastCount + "  Slow: " + slowCount);
                chadCount = 0;
                return new Chad(propertyStorage, 1337);

            }

        }

        public override Type ProvidedAgentType
        {
            get { return typeof(Chad); }
        }

        public override string Creators
        {
            get { return "Andreas Jensen"; }
        }

        public override void RegisterWinners(List<Agent> sortedAfterDeathTime)
        {
            //Do data collection - Perhaps used to evolutionary algoritmen
        }
    }
}

