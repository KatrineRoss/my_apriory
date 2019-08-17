using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApriory
{
    class ExelParser
    {
        protected static Microsoft.Office.Interop.Excel.Worksheet worksheet;// Лист книги exel.
        public static List<Transaction> transactions;// Список транзакций.
        public static List<Product> items;// Список продуктов.
        public static string[] itemsArray;// Список продуктов, представленный массивом, для алгоритма.
        public static ApriorySet[] apriorySets;// Список транзакций для алгоритма.

        public ExelParser(Microsoft.Office.Interop.Excel.Worksheet sheet) {
            items = new List<Product>();
            transactions = new List<Transaction>();

            worksheet = sheet;
        }

        /// <summary>
        /// Преобразование файла exel со списком транзакций в транзакции для алгоритма.
        /// </summary>
        /// <returns>Список транзакций.</returns>
        public List<Transaction> parse() {
            int row = 2;
            string transactionId = null;

            getTotalItemsCount();

            while (transactionId != "") {
                transactionId = worksheet.Cells[row, 1].Text.ToString();

                if (transactionId != "") {
                    Transaction set = new Transaction(transactionId, getTransactionSet(row));

                    transactions.Add(set);
                    ++row;
                }
            }

            getApriorySets();

            return transactions;
        }

        /// <summary>
        /// Формирует строку из списока считанных из файла транзакций. 
        /// </summary>
        /// <returns>Строка из списока считанных из файла транзакций.</returns>
        public string getTransactionsForPrint() {
            string result = "";

            for (int i = 0; i < apriorySets.Length; i++) {
                result += apriorySets[i].transactionId + ": " +
                    String.Join<string>(", ", apriorySets[i].products) +
                    "\n";
            }

            return result;
        }

        /// <summary>
        /// Возвращает набор транзакций для анализа алгоритмом.
        /// </summary>
        /// <returns>Набор транзакций для анализа алгоритмом.</returns>
        private static ApriorySet[] getApriorySets() {
            apriorySets = new ApriorySet[transactions.Count];

            for (int i = 0; i < transactions.Count; i++) {
                int productsCount = transactions[i].products.Count;
                string[] items = new string[productsCount];

                for (int j = 0; j < productsCount; j++) {
                    items[j] = transactions[i].products[j].name;
                }

                apriorySets[i] = new ApriorySet(transactions[i].transactionId, items);
            }

            return apriorySets;
        }

        /// <summary>
        /// Возвращает список продуктов из транзакции по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор транзакции (номер строки в exel).</param>
        /// <returns>Список продуктов из транзакции.</returns>
        private static List<Product> getTransactionSet(int id) {
            List<Product> set = new List<Product>();

            for (int i = 2; i <= items.Count + 1; i++) {
                if (worksheet.Cells[id, i].Text.ToString() == "+") {
                    set.Add(items.Find((item) => item.id == i));
                }
            }

            return set;
        }

        /// <summary>
        /// Возвращает побщее кол-во существубщих предметов.
        /// </summary>
        private static int getTotalItemsCount() {
            string cellData = null;
            int col = 2;

            while (cellData != "")
            {
                cellData = worksheet.Cells[1, col].Text;

                if (cellData != "") {
                    Product item = new Product(cellData.ToString(), col);

                    items.Add(item);
                }

                col++;
            }

            itemsArray = new string[items.Count];

            for (int i = 0; i < items.Count; i++) {
                itemsArray[i] = items[i].name;
            }

            return items.Count;
        }
    }

    /// <summary>
    /// Транзакция.
    /// </summary>
    class Transaction {
        public string transactionId;
        public List<Product> products;

        public Transaction(string id, List<Product> items)
        {
            transactionId = id;
            products = items;
        }
    }

    /// <summary>
    /// Транзакция для алгоритма.
    /// </summary>
    class ApriorySet {
        public string transactionId;
        public string[] products;

        public ApriorySet(string id, string[] items) { 
            transactionId = id;
            products = new string[items.Length];
            Array.Copy(items, products, items.Length);
        }
    }

    /// <summary>
    /// Продукт.
    /// </summary>
    class Product {
        public string name;
        public int id;

        public Product(string itemName, int itemId) {
            name = itemName;
            id = itemId;
        }
    }
}
