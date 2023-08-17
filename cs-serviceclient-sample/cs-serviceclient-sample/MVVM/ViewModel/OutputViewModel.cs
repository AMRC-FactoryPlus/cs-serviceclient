using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
        
        public OutputViewModel()
        { 
            OutputItems = new ObservableCollection<OutputItem>();

            var testitem = new OutputItem("testtopic", "testcontent", DateTime.Now);
            AddItem(testitem);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="outputItem"></param>
        public void AddItem(OutputItem outputItem)
        {
            OutputItems.Add(outputItem);
        }

    }
}

