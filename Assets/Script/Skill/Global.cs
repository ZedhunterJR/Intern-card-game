using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Global 
{
    private static Global instance;
    public static Global Instance
    {
        get
        {
            if (instance == null)
                instance = new Global();
            return instance;
        }
    }

    public Dictionary<string, ConditionClause> Conditions = new()
    {
        { "c1", new("c1", "[has] 1 dice", "#ffcd00".HexColor()) },
        { "c2", new("c2", "[has] ≥2 same dice face", "#ff8200".HexColor(), 1.2f) }
    };
    public Dictionary<string, EffectClause> Effects = new()
    {
        { "e1", new("e1", "deal [this] damage", "#ffcd00".HexColor()) },
        { "e3", new("e3", "deal [this]x2 damage", "#9300ff".HexColor()) }
    };

    public float CalculatePosMul(List<SkillCondition> conditions)
    {
        return (conditions.Count - 1) * 0.2f + 1; 
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
    public string description;
    public Color color;
    public Rarity rarity;

    public EffectClause(string id, string description, Color color, Rarity rarity = Rarity.Common)
    {
        this.id = id;
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
