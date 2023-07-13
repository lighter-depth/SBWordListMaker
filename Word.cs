using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBWordListMaker;

internal record struct Word(string Name, WordType Type1, WordType Type2)
{
    public readonly bool Contains(WordType type) => Type1 == type || Type2 == type;
    public readonly bool IsEmpty => Type1 == WordType.Empty;
    public readonly bool IsSingleType => Type1 != WordType.Empty && Type2 == WordType.Empty;
    public readonly bool IsDoubleType => Type1 != WordType.Empty && Type2 != WordType.Empty;
    public static List<string> WordTypeNames { get; private set; } = new() { "無効", "指定なし", "無属性" };
    public override readonly string ToString()
    {
        var type1 = Type1.TypeToString();
        var type2 = Type2.TypeToString();
        return IsEmpty ? Name
             : IsSingleType ? $"{Name} ({type1})"
             : $"{Name} ({type1}/{type2})";
    }
    public readonly string ToFormat(int formatType, WordType omitType)
    {
        var word = this with { };
        if(omitType != WordType.Empty)
        {
            var otherType = Type1 == omitType ? Type2 : Type1;
            word = word with { Type1 = otherType, Type2 = WordType.Empty };
        }
        var type1 = word.Type1 != WordType.Empty ? word.Type1.TypeToString() : string.Empty;
        var type2 = word.Type2 != WordType.Empty ? word.Type2.TypeToString() : string.Empty;
        if (formatType == 2) return word.ToString();
        return word.Name + toSpace(word.Type1) + type1 + toSpace(word.Type2) + type2;
        static string toSpace(WordType type) => type != WordType.Empty ? " " : string.Empty;
    }
    static Word()
    {
        for(var i = 1; i < 26; i++)
        {
            WordTypeNames.Add(((WordType)i).TypeToString());
        }
    }
    public static explicit operator Word(string str) => new(str, WordType.Empty, WordType.Empty);
}
internal enum WordType
{
    Empty, Normal, Animal, Plant, Place, Emote, Art, Food, Violence, Health, Body, Mech, Science, Time, Person, Work, Cloth, Society, Play, Bug, Math, Insult, Religion, Sports, Weather, Tale
}
internal static class WordTypeEx
{
    public static WordType StringToType(this string symbol)
    {
        return symbol switch
        {
            "ノーマル" => WordType.Normal,
            "動物" => WordType.Animal,
            "植物" => WordType.Plant,
            "地名" => WordType.Place,
            "感情" => WordType.Emote,
            "芸術" => WordType.Art,
            "食べ物" => WordType.Food,
            "暴力" => WordType.Violence,
            "医療" => WordType.Health,
            "人体" => WordType.Body,
            "機械" => WordType.Mech,
            "理科" => WordType.Science,
            "時間" => WordType.Time,
            "人物" => WordType.Person,
            "工作" => WordType.Work,
            "服飾" => WordType.Cloth,
            "社会" => WordType.Society,
            "遊び" => WordType.Play,
            "虫" => WordType.Bug,
            "数学" => WordType.Math,
            "暴言" => WordType.Insult,
            "宗教" => WordType.Religion,
            "スポーツ" => WordType.Sports,
            "天気" => WordType.Weather,
            "物語" => WordType.Tale,
            _ => WordType.Empty
        };
    }
    public static string TypeToString(this WordType type)
    {
        return type switch
        {
            WordType.Empty => string.Empty,
            WordType.Normal => "ノーマル",
            WordType.Animal => "動物",
            WordType.Plant => "植物",
            WordType.Place => "地名",
            WordType.Emote => "感情",
            WordType.Art => "芸術",
            WordType.Food => "食べ物",
            WordType.Violence => "暴力",
            WordType.Health => "医療",
            WordType.Body => "人体",
            WordType.Mech => "機械",
            WordType.Science => "理科",
            WordType.Time => "時間",
            WordType.Person => "人物",
            WordType.Work => "工作",
            WordType.Cloth => "服飾",
            WordType.Society => "社会",
            WordType.Play => "遊び",
            WordType.Bug => "虫",
            WordType.Math => "数学",
            WordType.Insult => "暴言",
            WordType.Religion => "宗教",
            WordType.Sports => "スポーツ",
            WordType.Weather => "天気",
            WordType.Tale => "物語",
            _ => "天で話にならねぇよ..."
        };
    }
}
