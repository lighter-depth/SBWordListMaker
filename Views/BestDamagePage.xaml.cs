using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SBWordListMaker.Views;


public partial class BestDamagePage : Page
{
    readonly bool isBeforeInit = true;
    public BestDamagePage()
    {
        InitializeComponent();
        PkrAbilityEffect.SelectedIndex = 0;
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
            hideLabels();
            return;
        }
        textBox.FontSize = text.Length < 12 ? 40 : 40 * (double)12 / text.Length;
        var formattedText = text.Length > 20 ? $"{text[..5]}...{text[^5..]}" : text;
        LblIndicator.Text = string.Empty;
        if (text[^1] == 'ん')
        {
            LblIndicator.Text += "んで終わっています。";
            hideLabels();
            return;
        }
        if (!WordDictionary.PerfectDic.Select(x => x.Name).ToList().Contains(text))
        {
            LblIndicator.Text += "辞書にない単語です。";
            hideLabels();
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
        var word = new Word(text, type1, type2);
        var list = WordDictionary.GetSplitList(text.GetLastChar());
        var randomLow = word.IsEmpty ? 1 : .85;
        var randomHigh = word.IsEmpty ? 1 : .99;
        var resultList = new List<AttackInfo>();
        var noAbilityList = new List<AttackInfo>();
        foreach (var i in list)
        {
            var lowDmgRaw = (int)(10 * i.CalcAmp(word) * randomLow);
            var highDmgRaw = (int)(10 * i.CalcAmp(word) * randomHigh);
            resultList.Add(new(i, WordType.Empty, highDmgRaw, lowDmgRaw));
            noAbilityList.Add(new(i, WordType.Empty, highDmgRaw, lowDmgRaw));
            if (i.Contains(WordType.Place))
            {
                var (lowDmg, highDmg) = ((int)(lowDmgRaw * 1.5), (int)(highDmgRaw * 1.5));
                resultList.Add(new(i, WordType.Place, highDmg, lowDmg));
            }
            if (i.Contains(WordType.Body))
            {
                var (lowDmg, highDmg) = ((int)(lowDmgRaw * 1.5), (int)(highDmgRaw * 1.5));
                resultList.Add(new(i, WordType.Body, highDmg, lowDmg));
            }
            if (i.Contains(WordType.Science))
            {
                var (lowDmg, highDmg) = ((int)(lowDmgRaw * 1.5), (int)(highDmgRaw * 1.5));
                resultList.Add(new(i, WordType.Science, highDmg, lowDmg));
            }
            if (i.Contains(WordType.Person))
            {
                var (lowDmg, highDmg) = ((int)(lowDmgRaw * 1.5), (int)(highDmgRaw * 1.5));
                resultList.Add(new(i, WordType.Person, highDmg, lowDmg));
            }
            if (i.Contains(WordType.Insult))
            {
                var (lowDmg, highDmg) = ((int)(lowDmgRaw * 1.5), (int)(highDmgRaw * 1.5));
                resultList.Add(new(i, WordType.Insult, highDmg, lowDmg));
            }
            if (i.Contains(WordType.Religion))
            {
                var (lowDmg, highDmg) = ((int)(lowDmgRaw * 1.5), (int)(highDmgRaw * 1.5));
                resultList.Add(new(i, WordType.Religion, highDmg, lowDmg));
            }
            if (i.Name.Length == 6)
            {
                var (lowDmg, highDmg) = ((int)(lowDmgRaw * 1.5), (int)(highDmgRaw * 1.5));
                resultList.Add(new(i, WordType.Tale, highDmg, lowDmg));
            }
            if (i.Name.Length > 6)
            {
                var (lowDmg, highDmg) = (lowDmgRaw * 2, highDmgRaw * 2);
                resultList.Add(new(i, WordType.Tale, highDmg, lowDmg));
            }
        }
        resultList.Sort();
        noAbilityList.Sort();
        LblResult.Text = string.Join("\n", resultList.Take(5).Select(x => x.ToString()).ToArray());
        LblNoAbility.Text = string.Join("\n", noAbilityList.Take(5).Select(x => x.ToString()).ToArray());
        if (PkrAbilityEffect.SelectedIndex == 0) LblResult.Visibility = Visibility.Visible;
        if (PkrAbilityEffect.SelectedIndex == 1) LblNoAbility.Visibility = Visibility.Visible;
        void hideLabels()
        {
            LblResult.Visibility = LblNoAbility.Visibility = Visibility.Hidden;
            LblResult.Text = LblNoAbility.Text = string.Empty;
        }
    }

    private void PkrAbilityEffect_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (isBeforeInit) return;
        if(PkrAbilityEffect.SelectedIndex == 0)
        {
            LblResult.Visibility = Visibility.Visible;
            LblNoAbility.Visibility = Visibility.Hidden;
        }
        if (PkrAbilityEffect.SelectedIndex == 1)
        {
            LblResult.Visibility = Visibility.Hidden;
            LblNoAbility.Visibility = Visibility.Visible;
        }
    }

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

