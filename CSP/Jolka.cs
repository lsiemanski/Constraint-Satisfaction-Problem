using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSP
{
    class Jolka : CSProblem<string>
    {
        public IList<Point[]> Points = new List<Point[]>();
        private Dictionary<string, int> wordLetterWeights;

        public override string ToString()
        {
            string toReturn = "";
            foreach (Point[] pointsLine in Points)
            {
                foreach (Point point in pointsLine)
                {
                    toReturn += point;
                    toReturn += " ";
                }
                toReturn += "\n";
            }
            return toReturn;
        }

        public void PrintConstraintString()
        {
            int k = 0;

            foreach (KeyValuePair<Variable<string>, IList<Constraint<string>>> entry in ConstraintDictionary)
            {
                Console.WriteLine(k);
                foreach (Constraint<string> constraint in entry.Value)
                {
                    Console.WriteLine(constraint);
                }
                k++;
            }
        }

        public override string GetAssignmentString(Dictionary<Variable<string>, string> assignment)
        {
            string toReturn = "";
            foreach (Point[] pointsLine in Points)
            {
                foreach (Point point in pointsLine)
                {
                    if(point.empty)
                    {
                        toReturn += "#";
                    }
                    else
                    {
                        if (assignment.ContainsKey(point.horizontally.Item1))
                            toReturn += assignment[point.horizontally.Item1][point.horizontally.Item2 - 1];
                        else
                            toReturn += '.';
                    }
                }
                toReturn += "\n";
            }
            return toReturn;
        }

        public IList<Variable<string>> SelectionByLength(IList<Variable<string>> variables)
        {
            return variables.OrderByDescending(
                item => ConstraintDictionary[item].Where(elem => typeof(WordLengthConstraint).IsInstanceOfType(elem))
                .Select(elem => (WordLengthConstraint)elem).First().Length).ToList();
        }

        public IList<Variable<string>> MostConstraintsThenLengthSelect(IList<Variable<string>> variables)
        {
            return SelectionByLength(MostConstraintsSelect<Variable<string>>(variables));
        }

        public IList<string> SelectionByWordWeights(IList<string> itemList)
        {
            return itemList.OrderByDescending(item => wordLetterWeights[item]).ToList();
        }

        private void initWordLetterWeights()
        {
            Dictionary<char, int> letterAppearances = new Dictionary<char, int>();
            foreach(string domainValue in Variables[0].Domain)
            {
                foreach(char letter in domainValue)
                {
                    if (letterAppearances.ContainsKey(letter))
                        letterAppearances[letter]++;
                    else
                        letterAppearances[letter] = 1;
                }
            }

            wordLetterWeights = new Dictionary<string, int>();

            foreach(string domainValue in Variables[0].Domain)
            {
                wordLetterWeights[domainValue] = 0;
                foreach(char letter in domainValue)
                {
                    wordLetterWeights[domainValue] += letterAppearances[letter];
                }
            }
        }

        protected override void performLocalInitialization()
        {
            initWordLetterWeights();
        }
    }

    class Point
    {
        public bool empty;
        public Tuple<Variable<string>, int> horizontally;
        public Tuple<Variable<string>, int> vertically;

        public override string ToString()
        {
            if (empty)
            {
                return "######";
            }
            else
            {
                string toReturn = "";
                if (horizontally != null)
                    toReturn += string.Format("h{0,2}", horizontally.Item2);
                if (vertically != null)
                    toReturn += string.Format("v{0,2}", vertically.Item2);
                return string.Format("{0,6}", toReturn);
            }
        }
    }
}
