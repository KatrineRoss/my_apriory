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

        public IList<string> MaximalItemSets { get; set; }

        /// <summary>
        /// Исключённые множества элементов.
        /// </summary>
        public Dictionary<string, Dictionary<string, double>> ClosedItemSets { get; set; }

        /// <summary>
        /// Частовстречающиеся элементы.
        /// </summary>
        public ItemsDictionary FrequentItems { get; set; }
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
