using System.Windows;
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
        
        /// <summary>
        /// Communicator for doing factory plus functions
        /// </summary>
        public FPlusCommunicator FPlusCommunicator { get; set; }

        /// <summary>
        /// Constructor for making an instance of the settings view
        /// </summary>
        public SettingsViewModel()
        {
            SaveCommand = new RelayCommand(o =>
            {
                // TODO: Attempt to load saved variables
                // TODO: Make F plus communicator
            });
        }
    }
}
