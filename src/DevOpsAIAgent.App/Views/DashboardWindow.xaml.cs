using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;
using DevOpsAIAgent.App.ViewModels;

namespace DevOpsAIAgent.App.Views;

public partial class DashboardWindow : Window
{
    public DashboardWindow(DashboardViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }

    private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = e.Uri.AbsoluteUri,
                UseShellExecute = true
            });
            e.Handled = true;
        }
        catch
        {
            // Ignore errors when opening browser
        }
    }
}
