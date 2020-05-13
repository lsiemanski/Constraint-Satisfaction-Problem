using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSP
{
    abstract class CSProblem<ValueType>
    {
        public IList<Variable<ValueType>> Variables;
        public IList<Constraint<ValueType>> Constraints;
        public IDictionary<Variable<ValueType>, IList<Constraint<ValueType>>> ConstraintDictionary;

        public delegate IList<T> Select<T>(IList<T> itemList);
        public abstract string GetAssignmentString(Dictionary<Variable<ValueType>, ValueType> assignment);

        virtual protected Dictionary<Variable<ValueType>, ValueType> GetInitialAssignment()
        {
            return new Dictionary<Variable<ValueType>, ValueType>();
        }

        protected abstract void performLocalInitialization();

        public IList<Dictionary<Variable<ValueType>, ValueType>> PerformBacktrackingSearch(Select<Variable<ValueType>> variableSelection, 
                                                                                           Select<ValueType> domainValueSelection, 
                                                                                           BacktrackingStats backtrackingStats)
        {
            performLocalInitialization();
            IList<Dictionary<Variable<ValueType>, ValueType>> solutions = new List<Dictionary<Variable<ValueType>, ValueType>>();
            backtrackingStats.Start();
            BacktrackingSearch(GetInitialAssignment(), solutions, variableSelection, domainValueSelection, backtrackingStats);
            backtrackingStats.End();
            return solutions;
        }

        public IList<Dictionary<Variable<ValueType>, ValueType>> PerformForwardChecking(Select<Variable<ValueType>> variableSelection, 
                                                                                        Select<ValueType> domainValueSelection, 
                                                                                        BacktrackingStats backtrackingStats)
        {
            performLocalInitialization();
            IList<Dictionary<Variable<ValueType>, ValueType>> solutions = new List<Dictionary<Variable<ValueType>, ValueType>>();
            backtrackingStats.Start();
            Dictionary<Variable<ValueType>, ValueType> initialAssignment = GetInitialAssignment();
            Dictionary<Variable<ValueType>, IList<ValueType>> domainAssignment = generateDefaultDomainAssignment();
            filterDomains(Variables, initialAssignment, domainAssignment, true);
            ForwardChecking(initialAssignment, solutions, variableSelection, domainValueSelection, backtrackingStats, domainAssignment);
            backtrackingStats.End();
            return solutions;
        }

        private Dictionary<Variable<ValueType>, IList<ValueType>> generateDefaultDomainAssignment()
        {
            Dictionary<Variable<ValueType>, IList<ValueType>> domainAssignment = new Dictionary<Variable<ValueType>, IList<ValueType>>();
            foreach(Variable<ValueType> variable in Variables)
                domainAssignment[variable] = new List<ValueType>(variable.Domain);
            
            return domainAssignment;
        }

        protected Dictionary<Variable<ValueType>, ValueType> BacktrackingSearch(Dictionary<Variable<ValueType>, ValueType> assignment, 
                                                                                IList<Dictionary<Variable<ValueType>, ValueType>> solutions,
                                                                                Select<Variable<ValueType>> variableSelection, 
                                                                                Select<ValueType> domainValueSelection,
                                                                                BacktrackingStats backtrackingStats)
        {
            if (assignment.Count == Variables.Count)
            {
                //Console.WriteLine(GetAssignmentString(assignment));
                solutions.Add(new Dictionary<Variable<ValueType>, ValueType>(assignment));
                backtrackingStats.SolutionFound();
                return null;
            }

            IList<Variable<ValueType>> unassigned = variableSelection(Variables.Where(item => !assignment.ContainsKey(item)).ToList());

            foreach (ValueType domainValue in domainValueSelection(unassigned[0].Domain))
            {
                backtrackingStats.AddNodeVisited();
                assignment[unassigned[0]] = domainValue;

                if (isConsistent(unassigned[0], assignment))
                {
                    //Console.WriteLine(GetAssignmentString(assignment));
                    Dictionary<Variable<ValueType>, ValueType> result = BacktrackingSearch(assignment, solutions, variableSelection, domainValueSelection, backtrackingStats);
                    if (result != null)
                        return result;
                }
            }
            assignment.Remove(unassigned[0]);
            backtrackingStats.AddBacktrack();
            return null;
        }

        protected Dictionary<Variable<ValueType>, ValueType> ForwardChecking(Dictionary<Variable<ValueType>, ValueType> assignment,
                                                                             IList<Dictionary<Variable<ValueType>, ValueType>> solutions,
                                                                             Select<Variable<ValueType>> variableSelection,
                                                                             Select<ValueType> domainValueSelection,
                                                                             BacktrackingStats backtrackingStats,
                                                                             Dictionary<Variable<ValueType>, IList<ValueType>> domainAssignment)
        {
            if (assignment.Count == Variables.Count)
            {
                //Console.WriteLine(GetAssignmentString(assignment));
                solutions.Add(new Dictionary<Variable<ValueType>, ValueType>(assignment));
                backtrackingStats.SolutionFound();
                return null;
            }

            IList<Variable<ValueType>> unassigned = variableSelection(Variables.Where(item => !assignment.ContainsKey(item)).ToList());
            Dictionary<Variable<ValueType>, IList<ValueType>> localDomainAssignment = copyDomainAssignment(domainAssignment);
            IList<ValueType> domain = new List<ValueType>(localDomainAssignment[unassigned[0]]);

            foreach (ValueType domainValue in domainValueSelection(domain))
            {
                backtrackingStats.AddNodeVisited();
                assignment[unassigned[0]] = domainValue;
                localDomainAssignment[unassigned[0]].Clear();
                localDomainAssignment[unassigned[0]].Add(domainValue);
                
                filterDomains(Variables.Where(item => !assignment.ContainsKey(item)).ToList(), assignment, localDomainAssignment, false);
                if (isAnyDomainEmpty(localDomainAssignment))
                {
                    assignment.Remove(unassigned[0]);
                    localDomainAssignment = copyDomainAssignment(domainAssignment);
                    continue;
                }

                Dictionary<Variable<ValueType>, ValueType> result = ForwardChecking(assignment, solutions, variableSelection, domainValueSelection, backtrackingStats, localDomainAssignment);
                if (result != null)
                    return result;
                else
                    localDomainAssignment = copyDomainAssignment(domainAssignment);

            }
            assignment.Remove(unassigned[0]);
            backtrackingStats.AddBacktrack();
            return null;
        }

        private Dictionary<Variable<ValueType>, IList<ValueType>> copyDomainAssignment(Dictionary<Variable<ValueType>, IList<ValueType>> domainAssignment)
        {
            Dictionary<Variable<ValueType>, IList<ValueType>> newDomainAssignment = new Dictionary<Variable<ValueType>, IList<ValueType>>();
            foreach (Variable<ValueType> variable in domainAssignment.Keys)
                newDomainAssignment[variable] = new List<ValueType>(domainAssignment[variable]);

            return newDomainAssignment;
        }

        private void filterDomains(IList<Variable<ValueType>> variables, 
                                   Dictionary<Variable<ValueType>, ValueType> assignment,
                                   Dictionary<Variable<ValueType>, IList<ValueType>> domainAssignment,
                                   bool filterAllValuesInDomain)
        {
            Dictionary<Variable<ValueType>, ValueType> localAssignment = new Dictionary<Variable<ValueType>, ValueType>(assignment);
            foreach (Variable<ValueType> variable in variables)
            {
                IList<ValueType> consistentValues = new List<ValueType>();
                foreach (ValueType value in (filterAllValuesInDomain ? variable.Domain : domainAssignment[variable]))
                {
                    localAssignment[variable] = value;
                    if (isConsistent(variable, localAssignment))
                        consistentValues.Add(value);
                }

                domainAssignment[variable].Clear();
                foreach(ValueType value in consistentValues)
                    domainAssignment[variable].Add(value);

                if(!assignment.ContainsKey(variable))
                    localAssignment.Remove(variable);
            }
        }

        private bool isAnyDomainEmpty(Dictionary<Variable<ValueType>, IList<ValueType>> domainAssignment)
        {
            return domainAssignment.Keys.Where(item => domainAssignment[item].Count == 0).ToList().Count > 0;
        }


        private bool isConsistent(Variable<ValueType> variable, Dictionary<Variable<ValueType>, ValueType> assignment)
        {
            foreach(Constraint<ValueType> constraint in ConstraintDictionary[variable])
            {
                if (!constraint.IsConstraintSatisfied(assignment))
                    return false;
            }

            return true;
        }

        static Random random = new Random();
        public static IList<T> RandomSelect<T>(IList<T> itemList)
        {
            return itemList.OrderBy(item => random.Next()).ToList();
        }

        public static IList<T> OrderedSelect<T>(IList<T> itemList)
        {
            return itemList;
        }

        public IList<Variable<ValueType>> MostConstraintsSelect<T>(IList<Variable<ValueType>> itemList)
        {
            return itemList.OrderByDescending(item => ConstraintDictionary[item].Count).ToList();
        }

    }

    class Variable<ValueType>
    {
        public IList<ValueType> Domain;
    }
}
