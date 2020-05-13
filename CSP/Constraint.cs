using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSP
{
    abstract class Constraint<ValueType>
    {
        public IList<Variable<ValueType>> Variables;
        public abstract bool IsConstraintSatisfied(Dictionary<Variable<ValueType>, ValueType> assignment);
    }

    class WordLengthConstraint : Constraint<string>
    {
        public int Length;

        public override bool IsConstraintSatisfied(Dictionary<Variable<string>, string> assignment)
        {
            Variable<string> variable = assignment.Keys.Where(item => item.Equals(Variables[0])).FirstOrDefault();

            if (variable == null)
                return true;

            return assignment[variable].Length == Length;
        }

        public override string ToString()
        {
            return Length.ToString();
        }
    }

    class WordsCrossingConstraint : Constraint<string>
    {
        public Tuple<Variable<string>, int> HorizontalCrossingPoint;
        public Tuple<Variable<string>, int> VerticalCrossingPoint;

        public override bool IsConstraintSatisfied(Dictionary<Variable<string>, string> assignment)
        {
            Variable<string> horizontalCrossing = assignment.Keys.Where(item => item.Equals(HorizontalCrossingPoint.Item1)).FirstOrDefault();
            Variable<string> verticalCrossing = assignment.Keys.Where(item => item.Equals(VerticalCrossingPoint.Item1)).FirstOrDefault();

            if (horizontalCrossing == null || verticalCrossing == null)
                return true;

            if (assignment[horizontalCrossing].Length < HorizontalCrossingPoint.Item2 || assignment[verticalCrossing].Length < VerticalCrossingPoint.Item2)
                return true;

            return assignment[horizontalCrossing][HorizontalCrossingPoint.Item2-1] == assignment[verticalCrossing][VerticalCrossingPoint.Item2-1];
        }

        public override string ToString()
        {
            return string.Format("h{0,2}v{1,2}", HorizontalCrossingPoint.Item2, VerticalCrossingPoint.Item2);
        }
    }

    class AllDiffConstraint<T> : Constraint<T>
    {
        public override bool IsConstraintSatisfied(Dictionary<Variable<T>, T> assignment)
        {
            IList<T> values = assignment.Keys.Where(item => Variables.Contains(item)).Select(item => assignment[item]).ToList();

            return values.Distinct().ToList().Count == values.Count;
        }
    }

    class VariableValueConstraint<T> : Constraint<T>
    {
        public T Value;
        public override bool IsConstraintSatisfied(Dictionary<Variable<T>, T> assignment)
        {
            IList<Variable<T>> variables = assignment.Keys.Where(item => Variables.Contains(item)).ToList();

            foreach(Variable<T> variable in variables)
                if (!assignment[variable].Equals(Value))
                    return false;

            return true;
        }
    }
}
