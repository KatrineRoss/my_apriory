using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApriory
{
    static class SortUtils
    {
        public static bool IsEqual<T>(T[] firstArray, T[] secondArray) {

            for (int i = 0; i < firstArray.Length; i++) {
                if (Comparer<T>.Default.Compare(firstArray[i], secondArray[i]) != 0) {
                    return false;
                }
            }

            return true;
        }

        public static string[] SortSet(string[] set) {
            string[] result = new string[set.Length];

            //Array.Sort()

            return result;
        }
    }
}
