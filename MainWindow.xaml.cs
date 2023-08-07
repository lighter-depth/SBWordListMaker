using System;
using System.Windows;

namespace SBWordListMaker;


public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        MainFrame.Source = new Uri("/Views/LoadingPage.xaml", UriKind.Relative);
    }
}
