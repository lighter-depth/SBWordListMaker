using System.Windows;
using System.Windows.Controls;

namespace SBWordListMaker.Views;


public partial class StartPage : Page
{
    public StartPage() => InitializeComponent();
    
    private void BtnWordSearch_Click(object sender, RoutedEventArgs e) => Navigator.Navigate(nameof(WordSearchPage));
    
    private void BtnListMaker_Click(object sender, RoutedEventArgs e) => Navigator.Navigate(nameof(ListMakerPage));
    
    private void BtnTypeCheck_Click(object sender, RoutedEventArgs e) => Navigator.Navigate(nameof(OtherContentsPage));
}
