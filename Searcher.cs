﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SBWordListMaker;

class Searcher
{
    public WordType? Type1 { get; init; } = null;
    public WordType? Type2 { get; init; } = null;
    public Regex Body { get; init; } = new(".*");
    public bool IsTypedOnly { get; init; } = false;
    public bool IsSingleTypedOnly { get; init; } = false;
    public bool IsDoubleTypedOnly { get; init; } = false;
    Func<Word, bool> Predicate => (Type1, Type2) switch
    {
        (WordType.Empty, WordType.Empty) => x => x.IsEmpty,
        (not null, not null) => x => x.Contains((WordType)Type1) && x.Contains((WordType)Type2),
        (not null, null) => x => x.Contains((WordType)Type1) && (!IsSingleTypedOnly || x.IsSingleType) && (!IsDoubleTypedOnly || x.IsDoubleType),
        (null, not null) => x => x.Contains((WordType)Type2) && (!IsSingleTypedOnly || x.IsSingleType) && (!IsDoubleTypedOnly || x.IsDoubleType),
        (null, null) when IsTypedOnly => x => !x.IsEmpty,
        (null, null) when IsSingleTypedOnly => x => x.IsSingleType,
        (null, null) when IsDoubleTypedOnly => x => x.IsDoubleType,
        _ => x => true
    };
    public List<Word> Search(Func<Word, bool>? predicate = null) => WordDictionary.PerfectDic.Where(predicate ?? Predicate).Where(x => Body.IsMatch(x.Name)).ToList();
    public static List<Word> SearchTyped(char startChar, Func<Word, bool> predicate) => WordDictionary.GetSplitList(startChar).Where(predicate).ToList();
}