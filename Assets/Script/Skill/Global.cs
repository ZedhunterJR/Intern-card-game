using System;
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
      { "e1", new("e1", "Rusty Axe", "<color=#2e7d32>+1 dmg</color>", "#cc6666".HexColor(), 0) },
      { "e2", new("e2", "Dual Sword", "If dices on left and right are the same\n<color=#2e7d32>+30 dmg</color>", "#6699cc".HexColor(), 5) },
      { "e3", new("e3", "Twin Bracelets", "For every pair,\n<color=#2e7d32>+8 dmg</color>", "#cc66aa".HexColor(), 5) },
      { "e4", new("e4", "Gambler's Dice", "<color=#2e7d32>0.1x mult</color> for each dice manually rerolled\n<color=#cc6666>reset after one attack</color>", "#cccc66".HexColor(), 10, Rarity.Rare) },
      { "e5", new("e5", "Berserker's Axe", "<color=#2e7d32>+30 dmg</color> for each attack performed this round", "#cc6666".HexColor(), 6) },
      { "e6", new("e6", "Single Glove", "If Solo dice on this card\n<color=#2e7d32>+40 dmg</color>", "#cc66aa".HexColor(), 7) },
      { "e7", new("e7", "Feathered Blade", "Foreach Sequence,\n<color=#2e7d32>+20 dmg</color>", "#6699cc".HexColor(), 5) },
      { "e8", new("e8", "Twin Buckler", "If there is adjacent pair\n<color=#2e7d32>+20 dmg</color>", "#2e7d32".HexColor(), 5) },
      { "e9", new("e9", "Dragon Claw", "3 of a kind that has dice on this card\n<color=#2e7d32>+50 dmg</color>", "#66cccc".HexColor(), 10) },
      { "e10", new("e10", "Fading Ember Ring", "<color=#2e7d32>+50 dmg</color>,\n<color=#cc6666>decrease by 5</color> after each attack (min 0)\ncurrently [v0]", "#cc9966".HexColor(), 5) },
      { "e11", new("e11", "6 Pearls", "For each value 6 in attack,\n<color=#2e7d32>+10 dmg</color>", "#66cccc".HexColor(), 5) },
      { "e12", new("e12", "Sacrificial Totem", "<color=#2e7d32>0.5x mult</color>,\n<color=#cc6666>disable this card</color>", "#9966cc".HexColor(), 12) },
      { "e13", new("e13", "Red band", "<color=#2e7d32>+20 dmg</color>, increase by 5 after each round\ncurrently [v0]", "#cc6666".HexColor(), 10) },
      { "e14", new("e14", "Executioner's Blade", "If attack only contains high value (5+),\n<color=#2e7d32>gain 0.7x mult</color>", "#cc6666".HexColor(), 7, Rarity.Rare) },
      { "e15", new("e15", "Final Hour", "If this is the last attack,\n<color=#2e7d32>gain 0.7x mult</color>", "#66cccc".HexColor(), 5, Rarity.Rare) },
      { "e16", new("e16", "Ring of Fate", "Round start:\none random attack type gain <color=#2e7d32>0.8x mult</color>\ncurrent attack type: [s0]", "#cccc66".HexColor(), 10) },
      { "e17", new("e17", "Heavy Gauntlet", "If attack contains 3+ dices,\n<color=#2e7d32>+30 dmg</color>", "#cc9966".HexColor(), 15) },
      { "e18", new("e18", "Ace of Spade", "If attack contains 2- dices,\n<color=#2e7d32>gain 1.5x mult</color>", "#cccc66".HexColor(), 7, Rarity.Rare) },
      { "e19", new("e19", "Loaded Crossbow", "<color=#cc6666>Consume 1 attack</color>,\n<color=#2e7d32>gain 1x mult</color>", "#cc6666".HexColor(), 8, Rarity.Rare) },
      { "e20", new("e20", "Arcane Orb", "If attack contains only special dices,\n<color=#2e7d32>gain 2x mult</color>", "#6699cc".HexColor(), 10, Rarity.Rare) },
      { "e21", new("e21", "Warrior Badge", "If attack contains only normal dices,\n<color=#2e7d32>+100 dmg</color>", "#2e7d32".HexColor(), 7, Rarity.Rare) },
      { "e22", new("e22", "Preservation Ring", "<color=#2e7d32>Gain 0.2x mult</color>, increase by 0.01x for each unused reroll after each round\ncurrently [v0]", "#2e7d32".HexColor(), 15, Rarity.Rare) },
      { "e23", new("e23", "Chaos Trinket", "<color=#2e7d32>+20 dmg</color> for each dice not used in a pattern", "#9966cc".HexColor(), 5) },
      { "e24", new("e24", "Automation Controller", "All high value (5+) always attack regardless of pattern\n<color=#2e7d32>+10 dmg</color> for each high value", "#9966cc".HexColor(), 5) },
      { "e25", new("e25", "Kingmaker's Sigil", "If all other card's dices is value 6,\n<color=#2e7d32>trigger All Sixes pattern</color>", "#cccc66".HexColor(), 5, Rarity.Rare) },
      { "e26", new("e26", "Mirror Prism", "Dice on this card always attack\n<color=#2e7d32>+dice value x10 as dmg</color>", "#6699cc".HexColor(), 7) },
      { "e27", new("e27", "Symmetry Staff", "If dice 1 and 5 are both even,\n<color=#2e7d32>+40 dmg</color>", "#9966cc".HexColor(), 8) },
      { "e28", new("e28", "Centering Lotus", "If this card is middle and its dice value is 6,\n<color=#2e7d32>gain 0.5x mult</color>", "#2e7d32".HexColor(), 5, Rarity.Rare) },
      { "e29", new("e29", "Trinity Linked Chain", "If position 2,3,4 form a sequence,\n<color=#2e7d32>+100 dmg</color>", "#cc9966".HexColor(), 10) }
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

    public float GetPatternPoints(DicePattern pattern)
    {
        switch (pattern)
        {
            case DicePattern.Single:
                return 10;
            case DicePattern.OnePair:
                return 20;
            case DicePattern.TwoPair:
                return 40;
            case DicePattern.ThreeOfAKind:
                return 30;
            case DicePattern.FourOfAKind:
                return 60;
            case DicePattern.FiveOfAKind:
                return 100;
            case DicePattern.SixOfAKind:
                return 200;
            case DicePattern.FullHouse:
                return 80;
            case DicePattern.TwoTriplet:
                return 200;
            case DicePattern.ThreePair:
                return 200;
            case DicePattern.FullSixes:
                return 200;
            case DicePattern.Sequence3:
                return 30;
            case DicePattern.Sequence4:
                return 50;
            case DicePattern.Sequence5:
                return 80;
            case DicePattern.Sequence6:
                return 200;
            default:
                return 0;
        }
    }
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