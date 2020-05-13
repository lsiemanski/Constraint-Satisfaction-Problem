using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSP
{
    abstract class Menu<ProblemType, ValueType> where ProblemType : CSProblem<ValueType>
    {
        public abstract void PrintMenu();

        public abstract CSProblem<ValueType>.Select<ValueType> SelectValueSelection(ProblemType problem);

        public abstract CSProblem<ValueType>.Select<Variable<ValueType>> SelectVariableSelection(ProblemType problem);
        
        public void PrintSolutions(ProblemType problem, IList<Dictionary<Variable<ValueType>, ValueType>> solutions)
        {
            if (solutions.Count == 0)
            {
                Console.WriteLine("No solutions!");
            }
            else
            {
                Console.WriteLine("Problem solutions: {0}", solutions.Count);
                foreach (var solution in solutions)
                {
                    Console.WriteLine(problem.GetAssignmentString(solution));
                }
            }
        }

        public void PrintBacktrackingStats(BacktrackingStats backtrackingStats, bool solutionFound)
        {
            Console.WriteLine();
            if (solutionFound)
            {
                Console.WriteLine("Time to first solution (seconds): {0}", backtrackingStats.TimeToFirstSolution.TotalSeconds);
                Console.WriteLine("Nodes visited to first solution: {0}", backtrackingStats.NodesVisitedToFirstSolution);
                Console.WriteLine("Backtracks to first solution: {0}", backtrackingStats.BacktracksToFirstSolution);
            }
            Console.WriteLine("Total time (seconds): {0}", backtrackingStats.TotalTime.TotalSeconds);
            Console.WriteLine("Total nodes visited: {0}", backtrackingStats.TotalNodesVisited);
            Console.WriteLine("Total backtracks: {0}", backtrackingStats.TotalBacktracks);
        }

        public void StartAlgorithm(ProblemType problem)
        {
            BacktrackingStats backtrackingStats = new BacktrackingStats();
            Console.WriteLine("Select algorithm: ");
            Console.WriteLine("[1] Backtrack algorithm");
            Console.WriteLine("[2] Forward checking");
            int selected = int.Parse(Console.ReadLine());
            IList<Dictionary<Variable<ValueType>, ValueType>> solutions;

            switch (selected)
            {
                case 1:
                    solutions = problem.PerformBacktrackingSearch(SelectVariableSelection(problem),
                                                SelectValueSelection(problem),
                                                backtrackingStats);
                    break;
                case 2:
                    solutions = problem.PerformForwardChecking(SelectVariableSelection(problem),
                                                SelectValueSelection(problem),
                                                backtrackingStats);
                    break;
                default:
                    Console.WriteLine("Wrong input!");
                    return;

            }
            
            PrintBacktrackingStats(backtrackingStats, solutions.Count > 0);
            PrintSolutions(problem, solutions);
        }
    }
}
