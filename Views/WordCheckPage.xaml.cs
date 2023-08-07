using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SBWordListMaker.Views;


public partial class WordCheckPage : Page
{
    readonly bool isBeforeInit = true;
    public WordCheckPage()
    {
        InitializeComponent();
        isBeforeInit = false;
    }

    private void BtnBack_Click(object sender, RoutedEventArgs e) => Navigator.Navigate(nameof(OtherContentsPage));

    private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (isBeforeInit) return;
        var textBox = (TextBox)sender;
        var text = textBox.Text;
        if (string.IsNullOrWhiteSpace(text))
        {
            LblResult.Text = string.Empty;
            return;
        }
        textBox.FontSize = text.Length < 12 ? 55 : 55 * (double)12 / text.Length;
        var formattedText = text.Length > 20 ? $"{text[..5]}...{text[^5..]}" : text;
        LblResult.Text = $"単語「{formattedText}」は\n";
        if (text[^1] == 'ん')
        {
            LblResult.Text += "んで終わっています。";
            return;
        }
        if (!WordDictionary.PerfectDic.Select(x => x.Name).ToList().Contains(text))
        {
            LblResult.Text += "辞書にない単語です。";
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
        LblResult.Text += (type1, type2) switch
        {
            (WordType.Empty, WordType.Empty) => "無属性の単語です。",
            (var type, WordType.Empty) => $"{type.TypeToString()}タイプです。",
            (WordType.Empty, var type) => $"{type.TypeToString()}タイプです。",
            (var typeA, var typeB) => $"{typeA.TypeToString()} / {typeB.TypeToString()} タイプです。"
        };
    }
}
