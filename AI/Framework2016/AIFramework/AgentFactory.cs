using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AIFramework.Entities;

namespace AIFramework
{
    /// <summary>
    /// Responsible for creating new Agents
    /// </summary>
    public abstract class AgentFactory : MarshalByRefObject
    {
        /// <summary>
        /// Creates a new Agent with any concirns as to its parents
        /// </summary>
        /// <returns>A new Agent</returns>
        public abstract Agent CreateAgent(IPropertyStorage propertyStorage);

        /// <summary>
        /// Creates a new Agent based upon its parents. Used for when Agents procreate
        /// </summary>
        /// <param name="parent1">Parent Agent 1</param>
        /// <param name="parent2">Parent Agent 2</param>
        /// <returns>A new Agent</returns>
        public abstract Agent CreateAgent(Agent parent1, Agent parent2, IPropertyStorage propertyStorage);

        /// <summary>
        /// Is called after game is finished, and used for registring winning agents. 
        /// </summary>
        /// <param name="sortedAfterDeathTime"></param>
        public abstract void RegisterWinners(List<Agent> sortedAfterDeathTime);

        public abstract Type ProvidedAgentType { get; }

        public string ProvidedAgentTypeName
        {
            get { return ProvidedAgentType.Name; }
        }
        public abstract string Creators { get; }

        public override string ToString()
        {
            return ProvidedAgentType.Name + "(" + Creators + ")";
        }
    }
}
