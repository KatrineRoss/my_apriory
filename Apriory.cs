using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApriory
{
    class Apriory
    {
        /// <summary>
        /// Запускает процесс поиска ассоциативных правил.
        /// </summary>
        /// <param name="minSupport">Минимальная поддержка (определяется частотностью вхождения элемента).</param>
        /// <param name="minConfidence">Минимальная надёжность (определяется тем, как часто правило срабатывает).</param>
        /// <param name="items">Список предметов.</param>
        /// <param name="transactions">Список транзакций.</param>
        /// <returns></returns>
        public Output processTransaction(double minSupport, double minConfidence, IEnumerable<string> items, ApriorySet[] transactions) {
            IList<Item> frequentItems = GetL1FrequentItems(minSupport, items, transactions);
            ItemsDictionary allFrequentItems = new ItemsDictionary();
            allFrequentItems.ConcatItems(frequentItems);
            IDictionary<string[], double> candidates = new Dictionary<string[], double>();
            double transactionsCount = transactions.Length;

            do
            {
                candidates = GenerateCandidates(frequentItems, transactions);
                frequentItems = GetFrequentItems(candidates, minSupport, transactions.Length);
                allFrequentItems.ConcatItems(frequentItems);
            } while (candidates.Count != 0);

            return new Output
            {

            };
        }

        /// <summary>
        /// Получаем набор часто встречающихся комбинаций первого уровня (состоящих из одного предмета).
        /// </summary>
        /// <param name="minSupport">Минимальная поддержка.</param>
        /// <param name="items">Список предметов.</param>
        /// <param name="transactions"></param>
        /// <returns></returns>
        private IList<Item> GetL1FrequentItems(double minSupport, IEnumerable<string> items, ApriorySet[] transactions)
        {
            var frequentItemsL1 = new List<Item>();
            double transactionsCount = transactions.Count();

            foreach (var item in items)
            {
                string[] i = new string[1];
                i[0] = item;

                double support = GetSupport(i, transactions);

                if (support / transactionsCount >= minSupport)
                {
                    string[] productSet = new string[1];
                    productSet[0] = item;

                    frequentItemsL1.Add(new Item { Name = item, Support = support, ProductSet = productSet });
                }
            }
            frequentItemsL1.Sort();
            return frequentItemsL1;
        }

        /// <summary>
        /// Получение значения поддержки для кандитдата.
        /// </summary>
        /// <param name="generatedCandidate">Кандидат (предмет или набор предметов).</param>
        /// <param name="transactionsList">Список транзакций.</param>
        /// <returns></returns>
        private double GetSupport(string[] generatedCandidate, IEnumerable<ApriorySet> transactionsList)
        {
            double support = 0;

            foreach (ApriorySet transaction in transactionsList)
            {
                if (CheckIsSubset(generatedCandidate, transaction.products))
                {
                    support++;
                }
            }

            return support;
        }

        /// <summary>
        /// Проверяет, является ли предмет или набор предметов подмножеством другого набора предметов.
        /// </summary>
        /// <param name="child">Предмет или множество, которое проверяем на вхождение.</param>
        /// <param name="parent">Множество.</param>
        /// <returns></returns>
        private bool CheckIsSubset(string[] child, string[] parent)
        {
            foreach (string c in child)
            {
                if (Array.Find(parent, (p) => p == c) == null)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Возвращает список всех возможных кандидатов (комбинаций предметов из часто встречающихся).
        /// </summary>
        /// <param name="frequentItems">Часто встречающиеся предметы.</param>
        /// <param name="transactions">Список транзакций.</param>
        /// <returns></returns>
        private Dictionary<string[], double> GenerateCandidates(IList<Item> frequentItems, IEnumerable<ApriorySet> transactions) {
            Dictionary<string[], double> candidates = new Dictionary<string[], double>();

            for (int i = 0; i < frequentItems.Count - 1; i++)
            {
                string[] firstItem = frequentItems[i].ProductSet;
                Array.Sort(firstItem);

                for (int j = i + 1; j < frequentItems.Count; j++)
                {
                    string[] secondItem = frequentItems[j].ProductSet;
                    Array.Sort(secondItem);
                    string[] generatedCandidate = GenerateCandidate(firstItem, secondItem);

                    if (generatedCandidate.Length != 0)
                    {
                        double support = GetSupport(generatedCandidate, transactions);
                        candidates.Add(generatedCandidate, support);
                    }
                }
            }

            return candidates;
        }

        /// <summary>
        /// Генерирует кандидата из двух элементов(наборов элементов).
        /// </summary>
        /// <param name="firstItem">Первый элемент(набор)</param>
        /// <param name="secondItem">Второй элемент(набор)</param>
        /// <returns></returns>
        private string[] GenerateCandidate(string[] firstItem, string[] secondItem)
        {
            int length = firstItem.Length;
            string[] candidate;

            if (length == 1)
            {
                candidate = new string[secondItem.Length + 1];
                candidate[0] = firstItem[0];
                candidate[1] = secondItem[0];

                return candidate;
            }
            else
            {
                string[] firstSubStr = new string[length - 1];
                string[] secondSubStr = new string[length - 1];

                Array.Copy(firstItem, firstSubStr, length - 1);
                Array.Copy(secondItem, secondSubStr, length - 1);

                if (Array.Equals(firstSubStr, secondSubStr))
                {
                    string[] str = new string[1];

                    str[0] = secondItem[length - 1];

                    return firstItem.Concat(str).ToArray();
                }

                candidate = new string[0];

                return candidate;
            }
        }

        /// <summary>
        /// Возвращает наиболее встречающиеся элементы(наборы) из всех кандидатов.
        /// </summary>
        /// <param name="candidates">Кандидаты.</param>
        /// <param name="minSupport">Минимальная поддержка.</param>
        /// <param name="transactionsCount">Общее кол-во транзакций.</param>
        /// <returns></returns>
        private List<Item> GetFrequentItems(IDictionary<string[], double> candidates, double minSupport, double transactionsCount)
        {
            var frequentItems = new List<Item>();

            foreach (var item in candidates)
            {
                if (item.Value / transactionsCount >= minSupport)
                {
                    frequentItems.Add(new Item { ProductSet = item.Key, Support = item.Value });
                }
            }

            return frequentItems;
        }
    }
}
