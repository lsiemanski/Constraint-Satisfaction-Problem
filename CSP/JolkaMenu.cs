using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSP
{
    class JolkaMenu : Menu<Jolka, string>
    {
        private const string JOLKA_PATH = "./dane/Jolka";
        private const string PUZZLE_FILE_START = "puzzle";
        private const string WORDS_FILE_START = "words";
        public override void PrintMenu()
        {
            Console.Write("Select problem number to solve: ");
            int problemNumber = int.Parse(Console.ReadLine());
            
            if(File.Exists(string.Format("{0}/{1}{2}", JOLKA_PATH, PUZZLE_FILE_START, problemNumber)))
            {
                Jolka jolka = new JolkaFileReader().generateJolkaFromFile(string.Format("{0}/{1}{2}", JOLKA_PATH, PUZZLE_FILE_START, problemNumber),
                                                                          string.Format("{0}/{1}{2}", JOLKA_PATH, WORDS_FILE_START, problemNumber));
                StartAlgorithm(jolka);
            } 
            else
            {
                Console.WriteLine("Problem of this number does not exist!");
            }
        }

        public override CSProblem<string>.Select<string> SelectValueSelection(Jolka jolka)
        {
            Console.WriteLine();
            Console.WriteLine("Select value selection heuristic");
            Console.WriteLine("[1] Random");
            Console.WriteLine("[2] Default order");
            Console.WriteLine("[3] By word weights (weights are counted by total appearances of word letters in whole domain");
            int option = int.Parse(Console.ReadLine());
            switch(option)
            {
                case 1:
                    return CSProblem<string>.RandomSelect;
                case 2:
                    return CSProblem<string>.OrderedSelect;
                case 3:
                    return jolka.SelectionByWordWeights;
                default:
                    return null;
            }
        }

        public override CSProblem<string>.Select<Variable<string>> SelectVariableSelection(Jolka jolka)
        {
            Console.WriteLine();
            Console.WriteLine("Select variable selection heuristic");
            Console.WriteLine("[1] Random");
            Console.WriteLine("[2] Default order");
            Console.WriteLine("[3] By most constraints");
            Console.WriteLine("[4] By longest words");
            Console.WriteLine("[5] By most constraints then by longest words");
            int option = int.Parse(Console.ReadLine());
            switch (option)
            {
                case 1:
                    return CSProblem<string>.RandomSelect;
                case 2:
                    return CSProblem<string>.OrderedSelect;
                case 3:
                    return jolka.MostConstraintsSelect<string>;
                case 4:
                    return jolka.SelectionByLength;
                case 5:
                    return jolka.MostConstraintsThenLengthSelect;
                default:
                    return null;
            }
        }
    }
}
