using System;
using System.Collections.Generic;
using System.Windows;
using AMRC.FactoryPlus;
using AMRC.FactoryPlus.ServiceClient;
using utility_sample.Core;
using utility_sample.MVVM.Model;

namespace utility_sample.MVVM.ViewModel
{
    /// <summary>
    /// Handles viewmodel for the default home view
    /// </summary>
    public class HomeViewModel: ObservableObject
    {
        public RelayCommand SubmitCommand { get; set; }
        
        public string ServiceUsername { get; set; }
        public string ServicePassword { get; set; }

        private FPlusCommunicator _fPlusCommunicator;
        
        /// <summary>
        /// Constructor for making an instance of the settings view model
        /// </summary>
        public HomeViewModel()
        {
            _fPlusCommunicator = FPlusCommunicator.GetInstance();
            
            SubmitCommand = new RelayCommand(o =>
            {
                if (ServiceUsername != null && ServicePassword != null)
                {
                    _fPlusCommunicator.DoTest(ServiceUsername, ServicePassword);
                }
            });
        }
    }
}
