using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using utility_sample.Core;
using utility_sample.MVVM.Model;

namespace utility_sample.MVVM.ViewModel
{
    /// <summary>
    /// 
    /// </summary>
    public class OutputViewModel : ObservableObject
    {
        public ObservableCollection<OutputItem> OutputItems { get; set; }
        // public event PropertyChangedEventHandler PropertyChanged;

        public OutputViewModel()
        {
            OutputItems = new ObservableCollection<OutputItem>();

            var testitem = new OutputItem("testtopic", "testcontent", DateTime.Now);
            AddItem(testitem);

            // OutputItems.CollectionChanged += collectionchanged;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="outputItem"></param>
        public void AddItem(OutputItem outputItem)
        {
            OutputItems.Add(outputItem);
            
            Debug.WriteLine(outputItem.Topic);
            
            // Force the UI to update - doesn't work
            // PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(OutputItems)));
            // Application.Current.Dispatcher.Invoke(CommandManager.InvalidateRequerySuggested);
        }
        
        // public void collectionchanged(object obj, NotifyCollectionChangedEventArgs args)
        // {
        //     Debug.WriteLine(obj);
        // }
    }
}

