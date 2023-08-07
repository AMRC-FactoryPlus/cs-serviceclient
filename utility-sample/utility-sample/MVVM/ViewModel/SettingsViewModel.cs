using System.Diagnostics;
using utility_sample.Core;
using utility_sample.MVVM.Model;

namespace utility_sample.MVVM.ViewModel
{
    /// <summary>
    /// Handles viewmodel for the settings screen
    /// </summary>
    public class SettingsViewModel: ObservableObject
    {
        /// <summary>
        /// Command to save settings
        /// </summary>
        public RelayCommand SaveCommand { get; set; }

        private FPlusCommunicator _fPlusCommunicator;
        
        /// <summary>
        /// Constructor for making an instance of the settings view
        /// </summary>
        public SettingsViewModel()
        {
            // TODO: Load saved variables if found
            _fPlusCommunicator = FPlusCommunicator.GetInstance();
            _fPlusCommunicator.TestString = "Hello from Settings View";
            Debug.Print(_fPlusCommunicator.TestString);
            
            SaveCommand = new RelayCommand(o =>
            {
                // TODO: Attempt to save variables
                _fPlusCommunicator.TestString = "Hello from Save button";
                Debug.Print(_fPlusCommunicator.TestString);
            });
        }
    }
}
