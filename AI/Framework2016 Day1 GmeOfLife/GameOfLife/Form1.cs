using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameOfLife
{
    public partial class Form1 : Form
    {
        World gameWorld;
        public Form1()
        {
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (gameWorld == null)
            {
                gameWorld = new World(CreateGraphics(), this.DisplayRectangle);
                gameWorld.Start();
            }

            GameInfo gi = gameWorld.GameStatus;
            if (gi.GameFinished)
            {
                if (gi.Won)
                {
                    Text = String.Format("Game ended and winner was {0} with {1} and {2} agents", gi.LeadingCreators, gi.LeadingAgents, gi.LeadingAgentsAmount);
                }
                else
                {
                    Text = String.Format("Game ended but there was no winner!");
                }
            }
            else
            {
                Text = String.Format("{0} Seconds left and current leading creator is {1} with {2} and {3} agents", gi.SecondsLeft,gi.LeadingCreators, gi.LeadingAgents, gi.LeadingAgentsAmount); 
            }



        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (gameWorld != null)
            {
                gameWorld.Stop();

            }
        }
    }
}
