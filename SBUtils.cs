using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SBWordListMaker;

internal static class SBUtils
{
    public static string Version { get; set; } = "v0.1.0";
    public static string[][] KanaList => new[] 
    {
        new[]{ "あ", "い", "う", "え", "お" },
        new[]{ "か", "き", "く", "け", "こ"},
        new[]{"さ", "し", "す", "せ", "そ"},
        new[]{"た", "ち", "つ", "て", "と"},
        new[]{"な", "に", "ぬ", "ね", "の"},
        new[]{"は", "ひ", "ふ", "へ", "ほ"},
        new[]{"ま", "み", "む", "め", "も"},
        new[]{"や", "ゆ", "よ"},
        new[]{"ら", "り", "る", "れ", "ろ"},
        new[]{"わ"},
        new[]{"が", "ぎ", "ぐ", "げ", "ご"},
        new[]{"ざ", "じ", "ず", "ぜ", "ぞ"},
        new[]{"だ", "で", "ど"},
        new[]{"ば", "び", "ぶ", "べ", "ぼ"},
        new[]{"ぱ", "ぴ", "ぷ", "ぺ", "ぽ"}
    };
    public static bool IsValidRegex(string pattern)
    {
        try
        {
            var regex = new Regex(pattern);
            return true;
        }
        catch
        {
            return false;
        }
    }
    public static IEnumerable<Word> SortByLength(this IEnumerable<Word> words, int arg)
    {
        var result = new List<Word>();
        if (arg == 2) return words;
        for (var i = 7; i < 12; i++) result.AddRange(words.Where(x => x.Name.Length == i));
        result.AddRange(words.Where(x => x.Name.Length > 12));
        if (arg == 1) return result;
        result.AddRange(words.Where(x => x.Name.Length == 6).Select(x => x with { Name = $"({x.Name})"}));
        result.AddRange(words.Where(x => x.Name.Length < 6).Select(x => x with { Name = $"({x.Name})" }));
        return result;
    }
}
