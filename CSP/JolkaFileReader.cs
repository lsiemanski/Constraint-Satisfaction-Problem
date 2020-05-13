using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSP
{
    class JolkaFileReader
    {
        public Jolka generateJolkaFromFile(string puzzleFilepath, string wordsFilepath)
        {
            IList<Point[]> points = new List<Point[]>();
            IList<Variable<string>> variables = new List<Variable<string>>();
            Dictionary<Variable<string>, IList<Constraint<string>>> constraintDicitonary = new Dictionary<Variable<string>, IList<Constraint<string>>>();
            IList<Constraint<string>> constraints = new List<Constraint<string>>();

            using (StreamReader sr = new StreamReader(puzzleFilepath))
            {
                string line;
                int lineNumber = 0;
                bool wordReading = false;
                Constraint<string> newConstraint;
                while ((line = sr.ReadLine()) != null)
                {
                    points.Add(new Point[line.Length]);
                    Variable<string> newVariable = new Variable<string>();
                    int variablePosition = 0;
                    for (int i = 0; i < line.Length; i++)
                    {
                        if (line[i] == '_')
                        {
                            wordReading = true;
                            if (variablePosition == 0)
                            {
                                newVariable = new Variable<string>();
                                variables.Add(newVariable);
                            }
                            variablePosition++;
                            points[lineNumber][i] = new Point
                            {
                                empty = false,
                                horizontally = new Tuple<Variable<string>, int>(newVariable, variablePosition)
                            };
                        }
                        else
                        {
                            points[lineNumber][i] = new Point
                            {
                                empty = true
                            };

                            if (wordReading)
                            {
                                if (!constraintDicitonary.ContainsKey(newVariable))
                                    constraintDicitonary.Add(newVariable, new List<Constraint<string>>());

                                newConstraint = new WordLengthConstraint()
                                {
                                    Variables = new List<Variable<string>>(),
                                    Length = variablePosition
                                };
                                newConstraint.Variables.Add(newVariable);
                                constraintDicitonary[newVariable].Add(newConstraint);
                                constraints.Add(newConstraint);
                            }

                            wordReading = false;
                            variablePosition = 0;
                        }
                    }
                    if (!constraintDicitonary.ContainsKey(newVariable))
                        constraintDicitonary.Add(newVariable, new List<Constraint<string>>());

                    if (wordReading)
                    {
                        newConstraint = new WordLengthConstraint()
                        {
                            Variables = new List<Variable<string>>(),
                            Length = variablePosition
                        };
                        newConstraint.Variables.Add(newVariable);
                        constraintDicitonary[newVariable].Add(newConstraint);
                        constraints.Add(newConstraint);
                    }
                    wordReading = false;
                    lineNumber++;
                }
            }

            for (int i = 0; i < points[0].Length; i++)
            {
                for (int j = 0; j < points.Count; j++)
                {
                    if (j == 0)
                    {
                        if (!points[j][i].empty && !points[j + 1][i].empty)
                        {
                            Variable<string> newVariable = new Variable<string>();
                            points[j][i].vertically = new Tuple<Variable<string>, int>(newVariable, 1);
                            variables.Add(newVariable);
                        }
                    }
                    else
                    {
                        if (!points[j][i].empty)
                        {
                            if (points[j - 1][i].vertically != null)
                            {
                                points[j][i].vertically = new Tuple<Variable<string>, int>(points[j - 1][i].vertically.Item1, points[j - 1][i].vertically.Item2 + 1);
                            }
                            else if (points[j - 1][i].empty && j != points.Count - 1 && !points[j + 1][i].empty)
                            {
                                Variable<string> newVariable = new Variable<string>();
                                points[j][i].vertically = new Tuple<Variable<string>, int>(newVariable, 1);
                                variables.Add(newVariable);
                            }
                        } 
                        else
                        {
                            if (points[j-1][i].vertically != null)
                            {
                                Variable<string> variable = points[j - 1][i].vertically.Item1;
                                WordLengthConstraint wordLengthConstraint = new WordLengthConstraint { Variables = new List<Variable<string>>(), Length = points[j - 1][i].vertically.Item2 };
                                wordLengthConstraint.Variables.Add(variable);
                                addConstraintToDictionary(variable, wordLengthConstraint, constraintDicitonary);
                            }
                        }
                    }

                    if(j == points.Count - 1)
                    {
                        if (!points[j-1][i].empty && !points[j][i].empty)
                        {
                            Variable<string> variable = points[j][i].vertically.Item1;
                            WordLengthConstraint wordLengthConstraint = new WordLengthConstraint { Variables = new List<Variable<string>>(), Length = points[j][i].vertically.Item2 };
                            wordLengthConstraint.Variables.Add(variable);
                            addConstraintToDictionary(variable, wordLengthConstraint, constraintDicitonary);
                        }
                    }
                }
            }

            foreach (Point[] pointsLine in points)
            {
                foreach (Point point in pointsLine)
                {
                    if (point.horizontally != null && point.vertically != null)
                    {
                        Constraint<string> newConstraint = new WordsCrossingConstraint()
                        {
                            Variables = new List<Variable<string>>(),
                            HorizontalCrossingPoint = point.horizontally,
                            VerticalCrossingPoint = point.vertically
                        };
                        newConstraint.Variables.Add(point.horizontally.Item1);
                        newConstraint.Variables.Add(point.vertically.Item1);
                        constraints.Add(newConstraint);

                        addConstraintToDictionary(point.horizontally.Item1, newConstraint, constraintDicitonary);
                        addConstraintToDictionary(point.vertically.Item1, newConstraint, constraintDicitonary);
                    }
                }
            }

            IList<string> words = new List<string>();

            using (StreamReader sr = new StreamReader(wordsFilepath))
            {
                string line;
                
                while ((line = sr.ReadLine()) != null)
                {
                    words.Add(line);
                }
            }

            AllDiffConstraint<string> allDiffConstraint = new AllDiffConstraint<string>() { Variables = variables };

            foreach(Variable<string> variable in variables)
            {
                variable.Domain = words;
                constraintDicitonary[variable].Add(allDiffConstraint);
            }

            return new Jolka()
            {
                Variables = variables,
                ConstraintDictionary = constraintDicitonary,
                Constraints = constraints,
                Points = points
            };
        }

        private void addConstraintToDictionary(Variable<string> variable, Constraint<string> constraint, Dictionary<Variable<string>, IList<Constraint<string>>> constraintDicitonary)
        {
            if (!constraintDicitonary.ContainsKey(variable))
                constraintDicitonary.Add(variable, new List<Constraint<string>>());
            constraintDicitonary[variable].Add(constraint);
        }
    }
}
