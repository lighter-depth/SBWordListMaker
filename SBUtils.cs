﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SBWordListMaker;

internal static class SBUtils
{
    public static string Version { get; set; } = "v0.2.0";
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
        result.AddRange(words.Where(x => x.Name.Length >= 12));
        if (arg == 1) return result;
        result.AddRange(words.Where(x => x.Name.Length == 6).Select(x => x with { Name = $"({x.Name})" }));
        result.AddRange(words.Where(x => x.Name.Length < 6).Select(x => x with { Name = $"({x.Name})" }));
        return result;
    }
    public static T? At<T>(this IEnumerable<T> source, int index) => source.ElementAtOrDefault(index);
    public static T? At<T>(this IEnumerable<T> source, Index index) => source.ElementAtOrDefault(index);

    public static char GetLastChar(this string str)
    {
        return (str.Length == 0
        || str.Length == 1 && str.At(0) == 'ー') ? '\0'
        : siritoriChar.ContainsKey(str.At(^1)) ? siritoriChar[str.At(^1)]
        : str.At(^1) == 'ー' && siritoriChar.ContainsKey(str.At(^2)) ? siritoriChar[str.At(^2)]
        : str.At(^1) == 'ー' ? str.At(^2)
        : str.At(^1);
    }
    static readonly Dictionary<char, char> siritoriChar = new()
    {
        ['ゃ'] = 'や',
        ['ゅ'] = 'ゆ',
        ['ょ'] = 'よ',
        ['っ'] = 'つ',
        ['ぁ'] = 'あ',
        ['ぃ'] = 'い',
        ['ぅ'] = 'う',
        ['ぇ'] = 'え',
        ['ぉ'] = 'お',
        ['を'] = 'お',
        ['ぢ'] = 'じ',
        ['づ'] = 'ず'
    };
}
