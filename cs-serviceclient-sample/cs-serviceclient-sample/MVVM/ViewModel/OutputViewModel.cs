using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using utility_sample.Core;
using utility_sample.MVVM.Model;

namespace utility_sample.MVVM.ViewModel
{
    /// <summary>
    /// Handles display of incoming messages
    /// </summary>
    public class OutputViewModel : ObservableObject
    {
        public ObservableCollection<OutputItem> OutputItems
        {
            get { return _outputItems; }
            set
            {
                _outputItems = value;
                OnPropertyChanged(nameof(OutputItems));
            }
        }

        private ObservableCollection<OutputItem> _outputItems;
        private bool _isDestroyed = false;
        private Queue<OutputItem> _incoming = new Queue<OutputItem>();
        
        /// <summary>
        /// Constructor for OutputViewModel
        /// </summary>
        public OutputViewModel()
        {
            OutputItems = new ObservableCollection<OutputItem>();

            var testitem = new OutputItem("testtopic", "testcontent", DateTime.Now);
            AddItem(testitem);

            AddStuff();
        }

        ~OutputViewModel()
        {
            _isDestroyed = true;
        }

        /// <summary>
        /// Thread safe way to add messages to the UI
        /// </summary>
        private async void AddStuff()
        {
            while (!_isDestroyed)
            {
                while (_incoming.Count > 0)
                {
                    if (_incoming.TryDequeue(out var result))
                    {
                        OutputItems.Add(result);

                        if (OutputItems.Count > 8)
                        {
                            OutputItems.RemoveAt(0);
                        }
                    }
                }
                await Task.Delay(100);
            }
        }

        /// <summary>
        /// Adds an item to be shown in the display window
        /// </summary>
        /// <param name="outputItem">Item to display</param>
        public void AddItem(OutputItem outputItem)
        {
            _incoming.Enqueue(outputItem);
            
            Debug.WriteLine(outputItem.Topic);
        }
    }
}

