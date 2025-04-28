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
        { "c1", new("c1", "[has] 1 dice", "#ffcd00".HexColor()) },
        { "c2", new("c2", "[has] ≥2 same dice face", "#ff8200".HexColor(), 1.2f) }
    };
    public Dictionary<string, EffectClause> Effects = new()
    {
        { "e1", new("e1", "deal [this] damage", "#ffcd00".HexColor()) },
        { "e3", new("e3", "deal [this]x2 damage", "#9300ff".HexColor()) }
    };
    public List<List<SkillCondition>> Positions = new()
    {
        new() {SkillCondition.Left, SkillCondition.Right },
        new() {SkillCondition.Current, SkillCondition.First},
        
    };

    public float CalculatePosMul(List<SkillCondition> conditions)
    {
        return (conditions.Count - 1) * 0.2f + 1; 
    }

    public void UpdatePositionArrowGraphic(List<SkillCondition> positions, Image[] positionArrows)
    {
        foreach (var item in positionArrows)
        {
            item.sprite = posArrowDict["blank"];
        }
        List<int> taken = new List<int>() { 0, 1, 2, 3, 4 };
        //skillPosCondition = positions;
        if (positions.Contains(SkillCondition.Left))
        {
            positionArrows[0].sprite = posArrowDict["left"];
            taken.Remove(0);
        }
        if (positions.Contains(SkillCondition.Right))
        {
            positionArrows[4].sprite = posArrowDict["right"];
            taken.Remove(4);
        }
        if (positions.Contains(SkillCondition.Current))
        {
            positionArrows[2].sprite = posArrowDict["this"];
            taken.Remove(2);
        }
        if (positions.Contains(SkillCondition.First))
        {
            int avai = taken[0];
            positionArrows[avai].sprite = posArrowDict["1st"];
            taken.Remove(avai);
        }
        if (positions.Contains(SkillCondition.Second))
        {
            int avai = taken[0];
            positionArrows[avai].sprite = posArrowDict["2nd"];
            taken.Remove(avai);
        }
        if (positions.Contains(SkillCondition.Third))
        {
            int avai = taken[0];
            positionArrows[avai].sprite = posArrowDict["3rd"];
            taken.Remove(avai);
        }
        if (positions.Contains(SkillCondition.Fifth))
        {
            int avai = taken[^1];
            positionArrows[avai].sprite = posArrowDict["5th"];
            taken.Remove(avai);
        }
        if (positions.Contains(SkillCondition.Fourth))
        {
            int avai = taken[^1];
            positionArrows[avai].sprite = posArrowDict["4th"];
            taken.Remove(avai);
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
