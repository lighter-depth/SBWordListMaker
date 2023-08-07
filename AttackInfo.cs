using System;

namespace SBWordListMaker;

record struct AttackInfo(Word Word, WordType Ability, int MaxDmg, int MinDmg) : IComparable<AttackInfo>
{
    public override readonly string ToString()
    {
        var ability = Ability == WordType.Empty ? string.Empty : " [" + Ability.AbilityToString() + "] ";
        return Word.ToString() + ability + $"{{{MinDmg}-{MaxDmg}}}";
    }
    public readonly int CompareTo(AttackInfo other) => -MaxDmg.CompareTo(other.MaxDmg);
}
