using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SBWordListMaker;

internal static class WordDictionary
{
    public static List<Word> PerfectDic { get; private set; } = new();
    public static List<string> NoTypeWords { get; internal set; } = new();
    public static List<Word> TypedWords { get; internal set; } = new();
    public static async Task InitAsync()
    {
        foreach (var i in new[] 
        {
             "no-type-words-extension-main","no-type-words", "no-type-words-extension-common", "no-type-words-extension-proper",
            "no-type-words-extension-quantity", "no-type-words-extension-sahen-conn"
        }) await ReadCsvStringAsync(i);
        foreach (var i in SBUtils.KanaList.SelectMany(x => x).ToArray()) await ReadCsvWordAsync(i);
        await Task.Run(InitPerfectDic);
    }
    public static void InitPerfectDic()
    {
        var result = new Dictionary<string, (WordType Type1, WordType Type2)>();
        foreach (var i in NoTypeWords) result.TryAdd(i, (WordType.Empty, WordType.Empty));
        foreach (var i in TypedWords) result[i.Name] = (i.Type1, i.Type2);
        PerfectDic = result.ToList().Select(x => new Word(x.Key, x.Value.Type1, x.Value.Type2)).ToList();
    }
    static async Task ReadCsvStringAsync(string filename)
    {
        var fileUri = new Uri($"/dic/no type/{filename}.csv", UriKind.Relative);
        var info = Application.GetResourceStream(fileUri);
        using var reader = new StreamReader(info.Stream);
        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync();
            if (!string.IsNullOrWhiteSpace(line)) 
            {
                var word = line.Trim();
                NoTypeWords.Add(word);
            }
        }
    }
    static async Task ReadCsvWordAsync(string keyLetter)
    {
        var index = KeyLetterToIndexLetter(keyLetter);
        var fileUri = new Uri($"/dic/typed/{index}行/typed-words-{keyLetter}.csv", UriKind.Relative);
        var info = Application.GetResourceStream(fileUri);
        using var reader = new StreamReader(info.Stream);
        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync();
            if (string.IsNullOrWhiteSpace(line)) continue;
            var formattedLine = line?.Trim().Split() ?? Array.Empty<string>();
            var name = formattedLine.At(0) ?? string.Empty;
            var type1 = formattedLine.At(1)?.StringToType() ?? WordType.Empty;
            var type2 = formattedLine.At(2)?.StringToType() ?? WordType.Empty;
            var word = new Word(name, type1, type2);
            TypedWords.Add(word);
        }
    }
    static string KeyLetterToIndexLetter(string keyLetter)
    {
        foreach (var i in SBUtils.KanaList) if (i.Contains(keyLetter)) return i[0];
        throw new ArgumentException($"引数 {keyLetter} はひらがなではありません");
    }
}

