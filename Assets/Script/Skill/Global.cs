using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Global : Singleton<Global>
{

    private void Awake()
    {

    }

    public Dictionary<string, EffectClause> Effects = new()
    {
        { "e1", new("e1", "Rusty Axe", "<span style='color:#00ff00'>+1 dmg</span>", "#ff0000".HexColor(), 0) },
        { "e2", new("e2", "Dual Sword", "If dices on left and right are the same<br><span style='color:#00ff00'>+30 dmg</span>", "#0000ff".HexColor(), 5) },
        { "e3", new("e3", "Twin Bracelets", "For every pair,<br><span style='color:#00ff00'>+8 dmg</span>", "#ff00bf".HexColor(), 5) },
        { "e4", new("e4", "Gambler's Dice", "<span style='color:#00ff00'>0.1x mult</span> for each dice manually rerolled<br><span style='color:#ff0000'>reset after one attack</span>", "#ffff00".HexColor(), 10, Rarity.Rare) },
        { "e5", new("e5", "Berserker's Axe", "<span style='color:#00ff00'>+30 dmg</span> for each attack performed this round", "#ff0000".HexColor(), 6) },
        { "e6", new("e6", "Single Glove", "If Solo dice on this card<br><span style='color:#00ff00'>+40 dmg</span>", "#ff00bf".HexColor(), 7) },
        { "e7", new("e7", "Feathered Blade", "Foreach Sequence,<br><span style='color:#00ff00'>+20 dmg</span>", "#0000ff".HexColor(), 5) },
        { "e8", new("e8", "Twin Buckler", "If there is adjacent pair<br><span style='color:#00ff00'>+20 dmg</span>", "#00ff00".HexColor(), 5) },
        { "e9", new("e9", "Dragon Claw", "3 of a kind that has dice on this card<br><span style='color:#00ff00'>+50 dmg</span>", "#00ffff".HexColor(), 10) },
        { "e10", new("e10", "Fading Ember Ring", "<span style='color:#00ff00'>+50 dmg</span>,<br><span style='color:#ff0000'>decrease by 5</span> after each attack (min 0)<br>currently [v0]", "#ff8000".HexColor(), 5) },
        { "e11", new("e11", "6 Pearls", "For each value 6 in attack,<br><span style='color:#00ff00'>+10 dmg</span>", "#00ffff".HexColor(), 5) },
        { "e12", new("e12", "Sacrificial Totem", "<span style='color:#00ff00'>0.5x mult</span>,<br><span style='color:#ff0000'>disable this card</span>", "#8000ff".HexColor(), 12) },
        { "e13", new("e13", "Red band", "<span style='color:#00ff00'>+20 dmg</span>, increase by 5 after each round<br>currently [v0]", "#ff0000".HexColor(), 10) },
        { "e14", new("e14", "Executioner's Blade", "If attack only contains high value (5+),<br><span style='color:#00ff00'>gain 0.7x mult</span>", "#ff0000".HexColor(), 7, Rarity.Rare) },
        { "e15", new("e15", "Final Hour", "If this is the last attack,<br><span style='color:#00ff00'>gain 0.7x mult</span>", "#00ffff".HexColor(), 5, Rarity.Rare) },
        { "e16", new("e16", "Ring of Fate", "Round start:<br>one random attack type gain <span style='color:#00ff00'>0.8x mult</span><br>current attack type: [s0]", "#ffff00".HexColor(), 10) },
        { "e17", new("e17", "Heavy Gauntlet", "If attack contains 3+ dices,<br><span style='color:#00ff00'>+30 dmg</span>", "#ff8000".HexColor(), 15) },
        { "e18", new("e18", "Ace of Spade", "If attack contains 2- dices,<br><span style='color:#00ff00'>gain 1.5x mult</span>", "#ffff00".HexColor(), 7, Rarity.Rare) },
        { "e19", new("e19", "Loaded Crossbow", "<span style='color:#ff0000'>Consume 1 attack</span>,<br><span style='color:#00ff00'>gain 1x mult</span>", "#ff0000".HexColor(), 8, Rarity.Rare) },
        { "e20", new("e20", "Arcane Orb", "If attack contains only special dices,<br><span style='color:#00ff00'>gain 2x mult</span>", "#0000ff".HexColor(), 10, Rarity.Rare) },
        { "e21", new("e21", "Warrior Badge", "If attack contains only normal dices,<br><span style='color:#00ff00'>+100 dmg</span>", "#00ff00".HexColor(), 7, Rarity.Rare) },
        { "e22", new("e22", "Preservation Ring", "<span style='color:#00ff00'>Gain 0.2x mult</span>, increase by 0.01x foreach unused reroll after each round<br>currently [v0]", "#00ff00".HexColor(), 15, Rarity.Rare) },
        { "e23", new("e23", "Chaos Trinket", "<span style='color:#00ff00'>+20 dmg</span> for each dice not used in a pattern", "#8000ff".HexColor(), 5) },
        { "e24", new("e24", "Automation Controller", "All high value (5+) always attack regardless of pattern<br><span style='color:#00ff00'>+10 dmg</span> foreach high value", "#8000ff".HexColor(), 5) },
        { "e25", new("e25", "Kingmaker's Sigil", "If all other card's dices is value 6,<br><span style='color:#00ff00'>trigger All Sixes pattern</span>", "#ffff00".HexColor(), 5, Rarity.Rare) },
        { "e26", new("e26", "Mirror Prism", "Dice on this card always attack<br><span style='color:#00ff00'>+dice value x10 as dmg</span>", "#0000ff".HexColor(), 7) },
        { "e27", new("e27", "Symmetry Staff", "If dice 1 and 5 are both even,<br><span style='color:#00ff00'>+40 dmg</span>", "#8000ff".HexColor(), 8) },
        { "e28", new("e28", "Centering Lotus", "If this card is middle and its dice value is 6,<br><span style='color:#00ff00'>gain 0.5x mult</span>", "#00ff00".HexColor(), 5, Rarity.Rare) },
        { "e29", new("e29", "Trinity Linked Chain", "If position 2,3,4 form a sequence,<br><span style='color:#00ff00'>+100 dmg</span>", "#ff8000".HexColor(), 10) },
    };

    public readonly Dictionary<DicePattern, string> DisplayNames = new Dictionary<DicePattern, string>
    {
        { DicePattern.Single, "Single" },
        { DicePattern.OnePair, "One Pair" },
        { DicePattern.TwoPair, "Two Pair" },
        { DicePattern.ThreeOfAKind, "Three of a Kind" },
        { DicePattern.FourOfAKind, "Four of a Kind" },
        { DicePattern.FiveOfAKind, "Five of a Kind" },
        { DicePattern.SixOfAKind, "Six of a Kind" },
        { DicePattern.FullHouse, "Full House" },
        { DicePattern.TwoTriplet, "Two Triplets" },
        { DicePattern.ThreePair, "Three Pairs" },
        { DicePattern.FullSixes, "Full Sixes" },
        { DicePattern.Sequence3, "Sequence of 3" },
        { DicePattern.Sequence4, "Sequence of 4" },
        { DicePattern.Sequence5, "Sequence of 5" },
        { DicePattern.Sequence6, "Sequence of 6" }
    };
}

public class EffectClause
{
    public string id;
    public string name;
    public string description;
    public int cost;
    public Color color;
    public Rarity rarity;
    public Sprite sprite;

    public EffectClause(string id, string name, string description, Color color, int cost, Rarity rarity = Rarity.Common, Sprite sprite = null)
    {
        this.id = id;
        this.name = name;
        this.description = description;
        this.cost = cost;
        this.color = color;
        this.rarity = rarity;
        this.sprite = sprite;
        this.sprite = sprite;
    }
}

public enum Rarity
{
    Common,
    Rare,
}