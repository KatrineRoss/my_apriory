using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApriory
{
    /// <summary>
    /// Класс, представляющий ассоциативное правило.
    /// X ---> Y
    /// </summary>
    public class Rule
    {
        #region Member Variables

        string[] combination;//Левая часть правила (X).
        string[] remaining;//Правая часть правила (Y).
        double confidence;//Достоверность.

        #endregion

        #region Constructor

        public Rule(string[] combination, string[] remaining, double confidence)
        {
            this.combination = combination;
            this.remaining = remaining;
            this.confidence = confidence;
        }

        #endregion

        #region Public Properties

        public string[] X { get { return combination; } }

        public string[] Y { get { return remaining; } }

        public double Confidence { get { return confidence; } }

        #endregion

        #region IComparable<clssRules> Members

        #endregion
        public override int GetHashCode()
        {
            string[] sortedXY = new string[X.Length + Y.Length];
            sortedXY = X.Concat(Y).ToArray();

            Array.Sort(sortedXY);

            return sortedXY.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var other = obj as Rule;
            if (other == null)
            {
                return false;
            }

            return other.X == this.X && other.Y == this.Y ||
                other.X == this.Y && other.Y == this.X;
        }
    }
}
