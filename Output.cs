using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace MyApriory
{
    class Output
    {
        /// <summary>
        /// Строгие правила, сгенерированные из частых наборов предметов.
        /// </summary>
        public IList<Rule> StrongRules { get; set; }

        /// <summary>
        /// Частовстречающиеся элементы.
        /// </summary>
        public ItemsDictionary FrequentItems { get; set; }

        private static string printProductSet(string[] productSet) {
            string result = "";

            if (productSet.Length == 1) {
                result = productSet[0];
            } else {
                for (int i = 0; i < productSet.Length; i++)
                {
                    result += i != productSet.Length - 1 ?
                        productSet[i] + " + ":
                        productSet[i];
                }
            }

            return result;
        }

        public static string printFrequentItems(ItemsDictionary items) {
            string result = "";

            foreach (Item item in items)
            {
                result += printProductSet(item.ProductSet) + "\n";
            }

            return result;
        }

        private static string printRulePart(string[] rulePart) {
            string result = "";

            for (int i = 0; i < rulePart.Length; i++)
            {
                if (i == 0) {
                    result += "[";
                }
                result += i != rulePart.Length - 1 ?
                    rulePart[i] + ", " :
                    rulePart[i] + "]";
            }

            return result;
        }

        private static string printRule(Rule rule) {
            return printRulePart(rule.X) + " => " + printRulePart(rule.Y);
        }

        public static string printRules(IList<Rule> rules) {
            string result = "";

            foreach (Rule rule in rules)
            {
                result += printRule(rule) + "\n";
            }

            return result;
        }
    }

    public class ItemsDictionary : KeyedCollection<string, Item>
    {
        protected override string GetKeyForItem(Item item)
        {
            return item.Name;
        }

        internal void ConcatItems(IList<Item> frequentItems)
        {
            foreach (var item in frequentItems)
            {
                this.Add(item);
            }
        }
    }
}
