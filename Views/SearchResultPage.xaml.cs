using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SBWordListMaker.Views;

public partial class SearchResultPage : Page
{
    internal static Searcher Searcher { get; set; } = new();
    internal static string SearchCondition { get; set; } = string.Empty;
    List<Word> searchResult = new();
    string[][] pages = Array.Empty<string[]>();
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
        searchResult.Sort();
        if (searchResult.Count == 0)
        {
            LblResultIndicator.Content = "単語が見つかりませんでした。";
            LblResultIndicator.Foreground = new SolidColorBrush(Colors.DarkRed);
            return;
        }
        LblResultIndicator.Visibility = Visibility.Hidden;
        GridSearchResult.Visibility = Visibility.Visible;
        pages = searchResult.Select(x => x.ToString()).Chunk(20).ToArray();
        LblTotalCounter.Content = $"全{searchResult.Count}件";
        if(pages.Length == 1) BtnNext.Visibility = Visibility.Hidden;
        ShowPage();
    }
    static ListViewItem StringToListViewItem(string str)
    {
        return new ListViewItem()
        {
            Content = str,
            FontSize = str.Length < 24 ? 18.3 : 18.3 * 23 / str.Length,
        };
    }

    private void BtnBack_Click(object sender, RoutedEventArgs e) => Navigator.Navigate(nameof(WordSearchPage));

    private void ShowPage()
    {
        ListResultLeft.ItemsSource = new List<ListViewItem>(pages[pageIndex].Take(10).Select(x => StringToListViewItem(x)));
        ListResultRight.ItemsSource = new List<ListViewItem>(pages[pageIndex].Skip(10).Take(10).Select(x => StringToListViewItem(x)));
        LblPageCounter.Content = $"{pageIndex + 1} / {pages.Length}";
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
        if(pageIndex == pages.Length - 1) BtnNext.Visibility = Visibility.Hidden;
        ShowPage();
    }
}
