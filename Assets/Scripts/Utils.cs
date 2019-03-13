using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AStar
{
    class Utils
    {
        static public int FindMinList<T>(List<List<T>> arr) {
            int min = int.MaxValue;
            foreach(var l in arr)
            {
                if (l.Count < min)
                    min = l.Count;
            }
            return min;
        }

        static public int FindMaxList<T>(List<List<T>> arr) {
            int max = int.MinValue;
            foreach (var l in arr)
            {
                if (l.Count > max)
                    max = l.Count;
            }
            return max;
        }

        static public void PadList<T>(ref List<List<T>> arr, int len, T elem) {
            foreach(var l in arr)
            {
                while(l.Count < len)
                {
                    l.Add(elem);
                }
            }
        }
    }
}
