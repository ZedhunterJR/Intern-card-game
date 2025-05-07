using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Global : Singleton<Global>
{
    [SerializeField] private Sprite[] positionArrowSprites;
    [SerializeField] private string[] positionArrowHelpers;
    private Dictionary<string, Sprite> posArrowDict = new Dictionary<string, Sprite>();

    private void Awake()
    {
        for (int i = 0; i < positionArrowHelpers.Length; i++)
        {
            posArrowDict[positionArrowHelpers[i]] = positionArrowSprites[i];
        }
    }

    public Dictionary<string, ConditionClause> Conditions = new()
    {
        { "c1", new("c1", "has 1+ dice", "#ffcd00".HexColor()) },
        { "c2", new("c2", "has 2+ same value", "#ff8200".HexColor(), 2f) },
        { "c3", new("c3", "has 1+ [empty]", "#ffcd00".HexColor() , 3f) },
        { "c4", new("c4", "has [sum] 6+", "#ffcd00".HexColor(), 1.5f) },
        { "c5", new("c5", "has 2+ dice", "#ffcd00".HexColor(), 1.3f) },
        { "c6", new("c6", "has 1+ odd", "#ffcd00".HexColor(), 1.5f) },
        { "c7", new("c7", "has value 6", "#ffcd00".HexColor(), 2f) },
        { "c8", new("c8", "has [sum] 10+", "#ffcd00".HexColor(), 2.5f) },
        { "c9", new("c9", "has 3+ value > 4", "#ffcd00".HexColor(), 4f) },
        { "c10", new("c10", "has [sum] 20+", "#ffcd00".HexColor(), 6f) },
        { "c11", new("c11", "has [sequence] 2+", "#ffcd00".HexColor(), 2f) },
        { "c12", new("c12", "has [sum] = 7", "#ffcd00".HexColor() , 2.5f) },
        { "c13", new("c13", "has 2+ odd", "#ffcd00".HexColor() , 2f) },
        { "c14", new("c14", "has 2+ even", "#ffcd00".HexColor() , 1.8f) },
        { "c15", new("c15", "has 1+ frozen dice (not coded yet)", "#ffcd00".HexColor() , 5f) },
        { "c16", new("c16", "has 3+ different value", "#ffcd00".HexColor() , 5f) },
        { "c17", new("c17", "has [sum] < 7", "#ffcd00".HexColor() , 3f) },
        { "c18", new("c18", "has [hi] < 4", "#ffcd00".HexColor() , 3f) },
        { "c19", new("c19", "eachhas value 3", "#ffcd00".HexColor() , 3f) },
        { "c20", new("c18", "eachhas [empty]", "#ffcd00".HexColor() , 3f) },
    };
    public Dictionary<string, EffectClause> Effects = new()
    {
        { "e1", new("e1", "Strike", "Deal [this] damage", "#ffcd00".HexColor()) },
        { "e2", new("e2", "All out", "Deal [sum] damage", "#ffcd00".HexColor()) },
        { "e3", new("e3", "Sword", "Deal 4x[count] dice 4+", "#ffcd00".HexColor()) },
        { "e4", new("e4", "Conjure", "Place a 6 on all [empty]", "#ffcd00".HexColor()) },
        { "e5", new("e5", "Twin Strike", "Deal [this]x2 damage", "#ff0015".HexColor()) },
        { "e6", new("e6", "Smash", "Deal [hi]x2 damage", "#ff0015".HexColor()) },
        { "e7", new("e7", "Smite", "Deal 5x[count] dice 6", "#ff0015".HexColor()) },
        { "e8", new("e8", "Sacrify", "-1 dice, deal 20 damage", "#ff0015".HexColor()) },
        { "e9", new("e9", "Bludgeon", "[disable], deal 30", "#ff0015".HexColor()) },
        { "e10", new("e10", "Echo", "Deal [this] + [chrono], gain [chrono] equal to [this]", "#0001ff".HexColor()) },
        { "e11", new("e11", "Delay", "If [chrono] > 0, deal [this]x3 <br>Gain 1 [chrono]", "#0001ff".HexColor()) },
        { "e12", new("e12", "Rewind", "[trigger] left card", "#0001ff".HexColor()) },
        { "e13", new("e13", "Stop", "Deal [this], [freeze] [this]", "#0001ff".HexColor()) },
        { "e14", new("e14", "Consume", "Deal [this] + [chrono], set [chrono] to 0", "#0001ff".HexColor()) },
        { "e15", new("e15", "Roll for day", "Deal [this], +1 dice", "#80fa00".HexColor()) },
        { "e16", new("e16", "High roll", "[reroll] [this], deal [this]x4", "#80fa00".HexColor()) },
        { "e17", new("e17", "Risk", "Gain [this] as gold, -1 dice", "#80fa00".HexColor()) },
        { "e18", new("e18", "Lucky", "All [pos] become 6, deal 6x[count][pos]", "#80fa00".HexColor()) },
        { "e19", new("e19", "Spender", "Deal dmg equal to gold spent (max 50)", "#80fa00".HexColor()) },
        { "e20", new("e20", "All in", "Deal [sum]x[count] unused dice", "#80fa00".HexColor()) },
        { "e21", new("e21", "Growth", "This card permanently gain 0.1x mult", "#9e00f9".HexColor()) },
        { "e22", new("e22", "Potent", "Deal [this] + [magic], gain 1 [magic]", "#9e00f9".HexColor()) },
        { "e23", new("e23", "Accel", "Gain 2 [magic]", "#9e00f9".HexColor()) },
        { "e24", new("e24", "Arcane", "Deal [magic]x2", "#9e00f9".HexColor()) },
        { "e25", new("e25", "Cash in", "[consume], Gain gold equal to [magic]", "#9e00f9".HexColor()) },
        { "e26", new("e26", "Flow", "Deal 30, -[this] foreach used dice", "#00ffff".HexColor()) },
        { "e27", new("e27", "Thousand cut", "Deal 5, +10 foreach [pos] < 4", "#00ffff".HexColor()) },
        { "e28", new("e28", "Vault", "[disable], gain 1 play", "#00ffff".HexColor()) },
    };
    public List<List<SkillCondition>> Positions = new()
    {
        new() {SkillCondition.Left, SkillCondition.Right },
        new() {SkillCondition.Current, SkillCondition.First},
        new() {SkillCondition.First, SkillCondition.Third, SkillCondition.Fifth},
        new() {SkillCondition.Second, SkillCondition.Fourth},
        new() {SkillCondition.First, SkillCondition.Third, SkillCondition.Fifth, SkillCondition.Second, SkillCondition.Fourth},
        new() {SkillCondition.Current},
        new() {SkillCondition.Left}, new() {SkillCondition.Right},
        new() {SkillCondition.First}, new() {SkillCondition.Second}, new() {SkillCondition.Third},
        new() {SkillCondition.Fourth}, new() {SkillCondition.Fifth},
    };

    public float CalculatePosMul(List<SkillCondition> conditions)
    {
        return (conditions.Count - 1) * 0.2f + 1; 
    }

    public void UpdatePositionArrowGraphic(List<SkillCondition> positions, Image[] positionArrows)
    {
        // Reset all to blank
        foreach (var img in positionArrows)
            img.sprite = posArrowDict["blank"];

        // Track available slots
        List<int> available = new List<int>() { 0, 1, 2, 3, 4 };

        // Direct assignments
        Dictionary<SkillCondition, (string spriteKey, int index)> directArrows = new()
        {
            { SkillCondition.Left, ("left", 0) },
            { SkillCondition.Right, ("right", 4) },
            { SkillCondition.Current, ("this", 2) }
        };

        foreach (var entry in directArrows)
        {
            if (positions.Contains(entry.Key))
            {
                positionArrows[entry.Value.index].sprite = posArrowDict[entry.Value.spriteKey];
                available.Remove(entry.Value.index);
            }
        }

        // Ranked positions and preferred slots
        Dictionary<SkillCondition, (string spriteKey, int preferredIndex)> rankedPositions = new()
        {
            { SkillCondition.First, ("1st", 0) },
            { SkillCondition.Second, ("2nd", 1) },
            { SkillCondition.Third, ("3rd", 2) },
            { SkillCondition.Fourth, ("4th", 3) },
            { SkillCondition.Fifth, ("5th", 4) }
        };

        foreach (var entry in rankedPositions)
        {
            if (positions.Contains(entry.Key))
            {
                int target = entry.Value.preferredIndex;
                if (!available.Contains(target))
                    target = available.Count > 0 ? available[0] : -1;

                if (target != -1)
                {
                    positionArrows[target].sprite = posArrowDict[entry.Value.spriteKey];
                    available.Remove(target);
                }
            }
        }
    }


}


public class ConditionClause
{
    public string id;
    public string description;
    public Color color;
    public float multiplier;

    public ConditionClause(string id, string description, Color color, float multiplier = 1f)
    {
        this.id = id;
        this.description = description;
        this.color = color;
        this.multiplier = multiplier;
    }
}
public class EffectClause
{
    public string id;
    public string name;
    public string description;
    public Color color;
    public Rarity rarity;

    public EffectClause(string id, string name, string description, Color color, Rarity rarity = Rarity.Common)
    {
        this.id = id;
        this.name = name;
        this.description = description;
        this.color = color;
        this.rarity = rarity;
    }
}

public enum Rarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}
