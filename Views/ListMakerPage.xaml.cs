using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
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
/// ListMakerPage.xaml の相互作用ロジック
/// </summary>
public partial class ListMakerPage : Page
{
    static string DirName = @"C:\Users";
    static int ListTypeIndex = 0;
    static int WordLengthIndex = 0;
    static int WordCountIndex = 0;
    static int KeyTypeIndex = 0;
    static int FormatIndex = 0;
    static readonly Regex KanaRegex = new("^[ぁ-ゟ]$");
    static readonly Regex InvalidRegex = new("^[ぁぃぅぇぉぢっづゃゅょを]$");
    static readonly Regex SUSRegex = new("^[あいうえおじずつやゆよ]$");
    static readonly Regex NumberRegex = new(@"^\d+$");
    static readonly Dictionary<string, string> KanaShrinker = new()
    {
        ["あ"] = "ぁ",
        ["い"] = "ぃ",
        ["う"] = "ぅ",
        ["え"] = "ぇ",
        ["お"] = "ぉ",
        ["じ"] = "ぢ",
        ["ず"] = "づ",
        ["つ"] = "っ",
        ["や"] = "ゃ",
        ["ゆ"] = "ゅ",
        ["よ"] = "ょ"
    };
    public ListMakerPage()
    {
        InitializeComponent();
        PkrListType.SelectedIndex = ListTypeIndex;
        PkrWordLength.SelectedIndex = WordLengthIndex;
        PkrWordCount.SelectedIndex = WordCountIndex;
        PkrKeyType.SelectedIndex = KeyTypeIndex;
        PkrFormat.SelectedIndex = FormatIndex;
    }

    private void BtnBack_Click(object sender, RoutedEventArgs e)
    {
        Navigator.Navigate(nameof(StartPage));
    }

    private void BtnFileSave_Click(object sender, RoutedEventArgs e)
    {
        if (PkrListType.SelectedIndex is 0 or 1 && TxtKeyChar.Text.Length == 0
         || PkrListType.SelectedIndex is 0 or 1 && PkrWordCount.SelectedIndex == 0 && TxtWordCount.Text.Length == 0)
        {
            LblWarn.Content = "⚠条件が入力されていません";
            LblWarn.Foreground = new SolidColorBrush(Colors.IndianRed);
            return;
        }
        if (PkrListType.SelectedIndex is 0 or 1 && !KanaRegex.IsMatch(TxtKeyChar.Text))
        {
            LblWarn.Content = "⚠ひらがなを入力してください";
            LblWarn.Foreground = new SolidColorBrush(Colors.IndianRed);
            return;
        }
        if (PkrListType.SelectedIndex is 0 or 1 && InvalidRegex.IsMatch(TxtKeyChar.Text))
        {
            LblWarn.Content = "⚠無効なひらがなです";
            LblWarn.Foreground = new SolidColorBrush(Colors.IndianRed);
            return;
        }
        if (PkrListType.SelectedIndex is 0 or 1 && PkrWordCount.SelectedIndex == 0 && !NumberRegex.IsMatch(TxtWordCount.Text))
        {
            LblWarn.Content = "⚠数値を入力してください";
            LblWarn.Foreground = new SolidColorBrush(Colors.IndianRed);
            return;
        }
        LblWarn.Content = "単語帳を生成中. . . ";
        LblWarn.Foreground = new SolidColorBrush(Color.FromRgb(50, 50, 50));
        var type = PkrListType.SelectedIndex == 2 ? ((ComboBoxItem)PkrKeyType.SelectedItem).Content.ToString()?.StringToType() : WordType.Empty;
        var body = new Regex(PkrListType.SelectedIndex switch
        {
            0 => $"^.*{TxtKeyChar.Text}ー*$",
            1 => $"^{TxtKeyChar.Text}.*[^ん]$",
            _ => ".*"
        });
        if (PkrListType.SelectedIndex == 0 && SUSRegex.IsMatch(TxtKeyChar.Text))
        {
            var txt = TxtKeyChar.Text;
            body = new Regex($"^.*{txt}ー*$|^.*{KanaShrinker[txt]}ー*$");
        }
        var searcher = new Searcher()
        {
            Type1 = type,
            Type2 = type != WordType.Empty ? null : WordType.Empty,
            IsTypedOnly = type is not WordType.Empty,
            Body = body
        };
        var result = searcher.Search();
        var wordCount = TxtWordCount.Text;
        var list = FormatResult(result, type ?? WordType.Empty, wordCount);
        var fileExt = PkrFormat.SelectedIndex == 3 ? "csv" : "txt";
        SaveFile(list, fileExt);
        LblWarn.Content = "条件を入力してください. . . ";
    }
    private static string FormatResult(List<Word> words, WordType type, string wordCount)
    {
        var sb = new StringBuilder();
        if(FormatIndex != 3) sb.Append("/*\r\n * このリストは、機械的に生成されたものです。\r\n * 実際のゲーム内容とは差異がある可能性があります。\r\n */\r\n\r\n");
        var sortArg = ListTypeIndex switch
        {
            2 => 2,
            _ => WordLengthIndex
        };
        foreach (var i in SBUtils.KanaList)
        {
            sb.Append(toHeader(i[0]));
            for (var j = 0; j < i.Length; j++)
            {
                var key = i[j];
                var format = new Regex(ListTypeIndex switch
                {
                    1 => $"^.*{key}ー*$",
                    _ => $"^{key}.*$"
                });
                var filtered = words.Where(x => format.IsMatch(x.Name)).SortByLength(sortArg).Select(x => x.ToFormat(FormatIndex, type)).ToList();
                var takeCount = filtered.Count;
                if (WordCountIndex == 0
                && ListTypeIndex != 2
                && int.TryParse(wordCount, out var tmp)) takeCount = tmp;
                if (filtered.Count != 0) sb.Append(BuildRow(filtered.Take(takeCount).ToList()));
            }
        }
        return sb.ToString();
        static string toHeader(string keyStr)
        {
            return FormatIndex switch
            {
                2 => $"【{keyStr}行】\r\n",
                3 => string.Empty,
                _ => $"**{keyStr}行\r\n"
            };
        }
    }
    private static string BuildRow(List<string> filtered)
    {
        var header = FormatIndex is not 3 ? "・" : string.Empty;
        var splitter = FormatIndex is not 3 ? "、" : "\r\n";
        var footer = "\r\n";
        var sb = new StringBuilder();
        sb.Append(header);
        sb.Append(filtered[0]);
        for (var i = 1; i < filtered.Count; i++)
        {
            sb.Append(splitter);
            sb.Append(filtered[i]);
        }
        sb.Append(footer);
        return sb.ToString();
    }
    private static List<string> SortByLength(List<string> list)
    {
        var result = new List<string>();
        result.AddRange(list.Where(x => x.Length == 7));
        for (var i = 8; i < 170; i++) result.AddRange(list.Where(x => x.Length == i));
        result.AddRange(list.Where(x => x.Length == 6));
        result.AddRange(list.Where(x => x.Length < 6));
        return result;
    }
    private static void SaveFile(string wordList, string fileExt)
    {
        var dialog = new SaveFileDialog
        {
            InitialDirectory = DirName,
            FileName = $"単語帳.{fileExt}",
            Filter = $"すべてのファイル|*.*|{fileExt} ファイル|*.{fileExt}",
            FilterIndex = 2
        };
        if (dialog.ShowDialog() is true)
        {
            var filename = dialog.FileName;
            DirName = System.IO.Path.GetDirectoryName(filename) ?? @"C:\Users";
            try
            {
                File.WriteAllText(filename, wordList, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
    private void PkrListType_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var index = PkrListType.SelectedIndex;
        ListTypeIndex = index;
        if (index == 0)
        {
            LblKeyChar.Content = "語尾の文字";
            LblKeyType.Visibility = Visibility.Hidden;
            PkrKeyType.Visibility = Visibility.Hidden;
            LblKeyChar.Visibility = Visibility.Visible;
            TxtKeyChar.Visibility = Visibility.Visible;
            LblWordLength.Visibility = Visibility.Visible;
            PkrWordLength.Visibility = Visibility.Visible;
            LblWordCountTitle.Visibility = Visibility.Visible;
            PkrWordCount.Visibility = Visibility.Visible;
            TxtWordCount.Visibility = Visibility.Visible;
            LblWordCount.Visibility = Visibility.Visible;
        }
        if (index == 1)
        {
            LblKeyChar.Content = "語頭の文字";
            LblKeyType.Visibility = Visibility.Hidden;
            PkrKeyType.Visibility = Visibility.Hidden;
            LblKeyChar.Visibility = Visibility.Visible;
            TxtKeyChar.Visibility = Visibility.Visible;
            LblWordLength.Visibility = Visibility.Visible;
            PkrWordLength.Visibility = Visibility.Visible;
            LblWordCountTitle.Visibility = Visibility.Visible;
            PkrWordCount.Visibility = Visibility.Visible;
            TxtWordCount.Visibility = Visibility.Visible;
            LblWordCount.Visibility = Visibility.Visible;
        }
        if (index == 2)
        {
            LblKeyChar.Visibility = Visibility.Hidden;
            TxtKeyChar.Visibility = Visibility.Hidden;
            LblKeyType.Visibility = Visibility.Visible;
            PkrKeyType.Visibility = Visibility.Visible;
            LblWordLength.Visibility = Visibility.Hidden;
            PkrWordLength.Visibility = Visibility.Hidden;
            LblWordCountTitle.Visibility = Visibility.Hidden;
            PkrWordCount.Visibility = Visibility.Hidden;
            TxtWordCount.Visibility = Visibility.Hidden;
            LblWordCount.Visibility = Visibility.Hidden;
        }
    }

    private void PkrWordLength_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var index = PkrWordLength.SelectedIndex;
        WordLengthIndex = index;
    }

    private void PkrWordCount_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var index = PkrWordCount.SelectedIndex;
        WordCountIndex = index;
        TxtWordCount.Visibility = LblWordCount.Visibility = (Visibility)index;
    }

    private void PkrKeyType_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var index = PkrKeyType.SelectedIndex;
        KeyTypeIndex = index;
    }

    private void PkrFormat_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var index = PkrFormat.SelectedIndex;
        FormatIndex = index;
    }
}
