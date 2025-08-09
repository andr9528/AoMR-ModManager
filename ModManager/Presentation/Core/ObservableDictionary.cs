using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;

namespace ModManager.Presentation.Core;

// Created mostly by ChatGPT.
public class ObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, INotifyCollectionChanged,
    INotifyPropertyChanged
{
    private readonly Dictionary<TKey, TValue> _dictionary = new();

    public event NotifyCollectionChangedEventHandler CollectionChanged;
    public event PropertyChangedEventHandler PropertyChanged;

    private void RaisePropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void RaiseCollectionChanged(NotifyCollectionChangedAction action, object changedItem = null)
    {
        CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(action, changedItem));
    }

    private void RaiseCollectionReset()
    {
        CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }

    #region IDictionary<TKey, TValue> Members

    public void Add(TKey key, TValue value)
    {
        _dictionary.Add(key, value);
        RaiseCollectionChanged(NotifyCollectionChangedAction.Add, new KeyValuePair<TKey, TValue>(key, value));
        RaisePropertyChanged(nameof(Count));
        RaisePropertyChanged("Item[]");
        RaisePropertyChanged(nameof(Keys));
        RaisePropertyChanged(nameof(Values));
    }

    public bool Remove(TKey key)
    {
        if (_dictionary.TryGetValue(key, out TValue? value) && _dictionary.Remove(key))
        {
            RaiseCollectionChanged(NotifyCollectionChangedAction.Remove, new KeyValuePair<TKey, TValue>(key, value));
            RaisePropertyChanged(nameof(Count));
            RaisePropertyChanged("Item[]");
            RaisePropertyChanged(nameof(Keys));
            RaisePropertyChanged(nameof(Values));
            return true;
        }

        return false;
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        return _dictionary.TryGetValue(key, out value);
    }

    public TValue this[TKey key]
    {
        get => _dictionary[key];
        set
        {
            bool exists = _dictionary.ContainsKey(key);
            _dictionary[key] = value;

            if (exists)
            {
                RaiseCollectionChanged(NotifyCollectionChangedAction.Replace,
                    new KeyValuePair<TKey, TValue>(key, value));
            }
            else
            {
                RaiseCollectionChanged(NotifyCollectionChangedAction.Add, new KeyValuePair<TKey, TValue>(key, value));
                RaisePropertyChanged(nameof(Count));
                RaisePropertyChanged(nameof(Keys));
                RaisePropertyChanged(nameof(Values));
            }

            RaisePropertyChanged("Item[]");
        }
    }

    public ICollection<TKey> Keys => _dictionary.Keys;
    public ICollection<TValue> Values => _dictionary.Values;
    public int Count => _dictionary.Count;
    public bool IsReadOnly => false;

    public void Add(KeyValuePair<TKey, TValue> item)
    {
        Add(item.Key, item.Value);
    }

    public void Clear()
    {
        if (_dictionary.Count > 0)
        {
            _dictionary.Clear();
            RaiseCollectionReset();
            RaisePropertyChanged(nameof(Count));
            RaisePropertyChanged("Item[]");
            RaisePropertyChanged(nameof(Keys));
            RaisePropertyChanged(nameof(Values));
        }
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        return _dictionary.Contains(item);
    }

    public bool ContainsKey(TKey key)
    {
        return _dictionary.ContainsKey(key);
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        ((IDictionary<TKey, TValue>) _dictionary).CopyTo(array, arrayIndex);
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return _dictionary.GetEnumerator();
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        return Remove(item.Key);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _dictionary.GetEnumerator();
    }

    #endregion
}
