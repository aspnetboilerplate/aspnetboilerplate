//-----------------------------------------------------------------------
// <copyright file="ObservableDictionary.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace NJsonSchema.Collections
{
    /// <summary>An implementation of an observable dictionary. </summary>
    /// <typeparam name="TKey">The type of the key. </typeparam>
    /// <typeparam name="TValue">The type of the value. </typeparam>
    internal class ObservableDictionary<TKey, TValue> :
        IDictionary<TKey, TValue>, INotifyCollectionChanged,
        INotifyPropertyChanged, IDictionary
#if !LEGACY
, IReadOnlyDictionary<TKey, TValue>
#endif
    {
        private IDictionary<TKey, TValue> _dictionary;

        /// <summary>Initializes a new instance of the <see cref="ObservableDictionary{TKey, TValue}"/> class. </summary>
        public ObservableDictionary()
        {
            _dictionary = new Dictionary<TKey, TValue>();
        }

        /// <summary>Initializes a new instance of the <see cref="ObservableDictionary{TKey, TValue}"/> class. </summary>
        /// <param name="dictionary">The dictionary to initialize this dictionary. </param>
        public ObservableDictionary(IDictionary<TKey, TValue> dictionary)
        {
            _dictionary = new Dictionary<TKey, TValue>(dictionary);
        }

        /// <summary>Initializes a new instance of the <see cref="ObservableDictionary{TKey, TValue}"/> class. </summary>
        /// <param name="comparer">The comparer. </param>
        public ObservableDictionary(IEqualityComparer<TKey> comparer)
        {
            _dictionary = new Dictionary<TKey, TValue>(comparer);
        }

        /// <summary>Initializes a new instance of the <see cref="ObservableDictionary{TKey, TValue}"/> class. </summary>
        /// <param name="capacity">The capacity. </param>
        public ObservableDictionary(int capacity)
        {
            _dictionary = new Dictionary<TKey, TValue>(capacity);
        }

        /// <summary>Initializes a new instance of the <see cref="ObservableDictionary{TKey, TValue}"/> class. </summary>
        /// <param name="dictionary">The dictionary to initialize this dictionary. </param>
        /// <param name="comparer">The comparer. </param>
        public ObservableDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
        {
            _dictionary = new Dictionary<TKey, TValue>(dictionary, comparer);
        }

        /// <summary>Initializes a new instance of the <see cref="ObservableDictionary{TKey, TValue}"/> class. </summary>
        /// <param name="capacity">The capacity. </param>
        /// <param name="comparer">The comparer. </param>
        public ObservableDictionary(int capacity, IEqualityComparer<TKey> comparer)
        {
            _dictionary = new Dictionary<TKey, TValue>(capacity, comparer);
        }

        /// <summary>Gets the underlying dictonary. </summary>
        protected IDictionary<TKey, TValue> Dictionary
        {
            get { return _dictionary; }
        }

        /// <summary>Adds multiple key-value pairs the the dictionary. </summary>
        /// <param name="items">The key-value pairs. </param>
        public void AddRange(IDictionary<TKey, TValue> items)
        {
            if (items == null)
                throw new ArgumentNullException("items");

            if (items.Count > 0)
            {
                if (Dictionary.Count > 0)
                {
                    if (items.Keys.Any(k => Dictionary.ContainsKey(k)))
                        throw new ArgumentException("An item with the same key has already been added.");

                    foreach (var item in items)
                        Dictionary.Add(item);
                }
                else
                    _dictionary = new Dictionary<TKey, TValue>(items);

                OnCollectionChanged(NotifyCollectionChangedAction.Add, items.ToArray());
            }
        }

        /// <summary>Inserts a key-value pair into the dictionary. </summary>
        /// <param name="key">The key. </param>
        /// <param name="value">The value. </param>
        /// <param name="add">If true and key already exists then an exception is thrown. </param>
        protected virtual void Insert(TKey key, TValue value, bool add)
        {
            TValue item;
            if (Dictionary.TryGetValue(key, out item))
            {
                if (add)
                    throw new ArgumentException("An item with the same key has already been added.");

                if (Equals(item, value))
                    return;

                Dictionary[key] = value;
                OnCollectionChanged(NotifyCollectionChangedAction.Replace, new KeyValuePair<TKey, TValue>(key, value), new KeyValuePair<TKey, TValue>(key, item));
            }
            else
            {
                Dictionary[key] = value;
                OnCollectionChanged(NotifyCollectionChangedAction.Add, new KeyValuePair<TKey, TValue>(key, value));
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var copy = PropertyChanged;
            if (copy != null)
                copy(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void OnCollectionChanged()
        {
            OnPropertyChanged();
            var copy = CollectionChanged;
            if (copy != null)
                copy(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        protected void OnCollectionChanged(NotifyCollectionChangedAction action, KeyValuePair<TKey, TValue> changedItem)
        {
            OnPropertyChanged();
            var copy = CollectionChanged;
            if (copy != null)
                copy(this, new NotifyCollectionChangedEventArgs(action, changedItem, 0));
        }

        protected void OnCollectionChanged(NotifyCollectionChangedAction action, KeyValuePair<TKey, TValue> newItem, KeyValuePair<TKey, TValue> oldItem)
        {
            OnPropertyChanged();
            var copy = CollectionChanged;
            if (copy != null)
                copy(this, new NotifyCollectionChangedEventArgs(action, newItem, oldItem, 0));
        }

        protected void OnCollectionChanged(NotifyCollectionChangedAction action, IList newItems)
        {
            OnPropertyChanged();
            var copy = CollectionChanged;
            if (copy != null)
                copy(this, new NotifyCollectionChangedEventArgs(action, newItems, 0));
        }

        private void OnPropertyChanged()
        {
            OnPropertyChanged("Count");
            OnPropertyChanged("Item[]");
            OnPropertyChanged("Keys");
            OnPropertyChanged("Values");
        }

        #region IDictionary<TKey,TValue> interface

        public void Add(TKey key, TValue value)
        {
            Insert(key, value, false);
        }

        public bool ContainsKey(TKey key)
        {
            return Dictionary.ContainsKey(key);
        }

        public ICollection<TKey> Keys
        {
            get { return Dictionary.Keys; }
        }

        ICollection IDictionary.Values { get { return ((IDictionary)Dictionary).Values; } }

        ICollection IDictionary.Keys { get { return ((IDictionary)Dictionary).Keys; } }

#if !LEGACY

        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values
        {
            get { return Values; }
        }

#endif

        public virtual bool Remove(TKey key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            TValue value;
            Dictionary.TryGetValue(key, out value);

            var removed = Dictionary.Remove(key);
            if (removed)
                OnCollectionChanged();
            //OnCollectionChanged(NotifyCollectionChangedAction.Remove, new KeyValuePair<TKey, TValue>(key, value));
            return removed;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return Dictionary.TryGetValue(key, out value);
        }

#if !LEGACY

        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys
        {
            get { return Keys; }
        }

#endif

        public ICollection<TValue> Values
        {
            get { return Dictionary.Values; }
        }

        public TValue this[TKey key]
        {
            get { return Dictionary[key]; }
            set { Insert(key, value, false); }
        }

        #endregion

        #region ICollection<KeyValuePair<TKey,TValue>> interface

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Insert(item.Key, item.Value, true);
        }

        void IDictionary.Add(object key, object value)
        {
            Insert((TKey)key, (TValue)value, true);
        }

        public void Clear()
        {
            if (Dictionary.Count > 0)
            {
                Dictionary.Clear();
                OnCollectionChanged();
            }
        }

        public void Initialize(IEnumerable<KeyValuePair<TKey, TValue>> keyValuePairs)
        {
            var pairs = keyValuePairs.ToList();
            foreach (var pair in pairs)
                Dictionary[pair.Key] = pair.Value;

            foreach (var key in Dictionary.Keys.Where(k => !pairs.Any(p => Equals(p.Key, k))).ToArray())
                Dictionary.Remove(key);

            OnCollectionChanged();
        }

        public void Initialize(IEnumerable keyValuePairs)
        {
            Initialize(keyValuePairs.Cast<KeyValuePair<TKey, TValue>>());
        }

        public bool Contains(object key)
        {
            return ContainsKey((TKey)key);
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return ((IDictionary)Dictionary).GetEnumerator();
        }

        public void Remove(object key)
        {
            Remove((TKey)key);
        }

        public bool IsFixedSize { get { return false; } }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return Dictionary.Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            Dictionary.CopyTo(array, arrayIndex);
        }

        public void CopyTo(Array array, int index)
        {
            ((IDictionary)Dictionary).CopyTo(array, index);
        }

        public int Count
        {
            get { return Dictionary.Count; }
        }

        public bool IsSynchronized { get; private set; }
        public object SyncRoot { get; private set; }

        public bool IsReadOnly
        {
            get { return Dictionary.IsReadOnly; }
        }

        object IDictionary.this[object key]
        {
            get { return this[(TKey)key]; }
            set { this[(TKey)key] = (TValue)value; }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return Remove(item.Key);
        }

        #endregion

        #region IEnumerable<KeyValuePair<TKey,TValue>> interface

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return Dictionary.GetEnumerator();
        }

        #endregion

        #region IEnumerable interface

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)Dictionary).GetEnumerator();
        }

        #endregion

        #region INotifyCollectionChanged interface

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        #endregion

        #region INotifyPropertyChanged interface

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
