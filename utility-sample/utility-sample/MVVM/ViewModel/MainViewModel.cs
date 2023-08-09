using System;
using utility_sample.Core;
using utility_sample.MVVM.Model;

namespace utility_sample.MVVM.ViewModel
{
    /// <summary>
    /// Handles viewmodel for the main, overall view
    /// </summary>
    public class MainViewModel : ObservableObject
    {
        /// <summary>
        /// Command to go to home view
        /// </summary>
        public RelayCommand HomeViewCommand { get; set; }
        /// <summary>
        /// Command to go to settings view
        /// </summary>
        public RelayCommand SettingsViewCommand { get; set; }
        /// <summary>
        /// The home view viewmodel
        /// </summary>
        public HomeViewModel HomeVM { get; set; }
        /// <summary>
        /// The settings view viewmodel
        /// </summary>
        public SettingsViewModel SettingsVM { get; set; }
        
        /// <summary>
        /// Factory Plus communicator instance to be passed around to other views
        /// </summary>
        public FPlusCommunicator FPlusCommunicator { get; set; }

        private object _currentView;

        /// <summary>
        /// The currently set view
        /// </summary>
        public object CurrentView
        {
            get { return _currentView; }
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// Constructor for making an instance of the main view model
        /// </summary>
        public MainViewModel()
        {
            FPlusCommunicator = new FPlusCommunicator();
            
            HomeVM = new HomeViewModel();
            SettingsVM = new SettingsViewModel();
            
            CurrentView = HomeVM;

            HomeViewCommand = new RelayCommand(o =>
            {
                CurrentView = HomeVM;
            });
            
            SettingsViewCommand = new RelayCommand(o =>
            {
                CurrentView = SettingsVM;
            });
        }
    }
}
