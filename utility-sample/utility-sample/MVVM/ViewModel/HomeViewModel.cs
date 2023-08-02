using System;
using System.Collections.Generic;
using System.Windows;
using AMRC.FactoryPlus;
using AMRC.FactoryPlus.ServiceClient;
using utility_sample.Core;

namespace utility_sample.MVVM.ViewModel
{
    public class HomeViewModel: ObservableObject
    {
        public RelayCommand SubmitCommand { get; set; }

        private ServiceClient _serviceClient;

        public HomeViewModel()
        {
            _serviceClient = new ServiceClient();
            SubmitCommand = new RelayCommand(o =>
            {
                // Do thing?
                DoAsync();
            });
        }

        private async void DoAsync()
        {
            var test = await _serviceClient.ConfigDb.Search(UUIDs.App[AppSubcomponents.Info], new Dictionary<string, object>(), new Dictionary<string, string>(), null);
                
            MessageBox.Show(test.Length.ToString());
        }
    }
}
