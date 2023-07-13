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

namespace SBWordListMaker.Views;

/// <summary>
/// SearchResultPage.xaml の相互作用ロジック
/// </summary>
public partial class SearchResultPage : Page
{
    internal static Searcher Searcher { get; set; } = new();
    internal static string SearchCondition { get; set; } = string.Empty;
    List<Word> searchResult = new();
    readonly List<List<string>> pages = new();
    int pageIndex = 0;
    public SearchResultPage()
    {
        InitializeComponent();
        LblSearchCondition.Content = SearchCondition;
        ShowResult();
    }
    public async void ShowResult()
    {
        await Task.Run(() => { searchResult = Searcher.Search(); });
        if (searchResult.Count == 0)
        {
            LblResultIndicator.Content = "単語が見つかりませんでした。";
            LblResultIndicator.Foreground = new SolidColorBrush(Colors.DarkRed);
            return;
        }
        LblResultIndicator.Visibility = Visibility.Hidden;
        GridSearchResult.Visibility = Visibility.Visible;
        SplitToPages();
        LblTotalCounter.Content = $"全{searchResult.Count - 1}件";
        if(pages.Count == 1) BtnNext.Visibility = Visibility.Hidden;
        ShowPage();
    }
    private void SplitToPages()
    {
        var count = 0;
        var page = new List<string>();
        foreach (var word in searchResult)
        {
            count++;
            page.Add(word.ToString());
            if (count % 20 == 0)
            {
                pages.Add(new(page));
                page = new();
            }
            if (count == searchResult.Count && count % 20 != 0) pages.Add(new(page));
        }
    }
    static ListViewItem StringToListViewItem(string str)
    {
        return new ListViewItem()
        {
            Content = str,
            FontSize = str.Length < 24 ? 18.3 : 18.3 * 23 / str.Length
        };
    }

    private void BtnBack_Click(object sender, RoutedEventArgs e)
    {
        Navigator.Navigate(nameof(WordSearchPage));
    }
    private void ShowPage()
    {
        ListResultLeft.ItemsSource = new List<ListViewItem>(pages[pageIndex].Take(10).Select(x => StringToListViewItem(x)));
        ListResultRight.ItemsSource = new List<ListViewItem>(pages[pageIndex].Skip(10).Take(10).Select(x => StringToListViewItem(x)));
        LblPageCounter.Content = $"{pageIndex + 1} / {pages.Count}";
    }

    private void BtnPrevious_Click(object sender, RoutedEventArgs e)
    {
        pageIndex--;
        BtnNext.Visibility = Visibility.Visible;
        if (pageIndex == 0) BtnPrevious.Visibility = Visibility.Hidden;
        ShowPage();
    }

    private void BtnNext_Click(object sender, RoutedEventArgs e)
    {
        pageIndex++;
        BtnPrevious.Visibility = Visibility.Visible;
        if(pageIndex == pages.Count - 1) BtnNext.Visibility = Visibility.Hidden;
        ShowPage();
    }
}
