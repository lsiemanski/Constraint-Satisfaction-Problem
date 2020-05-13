using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSP
{
    class Sudoku : CSProblem<int>
    {
        public override string GetAssignmentString(Dictionary<Variable<int>, int> assignment)
        {
            string toReturn = "";
            for (int i = 0; i < Variables.Count; i++)
            {
                if (i % 9 == 0)
                    toReturn += "\n";

                if (i % 3 == 0)
                    toReturn += " ";

                if(i % 27 == 0)
                    toReturn += "\n ";

                if (assignment.ContainsKey(Variables[i]))
                    toReturn += assignment[Variables[i]];
                else
                    toReturn += ".";
            }

            return toReturn;
        }

        public override string ToString()
        {
            string toReturn = "";
            for(int i = 0; i < Variables.Count; i++)
            {
                if (i % 9 == 0)
                    toReturn += "\n";

                if (Variables[i].Domain.Count == 1)
                {
                    toReturn += Variables[i].Domain[0];
                }
                else
                {
                    toReturn += ".";
                }
            }
            return toReturn;
        }

        protected override Dictionary<Variable<int>, int> GetInitialAssignment()
        {
            IList<Variable<int>> assignedVariables = Variables.Where(item => item.Domain.Count == 1).ToList();
            Dictionary<Variable<int>, int> initialAssignment = new Dictionary<Variable<int>, int>();
            foreach (Variable<int> variable in assignedVariables)
                initialAssignment.Add(variable, variable.Domain[0]);

            return initialAssignment;
        }

        public IList<Variable<int>> SelectionByConstValuesInConstraints(IList<Variable<int>> variables)
        {
            return variables.OrderByDescending(
                item => ConstraintDictionary[item].Sum(elem => elem.Variables.Where(e => e.Domain.Count == 1).Count())).ToList();
        }

        protected override void performLocalInitialization() {}
    }
}
