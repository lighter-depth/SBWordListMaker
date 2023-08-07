using SBWordListMaker.Views;
using System.Windows.Controls;

namespace SBWordListMaker;


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
