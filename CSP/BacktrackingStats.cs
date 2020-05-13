using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace CSP
{
    class BacktrackingStats
    {
        public TimeSpan TimeToFirstSolution { get; private set; }
        public int NodesVisitedToFirstSolution { get; private set; }
        public int BacktracksToFirstSolution { get; private set; }
        public TimeSpan TotalTime { get; private set; }
        public int TotalNodesVisited { get; private set; }
        public int TotalBacktracks { get; private set; }
        private Stopwatch stopwatch;
        private bool solutionFound = false;
        public void Start()
        {
            solutionFound = false;
            TimeToFirstSolution = new TimeSpan();
            NodesVisitedToFirstSolution = 0;
            BacktracksToFirstSolution = 0;
            TotalTime = new TimeSpan();
            TotalNodesVisited = 0;
            TotalBacktracks = 0;
            stopwatch = new Stopwatch();
            stopwatch.Start();
        }

        public void AddNodeVisited()
        {
            if (!solutionFound)
                NodesVisitedToFirstSolution++;
            TotalNodesVisited++;
        }

        public void AddBacktrack()
        {
            if (!solutionFound)
                BacktracksToFirstSolution++;
            TotalBacktracks++;
        }

        public void SolutionFound()
        {
            if(!solutionFound)
                TimeToFirstSolution = stopwatch.Elapsed;
            
            solutionFound = true;
        }

        public void End()
        {
            TotalTime = stopwatch.Elapsed;
            stopwatch.Stop();
        }
    }
}
