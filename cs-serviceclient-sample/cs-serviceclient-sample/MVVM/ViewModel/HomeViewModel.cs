using System.Windows;
using utility_sample.Core;
using utility_sample.MVVM.Model;

namespace utility_sample.MVVM.ViewModel
{
    /// <summary>
    /// Handles viewmodel for the default home view
    /// </summary>
    public class HomeViewModel: ObservableObject
    {
        public OutputViewModel OutputViewModel { get; set; }
        public RelayCommand SubmitCommand { get; set; }
        
        public string ServiceUsername { get; set; }
        public string ServicePassword { get; set; }
        public string Topic { get; set; }

        public RelayCommand CancelCommand { get; set; }

        private FPlusCommunicator _fPlusCommunicator;
        private OutputViewModel _outputViewModel;
        
        /// <summary>
        /// Constructor for making an instance of the settings view model
        /// </summary>
        public HomeViewModel()
        {
            OutputViewModel = new OutputViewModel();
            
            // Set default value
            Topic = "spBv1.0/#";
            
            _fPlusCommunicator = FPlusCommunicator.GetInstance();
            
            SubmitCommand = new RelayCommand(o =>
            {
                if (ServiceUsername != null && ServicePassword != null && Topic != null)
                {
                    _fPlusCommunicator.StartFPlusStuff(ServiceUsername, ServicePassword, Topic);
                }
                else
                {
                    MessageBox.Show("Please enter a username, password, and topic");
                }
            });
            
            CancelCommand = new RelayCommand(o =>
            {
                _fPlusCommunicator.StopFPlusStuff(Topic);
            });
        }
    }
}
