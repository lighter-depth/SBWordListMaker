using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SBWordListMaker;

class Searcher
{
    public WordType? Type1 { get; init; } = null;
    public WordType? Type2 { get; init; } = null;
    public Regex Body { get; init; } = new(string.Empty);
    public bool IsTypedOnly { get; init; } = false;
    public bool IsSingleTypedOnly { get; init; } = false;
    public bool IsDoubleTypedOnly { get; init; } = false;
    public List<Word> Search()
    {
        var typePred = GetPredicate();
        return WordDictionary.PerfectDic.Where(typePred).Where(x => Body.IsMatch(x.Name)).ToList();
    }
    Func<Word, bool> GetPredicate()
    {
        Func<Word, bool> typePred = _ => true;
        typePred = (Type1, Type2) switch
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
        return typePred;
    }
}