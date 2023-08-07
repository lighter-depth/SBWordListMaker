using System.Windows;
using System.Windows.Controls;

namespace SBWordListMaker.Views;


public partial class OtherContentsPage : Page
{
    public OtherContentsPage() => InitializeComponent();

    private void BtnBack_Click(object sender, RoutedEventArgs e) => Navigator.Navigate(nameof(StartPage));

    private void BtnTypeCheck_Click(object sender, RoutedEventArgs e) => Navigator.Navigate(nameof(WordCheckPage));

    private void BtnUNOSearch_Click(object sender, RoutedEventArgs e) => Navigator.Navigate(nameof(UNOSearchPage));

    private void BtnBestDamage_Click(object sender, RoutedEventArgs e) => Navigator.Navigate(nameof(BestDamagePage));

    private void BtnCalcDamage_Click(object sender, RoutedEventArgs e) => Navigator.Navigate(nameof(CalcDamagePage));
}
