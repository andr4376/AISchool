using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIFramework
{
    public interface IPropertyStorage
    {
        
        Guid Id
        {

            get;

        }



        AIVector Position
        {
            get;
            set;
        }

        float Hunger
        {
            get;

        } //Hunger above AIModifiers.maxHungerBeforeHitpointsDamage and it will start taking hitpoint damage

        float Hitpoints
        {
            get;
        }
        bool Defending
        {
            get;

        }
        float ProcreationCountDown
        {

            get;

        } //Used for making sure some time passes before next procreation (value between 0 - 100, 0 meaning it can procreate again)



        //Attributes - These my be tweeked by AI coding team (but should always have a total sum of AIModifiers.maxAttributePoints) 
        int MovementSpeed
        {

            get;
            set;


        } //Amount of pixels / second the agent can move

        int Strength
        {

            get;
            set;
        } //Damage delt in melee combat
        int Health
        {

            get;
            set;
        } //Amount of starting / maximum hitpoints

        int Eyesight
        {

            get;
            set;


        } //Amount of pixels Agent can see (360 degree vision)

        int Endurance
        {
            get;
            set;
        } //If hunger is below Endurance hitpoints regenerates

        int Dodge
        {
            get;
            set;
        } //Reduces the chance of an attack, is only taking into account if currently defending

    }
}
