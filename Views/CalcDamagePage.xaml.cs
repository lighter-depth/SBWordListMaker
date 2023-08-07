using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SBWordListMaker.Views;

public partial class CalcDamagePage : Page
{
    readonly bool isBeforeInit = true;
    Word attacker = new(); // 攻めの単語
    Word receiver = new(); // 受けの単語
    double valueA = 1;
    double valueB = 1;
    WordType ability = WordType.Empty;
    bool critIfPossible = false;
    public CalcDamagePage()
    {
        InitializeComponent();
        isBeforeInit = false;
    }

    private void BtnBack_Click(object sender, RoutedEventArgs e) => Navigator.Navigate(nameof(OtherContentsPage));

    private void TxtAllyWord_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (isBeforeInit) return;
        var textBox = (TextBox)sender;
        var text = textBox.Text;
        if (string.IsNullOrWhiteSpace(text))
        {
            LblAllyIndicator.Text = string.Empty;
            return;
        }
        textBox.FontSize = text.Length < 10 ? 30 : 30 * (double)10 / text.Length;
        var formattedText = text.Length > 20 ? $"{text[..5]}...{text[^5..]}" : text;
        LblAllyIndicator.Text = string.Empty;
        if (text[^1] == 'ん')
        {
            LblAllyIndicator.Text += "んで終わっています。";
            return;
        }
        if (!WordDictionary.PerfectDic.Select(x => x.Name).ToList().Contains(text))
        {
            LblAllyIndicator.Text += "辞書にない単語です。";
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
        LblAllyIndicator.Text += (type1, type2) switch
        {
            (WordType.Empty, WordType.Empty) => "タイプ：無属性",
            (var type, WordType.Empty) => $"タイプ：{type.TypeToString()}",
            (WordType.Empty, var type) => $"タイプ：{type.TypeToString()}",
            (var typeA, var typeB) => $"タイプ：{typeA.TypeToString()} / {typeB.TypeToString()}"
        };
        attacker = new(text, type1, type2);
        UpdateLblDamage();
    }

    private void TxtFoeWord_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (isBeforeInit) return;
        var textBox = (TextBox)sender;
        var text = textBox.Text;
        if (string.IsNullOrWhiteSpace(text))
        {
            LblFoeIndicator.Text = string.Empty;
            return;
        }
        textBox.FontSize = text.Length < 10 ? 30 : 30 * (double)10 / text.Length;
        var formattedText = text.Length > 20 ? $"{text[..5]}...{text[^5..]}" : text;
        LblFoeIndicator.Text = string.Empty;
        if (text[^1] == 'ん')
        {
            LblFoeIndicator.Text += "んで終わっています。";
            return;
        }
        if (!WordDictionary.PerfectDic.Select(x => x.Name).ToList().Contains(text))
        {
            LblFoeIndicator.Text += "辞書にない単語です。";
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
        LblFoeIndicator.Text += (type1, type2) switch
        {
            (WordType.Empty, WordType.Empty) => "タイプ：無属性",
            (var type, WordType.Empty) => $"タイプ：{type.TypeToString()}",
            (WordType.Empty, var type) => $"タイプ：{type.TypeToString()}",
            (var typeA, var typeB) => $"タイプ：{typeA.TypeToString()} / {typeB.TypeToString()}"
        };
        receiver = new(text, type1, type2);
        UpdateLblDamage();
    }
    private void UpdateLblDamage()
    {
        if (string.IsNullOrWhiteSpace(attacker.Name) || string.IsNullOrWhiteSpace(receiver.Name))
        {
            LblDamage.Content = string.Empty;
            return;
        }
        var resultStr = string.Empty;
        var baseDmg = attacker.IsEmpty && ability == WordType.Normal ? 13
                    : attacker.IsEmpty ? 7 
                    : 10;
        var randomLow = attacker.IsEmpty || receiver.IsEmpty ? 1 : 0.85;
        var randomHigh = attacker.IsEmpty || receiver.IsEmpty ? 1 : 0.99;
        var critFlag = (attacker.Contains(WordType.Body) && (critIfPossible || ability == WordType.Body))
                    || (attacker.Contains(WordType.Insult) && (critIfPossible || ability == WordType.Insult));
        var crit = critFlag ? 1.5 : 1;
        var atk = critFlag ? Math.Max(1, valueA) : valueA;
        var def = critFlag ? Math.Min(1, valueB) : valueB;
        var prop = attacker.CalcAmp(receiver);
        var amp = attacker.Contains(WordType.Place) && ability == WordType.Place ? 1.5
                : attacker.Contains(WordType.Science) && ability == WordType.Mech ? 1.5
                : attacker.Contains(WordType.Person) && ability == WordType.Person ? 1.5
                : attacker.Contains(WordType.Religion) && ability == WordType.Religion ? 1.5
                : attacker.Name.Length == 6 && ability == WordType.Tale ? 1.5
                : attacker.Name.Length > 6 && ability == WordType.Tale ? 2
                : 1;
        var dmgLow = (int)(crit * (int)(amp * (int)(baseDmg * prop * atk / def * randomLow)));
        var dmgHigh = (int)(crit * (int)(amp * (int)(baseDmg * prop * atk / def * randomHigh)));
        resultStr += $"{attacker} → {receiver}：";
        resultStr += $" [{dmgLow}-{dmgHigh}]";
        LblDamage.FontSize = resultStr.Length < 30 ? 30 : 30 * (double)30 / resultStr.Length;
        LblDamage.Content = resultStr;
    }

    private void PkrValueA_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (isBeforeInit) return;
        var text = ((PkrValueA.SelectedItem as ComboBoxItem)?.Content as string) ?? string.Empty;
        valueA = double.TryParse(text.Split().At(0), out var atkTmp) ? atkTmp : 1;
        UpdateLblDamage();
    }

    private void PkrValueB_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (isBeforeInit) return;
        var text = ((PkrValueB.SelectedItem as ComboBoxItem)?.Content as string) ?? string.Empty;
        valueB = double.TryParse(text.Split().At(0), out var defTmp) ? defTmp : 1;
        UpdateLblDamage();
    }

    private void PkrAbility_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (isBeforeInit) return;
        ability = PkrAbility.SelectedIndex switch
        {
            1 => WordType.Tale,
            2 => WordType.Normal,
            3 => WordType.Place,
            4 => WordType.Science,
            5 => WordType.Person,
            6 => WordType.Religion,
            7 => WordType.Body,
            8 => WordType.Insult,
            _ => WordType.Empty
        };
        UpdateLblDamage();
    }

    private void PkrPossiblyCrit_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if(isBeforeInit) return;
        critIfPossible = PkrPossiblyCrit.SelectedIndex == 1;
        UpdateLblDamage();
    }
}
