using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Collections;

namespace Databinding
{
    public class ObservableVector : IObservableVector<object>, IEnumerable
    {
        private IList<object> _internalCollection;
        private ReadOnlyCollection<object> _readOnlyCollection;

        public ObservableVector()
        {
            _internalCollection = new List<object>();
            _readOnlyCollection = new ReadOnlyCollection<object>(_internalCollection);
        }

        public int IndexOf(object item)
        {
            return _internalCollection.IndexOf(item);
        }

        public void Insert(int index, object item)
        {
            _internalCollection.Insert(index, item);

            if (VectorChanged != null)
            {
                VectorChanged(this, new VectorChangedEventArgs { CollectionChange = CollectionChange.ItemInserted, Index = (uint)index });
            }
        }

        public void RemoveAt(int index)
        {
            _internalCollection.RemoveAt(index);

            if (VectorChanged != null)
            {
                VectorChanged(this, new VectorChangedEventArgs { CollectionChange = CollectionChange.ItemRemoved, Index = (uint)index });
            }
        }

        public object this[int index]
        {
            get
            {
                return _internalCollection[index];
            }
            set
            {
                _internalCollection[index] = value;

                if (VectorChanged != null)
                {
                    VectorChanged(this, new VectorChangedEventArgs { CollectionChange = CollectionChange.ItemChanged, Index = (uint)index });
                }
            }
        }

        public void Add(object item)
        {
            _internalCollection.Add(item);

            if (VectorChanged != null)
            {
                VectorChanged(this, new VectorChangedEventArgs { CollectionChange = CollectionChange.ItemInserted, Index = (uint)(_internalCollection.Count - 1) });
            }
        }

        public bool Contains(object item)
        {
            return _internalCollection.Contains(item);
        }

        public void CopyTo(object[] array, int arrayIndex)
        {
            _internalCollection.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _internalCollection.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(object item)
        {
            int index = _internalCollection.IndexOf(item);
            bool retVal = _internalCollection.Remove(item);

            if (VectorChanged != null)
            {
                VectorChanged(this, new VectorChangedEventArgs { CollectionChange = CollectionChange.ItemRemoved, Index = (uint)index });
            }
            return retVal;
        }

        public Windows.Foundation.Collections.IIterator<object> First()
        {
            throw new NotImplementedException();
        }

        public IEnumerator GetEnumerator()
        {
            return _internalCollection.GetEnumerator();
        }

        IEnumerator<object> IEnumerable<object>.GetEnumerator()
        {
            return _internalCollection.GetEnumerator();
        }
                
        public event VectorChangedEventHandler<object> VectorChanged;

        public void Append(object value)
        {
            this.Add(value);

            if (VectorChanged != null)
            {
                VectorChanged(this, new VectorChangedEventArgs { CollectionChange = CollectionChange.ItemInserted, Index = (uint)(_internalCollection.Count - 1) });
            }
        }

        public void Clear()
        {
            this.Clear();

            if (VectorChanged != null)
            {
                VectorChanged(this, new VectorChangedEventArgs { CollectionChange = CollectionChange.Reset, Index = 0 });
            }
        }

        public object GetAt(uint index)
        {
            return this[(int)index];
        }

        public uint GetMany(uint startIndex, object[] items)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyList<object> GetView()
        {
            return _readOnlyCollection;
        }

        public bool IndexOf(object value, out uint index)
        {
            index = (uint)this.IndexOf(value);
            return true;
        }

        public void InsertAt(uint index, object value)
        {
            this.Insert((int)index, value);

            if (VectorChanged != null)
            {
                VectorChanged(this, new VectorChangedEventArgs { CollectionChange = CollectionChange.ItemInserted, Index = index });
            }
        }

        public void RemoveAt(uint index)
        {
            this.RemoveAt((int)index);

            if(VectorChanged != null)
            {
                VectorChanged(this, new VectorChangedEventArgs { CollectionChange = CollectionChange.ItemRemoved, Index = index });
            }
        }

        public void RemoveAtEnd()
        {
            this.RemoveAt(_internalCollection.Count - 1);

            if(VectorChanged != null)
            {
                VectorChanged(this, new VectorChangedEventArgs { CollectionChange = CollectionChange.ItemRemoved, Index = (uint)(_internalCollection.Count - 1) });
            }
        }

        public void ReplaceAll(object[] items)
        {
            throw new NotImplementedException();
        }

        public void SetAt(uint index, object value)
        {
            this[(int)index] = value;

            if (VectorChanged != null)
            {
                VectorChanged(this, new VectorChangedEventArgs { CollectionChange = CollectionChange.ItemChanged, Index = index });
            }
        }

        public uint Size
        {
            get
            {
                return (uint)this.Count;
            }
        }
    }

    public class VectorChangedEventArgs : IVectorChangedEventArgs
    {
        public CollectionChange CollectionChange
        {
            get;
            set;
        }

        public uint Index
        {
            get;
            set;
        }
    }
}
