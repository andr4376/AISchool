using AIFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfLife
{
    class PropertyProvider :MarshalByRefObject, IPropertyStorage
    {
        private bool onlyOnceMovementSpeed = false;
        private bool onlyOnceStrength = false;
        private bool onlyOnceHealth = false;
        private bool onlyOnceEyesight = false;
        private bool onlyOnceEndurance = false;
        private bool onlyOnceDodge = false;
 

        private Guid id; //Is the id of the Agent
        public Guid Id
        {
            get
            {
                return id;
            }
        }

        public PropertyProvider()
        {
            id = Guid.NewGuid();
        }


        private AIVector position;
        public AIVector Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
            }
        }

        private float hunger;
        public float Hunger
        {
            get { return hunger; }
            set {
                if (value < 0)
                {
                    value = 0;
                }
                hunger = value;
            }
        }

        private float hitpoints;
        public float Hitpoints
        {
            get { return hitpoints; }
            set {
                if (value < 0)
                {
                    value = 0;
                }
                hitpoints = value;
            }
        }


        private bool defending;
        public bool Defending
        {
            get { return defending; }
            set { defending = value; }
        }

        private float procreationCountDown;
        public float ProcreationCountDown
        {
            get { return procreationCountDown; }
            set { 
                procreationCountDown = value;
            }
        }

        private int movementSpeed;
        public int MovementSpeed
        {
            get { return movementSpeed; }
            set {
                if (value < 0)
                {
                    value = 0;
                }
                if (!onlyOnceMovementSpeed)
                {
                    onlyOnceMovementSpeed = true;
                    movementSpeed = value;
                }
                
            }
        }

        private int strength;
        public int Strength
        {
            get { return strength; }
            set
            {
                if (value < 0)
                {
                    value = 0;
                }
                if (!onlyOnceStrength)
                {
                    onlyOnceStrength = true;
                    strength = value;
                }
            }
        }

        private int health;
        public int Health
        {
            get { return health; }
            set
            {
                if (value < 0)
                {
                    value = 0;
                }
                if (!onlyOnceHealth)
                {
                    onlyOnceHealth = true;
                    health = value;
                }
            }
        }

        private int eyesight;
        public int Eyesight
        {
            get { return eyesight; }
            set
            {
                if (value < 0)
                {
                    value = 0;
                }
                if (!onlyOnceEyesight)
                {
                    onlyOnceEyesight = true;
                    eyesight = value;
                }
            }
        }


        private int endurance;
        public int Endurance
        {
            get { return endurance; }
            set
            {
                if (value < 0)
                {
                    value = 0;
                }
                if (!onlyOnceEndurance)
                {
                    onlyOnceEndurance = true;
                    endurance = value;
                }
            }
        }


        private int dodge;
        public int Dodge
        {
            get { return dodge; }
            set
            {
                if (value < 0)
                {
                    value = 0;
                }
                if (!onlyOnceDodge)
                {
                    onlyOnceDodge = true;
                    dodge = value;
                }
            }
        }
    }
}
