using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.UI.Xaml;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CollectionChangedReplace
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public readonly TestCollection testCollection;
        public readonly ObservableCollection<string> observableCollection;

        private int count = 0;

        public MainWindow()
        {
            this.InitializeComponent();
            testCollection = new();
            observableCollection = new() { "hello", "world" };
        }

        private void ReplaceButton_Click(object sender, RoutedEventArgs e)
        {
            // testCollection[0] = NewText();
            // testCollection[1] = NewText();
            testCollection.AddByReplace(NewText());

            observableCollection[0] = NewText();
            observableCollection[1] = NewText();
        }

        private void RemoveAddButton_Click(object sender, RoutedEventArgs e)
        {
            testCollection.RemoveAt(1);
            testCollection.RemoveAt(0);
            testCollection.Add(NewText());
            testCollection.Add(NewText());

            observableCollection.RemoveAt(1);
            observableCollection.RemoveAt(0);
            observableCollection.Add(NewText());
            observableCollection.Add(NewText());
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            count = 0;

            testCollection.Clear();
            testCollection.Add(NewText());
            testCollection.Add(NewText());

            observableCollection.Clear();
            observableCollection.Add(NewText());
            observableCollection.Add(NewText());
        }

        private string NewText() => $"New Text {count++}";
    }

    public sealed class TestCollection : IList, IList<string>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        private readonly List<string> _list = new() { "hello", "world" };

        public IEnumerator<string> GetEnumerator() => _list.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void AddByReplace(string newItem)
        {
            _list.Add(newItem);

            NotifyIndexer();
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Replace,
                _list.ToArray(),
                _list.GetRange(0, _list.Count - 1),
                0));
        }

        public void Add(string item)
        {
            _list.Add(item);

            NotifyIndexer();
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Add,
                item,
                _list.Count - 1));
        }

        public int Add(object value)
        {
            Add((string)value);
            return _list.Count - 1;
        }

        public void Clear()
        {
            _list.Clear();

            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Reset));
        }

        public bool Contains(object value) => Contains((string)value);

        public int IndexOf(object value) => IndexOf((string)value);

        public void Insert(int index, object value) => Insert(index, (string)value);

        public void Remove(object value) => Remove((string)value);

        public bool Contains(string item) => _list.Contains(item);

        public void CopyTo(string[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);

        public bool Remove(string item)
        {
            int index = _list.IndexOf(item);
            if (index < 0)
                return false;

            _list.Remove(item);

            NotifyIndexer();
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Remove,
                item,
                index));
            return true;
        }

        public void CopyTo(Array array, int index) => CopyTo((string[])array, index);

        public int Count => _list.Count;
        public bool IsSynchronized => false;
        public object SyncRoot { get; }
        public bool IsReadOnly => false;
        object IList.this[int index]
        {
            get => this[index];
            set => this[index] = (string)value;
        }

        public int IndexOf(string item) => _list.IndexOf(item);

        public void Insert(int index, string item) => _list.Insert(index, item);

        public void RemoveAt(int index)
        {
            var old = _list[index];
            _list.RemoveAt(index);

            NotifyIndexer();
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Remove,
                old,
                index));
        }

        public bool IsFixedSize => false;

        public string this[int index]
        {
            get => _list[index];
            set
            {
                var old = _list[index];
                _list[index] = value;

                NotifyIndexer();
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Replace,
                    value,
                    old,
                    index));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyIndexer()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Item[]"));
        }

    }
}
