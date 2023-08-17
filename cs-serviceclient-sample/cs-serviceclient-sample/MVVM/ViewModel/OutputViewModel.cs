using System.Collections.Generic;
using utility_sample.Core;
using utility_sample.MVVM.Model;

namespace utility_sample.MVVM.ViewModel
{
    /// <summary>
    /// 
    /// </summary>
    public class OutputViewModel : ObservableObject
    {
        public List<OutputItem> OutputItems { get; set; }
        
        public OutputViewModel()
        {
        }

        private void AddItem(OutputItem outputItem)
        {
            OutputItems.Add(outputItem);
        }
    }
}

