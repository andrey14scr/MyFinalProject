using MiniBot.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniBot.Activity
{
    class Basket<T> : IEnumerator<(T product, short id, short amount)>, IEnumerable where T : IShowInfo, IProduct
    {
        private List<(T item, short id, short amount)> _listOfItems = new List<(T item, short id, short amount)>();

        public float TotalPrice
        {
            get
            {
                float summary = 0;
                for (int i = 0; i < this.Count; i++)
                {
                    summary += _listOfItems[i].item.Cost * _listOfItems[i].amount;
                }
                return summary;
            }
        }

        private int _position = 0;

        public int Count 
        {
            get
            {
                return _listOfItems.Count;
            }
        }

        public (T product, short id, short amount) Current => _listOfItems[_position];

        object IEnumerator.Current => _listOfItems[_position];

        public void Add(T item, short id, short amount = 1)
        {
            int index = _listOfItems.FindIndex(x => x.item.Equals(item));

            if (index != -1)
            {
                _listOfItems[index] = (_listOfItems[index].item, id, (short)(_listOfItems[index].amount + amount));
                if (_listOfItems[index].amount == 0)
                    _listOfItems.RemoveAt(index);
            }
            else
                _listOfItems.Add((item, id, amount));
        }

        public void Clear()
        {
            _listOfItems.Clear();
        }

        public void RemoveAt(int index)
        {
            _listOfItems.RemoveAt(index);
        }

        public void RemoveById(short id)
        {
            _listOfItems.RemoveAt(_listOfItems.FindIndex(a => a.id == id));
        }

        public void Remove(T item, short amount = 1)
        {
            this.Add(item, (short)-amount);
        }

        public (T product, short id, short amount) this[int index]
        {
            get
            {
                return _listOfItems[index];
            }
            set
            {
                _listOfItems[index] = ((T, short, short))value;
            }
        }

        public void Insert(int index, T item, short id, short amount = 1)
        {
            _listOfItems.Insert(index, (item, id, amount));
        }

        public void ShowShortInfo()
        {
            for (int i = 0; i < this.Count; i++)
            {
                _listOfItems[i].item.ShowShortInfo();
            }
        }

        public void ShowInfo()
        {
            for (int i = 0; i < this.Count; i++)
            {
                Console.WriteLine(GetItemInfo(i));
            }
        }

        public string GetItemInfo(int index)
        {
            return _listOfItems[index].item.Name + " x" + _listOfItems[index].amount + " = " + String.Format("${0:0.00}", _listOfItems[index].item.Cost * _listOfItems[index].amount);
        }

        public void ShowSummary(string space = "")
        {
            float summary = 0;
            for (int i = 0; i < this.Count; i++)
            {
                Console.WriteLine(space + GetItemInfo(i));
                summary += _listOfItems[i].item.Cost * _listOfItems[i].amount;
            }
            Console.WriteLine($"{space}---------------");
            Console.WriteLine($"{space}Total: {summary:$0.00}");
        }

        public bool MoveNext()
        {
            _position++;
            if (_position >= _listOfItems.Count)
            {
                _position = 0;
                return false;
            }

            return true;
        }

        public void Reset()
        {
            _position = 0;
        }

        public void Dispose()
        {
            _position = 0;
        }

        public IEnumerator GetEnumerator()
        {
            return _listOfItems.GetEnumerator();
        }
    }
}