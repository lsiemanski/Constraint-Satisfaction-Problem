using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSP
{
    class SudokuFileReader
    {
        public Sudoku GetSudokuFromFile(int problemId)
        {
            IList<Variable<int>> variables = new List<Variable<int>>();
            Dictionary<Variable<int>, IList<Constraint<int>>> constraintDicitonary = new Dictionary<Variable<int>, IList<Constraint<int>>>();
            IList<Constraint<int>> constraints = new List<Constraint<int>>();

            using (StreamReader sr = new StreamReader("dane/sudoku.csv"))
            {
                string line;
                sr.ReadLine();
                IList<int> typicalDomain = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

                while ((line = sr.ReadLine()) != null)
                {
                    string[] splitted = line.Split(';');
                    if(problemId == Int32.Parse(splitted[0]))
                    {
                        foreach(char value in splitted[2])
                        {
                            Variable<int> newVariable = new Variable<int>();
                            variables.Add(newVariable);

                            if(value == '.')
                            {
                                newVariable.Domain = new List<int>(typicalDomain);
                            }
                            else
                            {
                                int valueNumber = Int32.Parse(value.ToString());
                                newVariable.Domain = new List<int>();
                                newVariable.Domain.Add(valueNumber);
                            }
                        }
                    }
                }
            }

            initConstraints(constraintDicitonary, constraints, variables);

            return new Sudoku()
            {
                ConstraintDictionary = constraintDicitonary,
                Constraints = constraints,
                Variables = variables
            };
        }

        private void initConstraints(Dictionary<Variable<int>, IList<Constraint<int>>> constraintDicitonary, IList<Constraint<int>> constraints, IList<Variable<int>> variables)
        {
            for(int i = 0; i < 9; i++)
                constraints.Add(new AllDiffConstraint<int>() { Variables = variables.Where(item => variables.IndexOf(item) % 9 == i).ToList() });

            for (int i = 0; i < 9; i++)
                constraints.Add(new AllDiffConstraint<int>() { Variables = variables.Where(item => variables.IndexOf(item) / 9 == i).ToList() });

            constraints.Add(generateAllDiffConstraint(variables, new List<int>() { 0, 1, 2, 9, 10, 11, 18, 19, 20 }));
            constraints.Add(generateAllDiffConstraint(variables, new List<int>() { 3, 4, 5, 12, 13, 14, 21, 22, 23 }));
            constraints.Add(generateAllDiffConstraint(variables, new List<int>() { 6, 7, 8, 15, 16, 17, 24, 25, 26 }));
            constraints.Add(generateAllDiffConstraint(variables, new List<int>() { 27, 28, 29, 36, 37, 38, 45, 46, 47 }));
            constraints.Add(generateAllDiffConstraint(variables, new List<int>() { 30, 31, 32, 39, 40, 41, 48, 49, 50 }));
            constraints.Add(generateAllDiffConstraint(variables, new List<int>() { 33, 34, 35, 42, 43, 44, 51, 52, 53 }));
            constraints.Add(generateAllDiffConstraint(variables, new List<int>() { 54, 55, 56, 63, 64, 65, 72, 73, 74 }));
            constraints.Add(generateAllDiffConstraint(variables, new List<int>() { 57, 58, 59, 66, 67, 68, 75, 76, 77 }));
            constraints.Add(generateAllDiffConstraint(variables, new List<int>() { 60, 61, 62, 69, 70, 71, 78, 79, 80 }));

            for(int i = 0; i < variables.Count; i++)
            {
                constraintDicitonary.Add(variables[i], constraints.Where(item => ((AllDiffConstraint<int>)item).Variables.Contains(variables[i])).ToList());
            }
        }

        private AllDiffConstraint<int> generateAllDiffConstraint(IList<Variable<int>> variables, IList<int> indexes)
        {
            return new AllDiffConstraint<int>() { Variables = variables.Where(item => indexes.Contains(variables.IndexOf(item))).ToList() };
        }
    }
}
