using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SBWordListMaker.Views
{
    /// <summary>
    /// StartPage.xaml の相互作用ロジック
    /// </summary>
    public partial class StartPage : Page
    {
        public StartPage()
        {
            InitializeComponent();
        }

        private void BtnWordSearch_Click(object sender, RoutedEventArgs e)
        {
            Navigator.Navigate(nameof(WordSearchPage));
        }

        private void BtnListMaker_Click(object sender, RoutedEventArgs e)
        {
            Navigator.Navigate(nameof(ListMakerPage));
        }

        private void BtnTypeCheck_Click(object sender, RoutedEventArgs e)
        {
            Navigator.Navigate(nameof(WordCheckPage));
        }
    }
}
