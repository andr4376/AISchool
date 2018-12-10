using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AIFramework.Actions;
using System.Diagnostics;
using System.Reflection;

namespace AIFramework.Entities
{
    public abstract class Agent : MarshalByRefObject, IEntity
    {
        private IPropertyStorage ps;

        
        public Guid Id
        {
            get
            {
                return ps.Id;
            }
        }

        
        //State attributes - These may only be modified by framework
       // private AIVector position;
        public AIVector Position
        {
            get
            {
                return ps.Position;
            }
        }
        
        public  float Hunger {
            get
            {
                return ps.Hunger;
            }

        } //Hunger above AIModifiers.maxHungerBeforeHitpointsDamage and it will start taking hitpoint damage

        public  float Hitpoints {
            get
            {
                return ps.Hitpoints;
            }
        }
        public bool Defending {
            get
            {
                return ps.Defending;
            }
        
        }
        public float ProcreationCountDown
        {
            get
            {
                return ps.ProcreationCountDown;
            }
        
        } //Used for making sure some time passes before next procreation (value between 0 - 100, 0 meaning it can procreate again)



        //Attributes - These my be tweeked by AI coding team (but should always have a total sum of AIModifiers.maxAttributePoints) 
        public  int MovementSpeed {
            get
            {
                return ps.MovementSpeed;
            }
            set
            {
                ps.MovementSpeed = value;
            }
        

        } //Amount of pixels / second the agent can move

        public int Strength
        {
            get
            {
                return ps.Strength;
            }
            set
            {
                ps.Strength = value;
            }
        } //Damage delt in melee combat
        public int Health
        {
            get
            {
                return ps.Health;
            }
            set
            {
                ps.Health = value;
            }
        } //Amount of starting / maximum hitpoints

        public  int Eyesight {
            get
            {
                return ps.Eyesight;
            }
            set
            {
                ps.Eyesight = value;
            }

        
        } //Amount of pixels Agent can see (360 degree vision)

        public  int Endurance {
            get
            {
                return ps.Endurance;
            }
            set
            {
                ps.Endurance = value;
            }
        } //If hunger is below Endurance hitpoints regenerates

        public  int Dodge {
            get
            {
                return ps.Dodge;
            }
            set
            {
                ps.Dodge = value;
            }

        
        } //Reduces the chance of an attack, is only taking into account if currently defending

        public virtual string EntityType
        {
            get { return GetType().Name; }
        }


        public Agent(IPropertyStorage propertyStorage)
        {
            ps = propertyStorage;
        }



        /// <summary>
        /// Checks to see if attributes of Agent is valid
        /// </summary>
        /// <returns></returns>
        public bool Invarient()
        {
            return (AIModifiers.maxAttributePoints == MovementSpeed + Strength + Health + Eyesight  + Endurance + Dodge);
        }

        /// <summary>
        /// Asks agent to provide it's next action
        /// </summary>
        /// <param name="otherEntities">List of other entities within eyesight</param>
        /// <returns>Action to do</returns>
        public abstract IAction GetNextAction(List<IEntity> otherEntities);

        /// <summary>
        /// Provide agent with feedback about last action
        /// </summary>
        /// <param name="success">Was the action a success or not</param>
        public abstract void ActionResultCallback(bool success);
    }
}
