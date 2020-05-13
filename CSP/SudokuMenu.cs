using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSP
{
    class SudokuMenu : Menu<Sudoku, int>
    {
        private const string SUDOKU_PATH = "./dane/sudoku.csv";
        public override void PrintMenu()
        {
            IList<Tuple<int, double>> sudokuProblemInstances = new List<Tuple<int, double>>();
            using (StreamReader sr = new StreamReader(SUDOKU_PATH))
            {
                sr.ReadLine();
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] splitted = line.Split(';');
                    sudokuProblemInstances.Add(new Tuple<int, double>(int.Parse(splitted[0]), double.Parse(splitted[1], CultureInfo.InvariantCulture)));
                }
            }

            foreach(Tuple<int, double> sudokuInstance in sudokuProblemInstances)
            {
                Console.WriteLine("[{0}] - difficulty: {1}", sudokuInstance.Item1, sudokuInstance.Item2);
            }

            Console.Write("Select problem instance: ");
            int selected = int.Parse(Console.ReadLine());
            int problemNumber = sudokuProblemInstances.Select(item => item.Item1).Where(item => item == selected).FirstOrDefault();
            if(problemNumber != 0)
            {
                Sudoku sudoku = new SudokuFileReader().GetSudokuFromFile(problemNumber);
                StartAlgorithm(sudoku);
            }
            else
            {
                Console.WriteLine("Problem of this number does not exist!");
            }
        }

        public override CSProblem<int>.Select<int> SelectValueSelection(Sudoku sudoku)
        {
            Console.WriteLine();
            Console.WriteLine("Select value selection heuristic");
            Console.WriteLine("[1] Random");
            Console.WriteLine("[2] Default order");
            int option = int.Parse(Console.ReadLine());
            switch (option)
            {
                case 1:
                    return CSProblem<int>.RandomSelect;
                case 2:
                    return CSProblem<int>.OrderedSelect;
                default:
                    return null;
            }
        }

        public override CSProblem<int>.Select<Variable<int>> SelectVariableSelection(Sudoku sudoku)
        {
            Console.WriteLine();
            Console.WriteLine("Select variable selection heuristic");
            Console.WriteLine("[1] Random (not recommended if you don't want to wait for long time)");
            Console.WriteLine("[2] Default order");
            Console.WriteLine("[3] By most const values in constraints");
            int option = int.Parse(Console.ReadLine());
            switch (option)
            {
                case 1:
                    return CSProblem<int>.RandomSelect;
                case 2:
                    return CSProblem<int>.OrderedSelect;
                case 3:
                    return sudoku.SelectionByConstValuesInConstraints;
                default:
                    return null;
            }
        }
    }
}
