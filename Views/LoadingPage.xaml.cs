using SBWordListMaker.Views;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace SBWordListMaker
{
    /// <summary>
    /// LoadingPage.xaml の相互作用ロジック
    /// </summary>
    public partial class LoadingPage : Page
    {
        public LoadingPage()
        {
            InitializeComponent();
            Init();
        }
        public async void Init()
        {
            await WordDictionary.InitAsync();
            Navigator.Navigate(nameof(StartPage));
        }
    }
}
