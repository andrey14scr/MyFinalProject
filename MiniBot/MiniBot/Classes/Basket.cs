using LogInfo;

using MiniBot.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniBot.Activity
{
    class Basket<T> : IEnumerator<(T product, int id, short amount)>, IEnumerable where T : IShowInfo, IProduct
    {
        private List<(T item, int id, short amount)> _listOfItems = new List<(T item, int id, short amount)>();

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

        public (T product, int id, short amount) Current => _listOfItems[_position];

        object IEnumerator.Current => _listOfItems[_position];

        public void Add(T item, int id, short amount = 1)
        {
            if (item == null)
            {
                throw new NullReferenceException("Null item reference");
            }

            int index = _listOfItems.FindIndex(x => x.id == id);

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
            if (index < 0)
            {
                Logger.Error("Index can not be nagative");
                return;
            }
            _listOfItems.RemoveAt(index);
        }

        public void RemoveById(int id)
        {
            _listOfItems.RemoveAt(_listOfItems.FindIndex(a => a.id == id));
        }

        public (T item, int id, short amount) GetById(int id)
        {
            return _listOfItems.Find(a => a.id == id);
        }

        public void Remove(T item, int id, short amount = 1)
        {
            if (item == null)
            {
                throw new NullReferenceException("Null item reference");
            }
            this.Add(item, id, (short)-amount);
        }

        public (T product, int id, short amount) this[int index]
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

        public void Insert(int index, T item, int id, short amount = 1)
        {
            if (item == null)
            {
                throw new NullReferenceException("Null item reference");
            }
            if (index < 0)
            {
                Logger.Error("Index can not be nagative");
                return;
            }

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
            if (index < 0)
            {
                Logger.Error("Index can not be nagative");
                return "";
            }

            return _listOfItems[index].item.Name + " x" + _listOfItems[index].amount + " = " + String.Format("${0:0.00}", _listOfItems[index].item.Cost * _listOfItems[index].amount);
        }

        public string GetItemInfoById(int id)
        {
            int index = _listOfItems.FindIndex(x => x.id == id);
            return GetItemInfo(index);
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
            Console.WriteLine($"{space}{Sources.Total}: {summary:$0.00}");
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