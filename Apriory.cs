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

            HashSet<Rule> rules = GenerateRules(allFrequentItems);
            IList<Rule> strongRules = GetStrongRules(minConfidence, rules, allFrequentItems);

            return new Output
            {
                StrongRules = strongRules,
                FrequentItems = allFrequentItems
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

        /// <summary>
        /// Генерация ассоциативных правил.
        /// </summary>
        /// <param name="allFrequentItems">Все наиболее встречающиеся элементы.</param>
        /// <returns></returns>
        private HashSet<Rule> GenerateRules(ItemsDictionary allFrequentItems)
        {
            var rulesList = new HashSet<Rule>();

            foreach (var item in allFrequentItems)
            {
                if (item.ProductSet.Length > 1)
                {
                    IEnumerable<string[]> subsetsList = GenerateSubsets(item.ProductSet);

                    foreach (var subset in subsetsList)
                    {
                        string[] remaining = GetRemaining(subset, item.ProductSet);
                        Rule rule = new Rule(subset, remaining, 0);

                        if (!rulesList.Contains(rule))
                        {
                            rulesList.Add(rule);
                        }
                    }
                }
            }

            return rulesList;
        }

        /// <summary>
        /// Генерация подмножеств (L/2).
        /// </summary>
        /// <param name="item">Элемент(множество).</param>
        /// <returns></returns>
        private IEnumerable<string[]> GenerateSubsets(string[] item)
        {
            IEnumerable<string[]> allSubsets = new string[][] { };
            int subsetLength = item.Length / 2;

            for (int i = 1; i <= subsetLength; i++)
            {
                IList<string[]> subsets = new List<string[]>();
                GenerateSubsetsRecursive(item, i, new string[item.Length], subsets);
                allSubsets = allSubsets.Concat(subsets);
            }

            return allSubsets;
        }

        /// <summary>
        /// Рекурсивная генерация подмножеств.
        /// </summary>
        /// <param name="item">Элемент(множество).</param>
        /// <param name="subsetLength">Длина подмножества.</param>
        /// <param name="temp">Временная переменная для результирующего подмножества.</param>
        /// <param name="subsets">Подмножество.</param>
        /// <param name="q">Индекс, указывающий конец отрезка для копирования.</param>
        /// <param name="r">Temp-индекс для копирования в подмножество.</param>
        private void GenerateSubsetsRecursive(string[] item, int subsetLength, string[] temp, IList<string[]> subsets, int q = 0, int r = 0)
        {
            if (q == subsetLength)
            {
                List<string> sb = new List<string>();

                for (int i = 0; i < subsetLength; i++)
                {
                    sb.Add(temp[i]);
                }

                subsets.Add(sb.ToArray());
            }

            else
            {
                for (int i = r; i < item.Length; i++)
                {
                    temp[q] = item[i];
                    GenerateSubsetsRecursive(item, subsetLength, temp, subsets, q + 1, i + 1);
                }
            }
        }

        /// <summary>
        /// Получение правой части правила (остатка).
        /// </summary>
        /// <param name="child">Левая часть правила.</param>
        /// <param name="parent">Набор из которого составляем правило.</param>
        /// <returns></returns>
        private string[] GetRemaining(string[] child, string[] parent)
        {
            List<string> result = new List<string>(parent);

            for (int i = 0; i < child.Length; i++)
            {
                result.Remove(child[i]);
            }

            parent = result.ToArray();

            return result.ToArray();
        }

        /// <summary>
        /// Получение ассоциативных правил, удовлетворяющих условие минимальной достоверности.
        /// </summary>
        /// <param name="minConfidence">Минимальная достоверность.</param>
        /// <param name="rules">Хэш с правилами.</param>
        /// <param name="allFrequentItems">Все частовстречающиеся элементы.</param>
        /// <returns></returns>
        private IList<Rule> GetStrongRules(double minConfidence, HashSet<Rule> rules, ItemsDictionary allFrequentItems)
        {
            var strongRules = new List<Rule>();

            foreach (Rule rule in rules)
            {
                string[] xy = new string[rule.X.Length + rule.Y.Length];

                xy = rule.X.Concat(rule.Y).ToArray();

                AddStrongRule(rule, xy, strongRules, minConfidence, allFrequentItems);
            }

            //strongRules.Sort();
            return strongRules;
        }

        /// <summary>
        /// Добавление нового правила, удовлетворяющего условию минимальной достоверности.
        /// </summary>
        /// <param name="rule">Правило.</param>
        /// <param name="XY">Правая часть правила (результирующий набор)</param>
        /// <param name="strongRules">Список правил, удовлетворяющих минимальной достоверности.</param>
        /// <param name="minConfidence">Минимальная достоверность.</param>
        /// <param name="allFrequentItems">Все частовстречающиеся элементы.</param>
        private void AddStrongRule(Rule rule, string[] XY, List<Rule> strongRules, double minConfidence, ItemsDictionary allFrequentItems)
        {
            double confidence = GetConfidence(rule.X, XY, allFrequentItems);

            if (confidence >= minConfidence)
            {
                Rule newRule = new Rule(rule.X, rule.Y, confidence);
                strongRules.Add(newRule);
            }

            confidence = GetConfidence(rule.Y, XY, allFrequentItems);

            if (confidence >= minConfidence)
            {
                Rule newRule = new Rule(rule.Y, rule.X, confidence);
                strongRules.Add(newRule);
            }
        }

        /// <summary>
        /// Возвращает достоверность правила.
        /// </summary>
        /// <param name="X">Элевент из левой части правила.</param>
        /// <param name="XY">Комбинация Х с элементов из правой части правила.</param>
        /// <param name="allFrequentItems"></param>
        /// <returns></returns>
        private double GetConfidence(string[] X, string[] XY, ItemsDictionary allFrequentItems)
        {
            double supportX = FindItemByProducts(X, allFrequentItems).Support;
            double supportXY = FindItemByProducts(XY, allFrequentItems).Support;
            return supportXY / supportX;
        }

        /// <summary>
        /// Находит набор элементов транзакции по списку элементов.
        /// </summary>
        /// <param name="products">Набор.</param>
        /// <param name="items">Список элементов, среди которых ищем.</param>
        /// <returns></returns>
        private Item FindItemByProducts(string[] products, ItemsDictionary items) {
            string[] itemProducts = new string[products.Length];
            double support = 0;

            foreach (Item item in items) {
                if (products.SequenceEqual(item.ProductSet)) {
                    Array.Copy(item.ProductSet, itemProducts, itemProducts.Length);
                    support = item.Support;
                }
            }

            return new Item { Support = support, ProductSet = itemProducts };
        }
    }
}
