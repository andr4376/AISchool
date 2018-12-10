using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfLife
{
    struct GameInfo
    {
        public bool GameFinished;

        public bool Won;

        public string LeadingCreators;

        public string LeadingAgents;

        public int LeadingAgentsAmount;

        public int AmountOfAgents;

        public int SecondsLeft;

    }
}
