using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SBWordListMaker.Views;


public partial class UNOSearchPage : Page
{
    readonly bool isBeforeInit = true;
    public UNOSearchPage()
    {
        InitializeComponent();
        isBeforeInit = false;
    }

    private void BtnBack_Click(object sender, RoutedEventArgs e) => Navigator.Navigate(nameof(OtherContentsPage));

    private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (isBeforeInit || TxtWordInput.Text == "単語を入力...") return;
        var textBox = (TextBox)sender;
        var text = textBox.Text;
        if (string.IsNullOrWhiteSpace(text))
        {
            LblIndicator.Text = string.Empty;
            ListResult.Visibility = Visibility.Hidden;
            return;
        }
        textBox.FontSize = text.Length < 12 ? 40 : 40 * (double)12 / text.Length;
        var formattedText = text.Length > 20 ? $"{text[..5]}...{text[^5..]}" : text;
        LblIndicator.Text = string.Empty;
        if (text[^1] == 'ん')
        {
            LblIndicator.Text += "んで終わっています。";
            ListResult.Visibility = Visibility.Hidden;
            return;
        }
        if (!WordDictionary.PerfectDic.Select(x => x.Name).ToList().Contains(text))
        {
            LblIndicator.Text += "辞書にない単語です。";
            ListResult.Visibility = Visibility.Hidden;
            return;
        }
        var (type1, type2) = (WordType.Empty, WordType.Empty);
        foreach (var i in WordDictionary.PerfectDic)
        {
            if (i.Name == text)
            {
                type1 = i.Type1;
                type2 = i.Type2;
                break;
            }
        }
        LblIndicator.Text += (type1, type2) switch
        {
            (WordType.Empty, WordType.Empty) => "タイプ：無属性",
            (var type, WordType.Empty) => $"タイプ：{type.TypeToString()}",
            (WordType.Empty, var type) => $"タイプ：{type.TypeToString()}",
            (var typeA, var typeB) => $"タイプ：{typeA.TypeToString()} / {typeB.TypeToString()}"
        };
        LblIndicator.Text += text.Length switch
        {
            < 1 => string.Empty,
            1 => "\n文字数：１文字",
            2 => "\n文字数：２文字",
            3 => "\n文字数：３文字",
            4 => "\n文字数：４文字",
            5 => "\n文字数：５文字",
            6 => "\n文字数：６文字",
            > 6 => "\n文字数：７文字以上"
        };
        if (type1 == WordType.Empty)
        {
            ListResult.Visibility = Visibility.Hidden;
            return;
        }
        var resultList = Searcher.SearchTyped(text.GetLastChar(), x => x.Contains(type1, type2) && x.Name != text).Select(x => x.ToString()).ToList();
        if (resultList.Count == 0)
        {
            ListResult.ItemsSource = new[]
            {
                new ListViewItem
                {
                    Content = "単語が見つかりませんでした",
                    FontSize = 20,
                    Foreground = new SolidColorBrush(Colors.DarkRed)
                }
            };
            ListResult.Visibility = Visibility.Visible;
            return;
        }
        resultList.Sort();
        ListResult.ItemsSource = resultList.Select(x => StringToListViewItem(x)).ToArray();
        ListResult.Visibility = Visibility.Visible;
    }
    static ListViewItem StringToListViewItem(string str) => new()
    {
        Content = str,
        FontSize = str.Length < 24 ? 18.3 : 18.3 * 23 / str.Length,
        Foreground = new SolidColorBrush(Colors.Black)
    };

    private void TxtWordInput_GotFocus(object sender, RoutedEventArgs e)
    {
        if (TxtWordInput.Text != "単語を入力...") return;
        TxtWordInput.Foreground = new SolidColorBrush(Colors.Black);
        TxtWordInput.Text = string.Empty;
    }

    private void TxtWordInput_LostFocus(object sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(TxtWordInput.Text)) return;
        TxtWordInput.Foreground = new SolidColorBrush(Colors.DarkGray);
        TxtWordInput.Text = "単語を入力...";
    }
}
