using utility_sample.Core;

namespace utility_sample.MVVM.ViewModel
{
    public class SettingsViewModel: ObservableObject
    {
        public RelayCommand SaveCommand { get; set; }

        public SettingsViewModel()
        {
            SaveCommand = new RelayCommand(o =>
            {
                // Do thing?
            });
        }
    }
}
