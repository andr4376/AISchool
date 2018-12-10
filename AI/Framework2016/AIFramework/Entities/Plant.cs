using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIFramework.Entities
{
    [Serializable]
    public class Plant : IEntity
    {

        private Guid id; //Is the id of the Agent
        public Guid Id
        {
            get
            {
                return id;
            }
        }

        public Plant()
        {
            id = Guid.NewGuid();
        }

        public AIVector Position { get; set; }
        public string EntityType
        {
            get { return GetType().Name; }
        }

    }
}
