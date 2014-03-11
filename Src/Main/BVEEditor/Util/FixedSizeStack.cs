using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BVEEditor.Util
{
    /// <summary>
    /// A stack that is fixed in its size.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FixedSizeStack<T> : IEnumerable<T>
    {
        readonly T[] array;

        int head;
        int size;


        public FixedSizeStack(int size)
        {
            array = new T[size];
        }

        public void Push(T val)
        {
            size = Math.Min(array.Length, size + 1);

            head++;

            head = head % size;

            array[head] = val;
        }


        public IEnumerator<T> GetEnumerator()
        {
            var list = new List<T>();

            for(int i = 0, j = head + 1; i < size; i++, j++)
                list.Add(array[j % size]);

            list.Reverse();

            return list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
