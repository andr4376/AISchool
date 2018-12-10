using AIFramework.Actions;
using AIFramework.Entities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfLife
{
    class VisualEffect : IDrawable
    {
        float amountOfSecondsvisible = 0;
        IAction action;
        Agent agent;

        public bool ShouldBeRemoved
        {
            get
            {
                if (amountOfSecondsvisible > 0.5)
                {
                    return true;
                }
                return false;
            }
            
        }

        public VisualEffect(Agent agent, IAction action)
        {
            this.agent = agent;
            this.action = action;
        }

        public void Draw(TimeSpan gameTime, Graphics surface)
        {
            amountOfSecondsvisible += (float)gameTime.TotalSeconds;
            switch (action.GetType().Name)
            {
                case "Attack":
                    {
                        Attack atk = (Attack) action;
                        if (atk.Defender != null)
                        {
                            surface.DrawLine(new Pen(Color.Red), agent.Position.X, agent.Position.Y, atk.Defender.Position.X, atk.Defender.Position.Y);
                        }
                    }
                    break;
                case "Procreate":
                    {

                        Procreate pc = (Procreate)action;
                        if (pc.Mate != null)
                        {
                            surface.DrawLine(new Pen(Color.Pink), agent.Position.X, agent.Position.Y, pc.Mate.Position.X, pc.Mate.Position.Y);
                        }
                    }
                    break;

                case "Feed":
                    {
                        Feed f = (Feed)action;
                        if (f.FoodSource != null)
                        {
                            surface.DrawLine(new Pen(Color.Green), agent.Position.X, agent.Position.Y, f.FoodSource.Position.X, f.FoodSource.Position.Y);
                        }
                    }
                    break;
                case "Defend":
                    surface.DrawEllipse(new Pen(Color.Orange),new RectangleF(agent.Position.X, agent.Position.Y, 16, 16));
                    break;

                default:
                    break;
            }


        }
    }
}
