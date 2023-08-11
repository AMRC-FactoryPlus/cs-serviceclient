using System;
using System.Windows;
using System.Windows.Controls;
using utility_sample.MVVM.ViewModel;

namespace utility_sample.MVVM.View;

public partial class HomeView : UserControl
{
    public HomeView()
    {
        InitializeComponent();
    }
    
    private void PasswordChangedHandler(Object sender, RoutedEventArgs args)
    {
        if (this.DataContext != null)
        {
            ((HomeViewModel)this.DataContext).ServicePassword = ((PasswordBox)sender).Password;
        }
    }
}

