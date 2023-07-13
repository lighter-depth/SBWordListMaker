using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
/// WordSearchPage.xaml の相互作用ロジック
/// </summary>
public partial class WordSearchPage : Page
{
    readonly bool isBeforeInit = true;
    static WordType? Type1 = null;
    static WordType? Type2 = null;
    static Regex Body = new(string.Empty);
    static bool IsTypedOnly = false;
    static bool IsSingleTypedOnly = false;
    static bool IsDoubleTypedOnly = false;
    static readonly Regex KanaRegex = new("^[ぁ-ゟ]*$");
    static int TypeConditionIndex = 0;
    static int SearchIndex = 0;
    public WordSearchPage()
    {
        InitializeComponent();
        isBeforeInit = false;
        PkrTypeCondition.SelectedIndex = TypeConditionIndex;
        PkrSearchMode.SelectedIndex = SearchIndex;
    }

    private void PkrTypeCondition_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (isBeforeInit) return;
        var index = PkrTypeCondition.SelectedIndex;
        TypeConditionIndex = index;
        PkrType1.ItemsSource = PkrType2.ItemsSource = Word.WordTypeNames;
        if (index == 0)
        {
            PkrType1.IsEnabled = PkrType2.IsEnabled = false;
            PkrType1.SelectedIndex = PkrType2.SelectedIndex = 0;
        }
        if (index == 1)
        {
            PkrType1.SelectedIndex = PkrType2.SelectedIndex = 2;
            PkrType1.IsEnabled = PkrType2.IsEnabled = false;
        }
        if (index == 2)
        {
            PkrType1.IsEnabled = PkrType2.IsEnabled = true;
            PkrType1.ItemsSource = Word.WordTypeNames.Where(x => Word.WordTypeNames.IndexOf(x) is not 0 and not 2).ToArray();
            PkrType2.ItemsSource = Word.WordTypeNames.Where(x => Word.WordTypeNames.IndexOf(x) is not 0).ToArray();
            PkrType1.SelectedIndex = PkrType2.SelectedIndex = 0;
        }
        if (index == 3)
        {
            PkrType1.IsEnabled = true;
            PkrType2.IsEnabled = false;
            PkrType1.ItemsSource = Word.WordTypeNames.Where(x => Word.WordTypeNames.IndexOf(x) is not 0 and not 2).ToArray();
            PkrType1.SelectedIndex = 0;
            PkrType2.SelectedIndex = 0;
        }
        if (index == 4)
        {
            PkrType1.IsEnabled = PkrType2.IsEnabled = true;
            PkrType1.ItemsSource = PkrType2.ItemsSource = Word.WordTypeNames.Where(x => Word.WordTypeNames.IndexOf(x) is not 0 and not 2).ToArray();
            PkrType1.SelectedIndex = PkrType2.SelectedIndex = 0;
        }
    }
    private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        var txt = (TextBox)sender;
        if (txt.Text.Length < 12) return;
        txt.FontSize = (double)11 / txt.Text.Length * 40;
    }

    private void PkrSearchMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (isBeforeInit) return;
        var index = PkrSearchMode.SelectedIndex;
        SearchIndex = index;
        var grids = new List<Grid> { ZenpouKouhou, Zenpou, Kouhou, Bubun, Kanzen, RegexSearch };
        grids.ForEach(grid => grid.Visibility = Visibility.Hidden);
        if (grids.ElementAtOrDefault(index) != null) grids[index].Visibility = Visibility.Visible;
    }

    private void PkrType1_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (isBeforeInit) return;
        Type1 = (PkrType1.SelectedItem as string ?? string.Empty).StringToType();
        if (Type1 != WordType.Empty) return;
        if (PkrType1.SelectedItem as string is "無効" or "指定なし") Type1 = null;
    }

    private void PkrType2_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (isBeforeInit) return;
        Type2 = (PkrType2.SelectedItem as string ?? string.Empty).StringToType();
        if (Type2 != WordType.Empty) return;
        if (PkrType2.SelectedItem as string is "無効" or "指定なし") Type2 = null;
        if (PkrTypeCondition.SelectedIndex == 3) Type2 = WordType.Empty;
    }

    private void BtnBack_Click(object sender, RoutedEventArgs e)
    {
        Navigator.Navigate(nameof(StartPage));
    }
    private (WordType? Type1, WordType? Type2) ConvertTypes()
    {
        var type1 = (PkrType1.SelectedItem as string ?? string.Empty).StringToType() as WordType?;
        if (PkrType1.SelectedItem as string is "無効" or "指定なし" && type1 == WordType.Empty) type1 = null;
        var type2 = (PkrType2.SelectedItem as string ?? string.Empty).StringToType() as WordType?;
        if (PkrType2.SelectedItem as string is "無効" or "指定なし" && type2 == WordType.Empty) type2 = null;
        if (PkrTypeCondition.SelectedIndex == 3 && type2 is null) type2 = WordType.Empty;
        return (type1, type2);
    }

    private List<TextBox> GetTextBoxes()
    {
        return PkrSearchMode.SelectedIndex switch
        {
            0 => new() { TxtZenpouKouhouFirst, TxtZenpouKouhouLast },
            1 => new() { TxtZenpou },
            2 => new() { TxtKouhou },
            3 => new() { TxtBubun },
            4 => new() { TxtKanzen },
            5 => new() { TxtRegex },
            _ => new()
        };
    }

    private void BtnSearch_Click(object sender, RoutedEventArgs e)
    {
        var txt = GetTextBoxes();
        var contents = txt.Select(x => x.Text).ToArray();
        if(contents.Where(x => x.Length < 1).Any())
        {
            LblWarn.Content = "⚠条件が入力されていません";
            LblWarn.Foreground = new SolidColorBrush(Colors.IndianRed);
            return;
        }
        var index = PkrSearchMode.SelectedIndex;
        if(index < 5 && contents.Where(x => !KanaRegex.IsMatch(x)).Any())
        {
            LblWarn.Content = "⚠ひらがなを入力してください";
            LblWarn.Foreground = new SolidColorBrush(Colors.IndianRed);
            return;
        }
        if(index == 5 && contents.Where(x => !SBUtils.IsValidRegex(x)).Any())
        {
            LblWarn.Content = "⚠正規表現の書式が不正です。";
            LblWarn.Foreground = new SolidColorBrush(Colors.IndianRed);
            return;
        }

        var first = contents.ElementAtOrDefault(0)?.Trim() ?? string.Empty;
        var last = contents.ElementAtOrDefault(1)?.Trim() ?? string.Empty;
        Body = new(index switch
        {
            0 => $@"^{first}.*{last}ー*$",
            1 => $@"^{first}.*[^ん]$",
            2 => $@"^.*{first}ー*$",
            3 => $@"^.*{first}.*[^ん]$",
            4 => $@"^{first}$",
            6 => $@".*",
            _ => $@"{first}"
        });
        IsTypedOnly = PkrTypeCondition.SelectedIndex == 2;
        IsSingleTypedOnly = PkrTypeCondition.SelectedIndex == 3;
        IsDoubleTypedOnly = PkrTypeCondition.SelectedIndex == 4;
        (Type1, Type2) = ConvertTypes();
        SearchResultPage.Searcher = new() 
        { 
            Type1 = Type1,
            Type2 = Type2,
            Body = Body,
            IsTypedOnly = IsTypedOnly,
            IsSingleTypedOnly = IsSingleTypedOnly,
            IsDoubleTypedOnly = IsDoubleTypedOnly
        };
        SearchResultPage.SearchCondition = GetSearchCondition();
        Navigator.Navigate(nameof(SearchResultPage));
    }
    private string GetSearchCondition()
    {
        var sb = new StringBuilder();
        sb.Append("検索条件：");
        var contents = GetTextBoxes().Select(x => x.Text).ToList();
        var first = contents.FirstOrDefault() ?? string.Empty;
        var last = contents.LastOrDefault() ?? string.Empty;
        var searchType = PkrSearchMode.SelectedIndex switch
        {
            0 => $"前方後方一致 [{first}, {last}]",
            1 => $"前方一致 [{first}]",
            2 => $"後方一致 [{first}]",
            3 => $"部分一致 [{first}]",
            4 => $"完全一致 [{first}]",
            5 => $"正規表現 [{first}]",
            _ => $"すべて検索"
        };
        sb.Append(searchType);
        var typeParam1 = PkrType1.SelectedItem as string;
        var typeParam2 = PkrType2.SelectedItem as string;
        var typeCondition = PkrTypeCondition.SelectedIndex switch
        {
            0 => string.Empty,
            1 => "(無属性)",
            2 => $"({typeParam1}/{typeParam2})",
            3 => $"({typeParam1} <単タイプ>)",
            4 when new[] {typeParam1, typeParam2}.Contains("指定なし") => $"({typeParam1}/{typeParam2} <複合タイプ>)",
            4 => $"({typeParam1}/{typeParam2})",
            _ => string.Empty
        };
        sb.Append(" " + typeCondition);
        return sb.ToString();
    }
}
