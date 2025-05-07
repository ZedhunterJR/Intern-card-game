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
        { "e1", new("e1", "Rusty Axe", "+1 dmg", "#ffcd00".HexColor(), 0) },
        { "e2", new("e2", "Dual Sword", "If dices on left and right are the same, +30 dmg", "#ffcd00".HexColor(), 5) },
        { "e3", new("e3", "Twin Bracelets", "For every pair, +8 dmg", "#ffcd00".HexColor(), 5) },
        { "e4", new("e4", "Gambler's Dice", "0.1x mult for each dice manually rerolled, reset after one attack", "#ffcd00".HexColor(), 10, Rarity.Rare) },
        { "e5", new("e5", "Berserker's Axe", "+10 dmg for each attack performed this round", "#ff0015".HexColor(), 6) },
        { "e6", new("e6", "Single Glove", "Solo dice on this card +40 dmg", "#ff0015".HexColor(), 7) },
        { "e7", new("e7", "Feathered Blade", "Sequence get +20 dmg", "#ff0015".HexColor(), 5) },
        { "e8", new("e8", "Twin Buckler", "If there is adjacent pair, +20 dmg", "#ff0015".HexColor(), 5) },
        { "e9", new("e9", "Dragon Claw", "3 of a kind that has dice on this card get +50 dmg", "#ff0015".HexColor(), 10)},
        { "e10", new("e10", "Fading Ember Ring", "+50 dmg, decrease by 5 after each attack (min 0), currently [v0]", "#0001ff".HexColor(), 5) },
        { "e11", new("e11", "6 Pearls", "For each value 6 in attack, +10 dmg", "#0001ff".HexColor(), 5) },
        { "e12", new("e12", "Sacrificial Totem", "0.5x mult, disable this card", "#0001ff".HexColor(), 12) },
        { "e13", new("e13", "Red band", "+20 dmg, increase by 5 after each round, currently [v0]", "#0001ff".HexColor(), 10) },
        { "e14", new("e14", "Executioner's Blade", "If attack only contains high value (5+), gain 0.7x mult", "#0001ff".HexColor(), 7, Rarity.Rare) },
        { "e15", new("e15", "Final Hour", "If this is the last attack, gain 0.7x mult", "#80fa00".HexColor(), 5, Rarity.Rare) },
        { "e16", new("e16", "Wheel of Fate", "Round start: one random attack type gain 0.8x mult, current attack type: [s0]", "#80fa00".HexColor(), 10) },
        { "e17", new("e17", "Heavy Gauntlet", "If attack contains 3+ dices, +30 dmg", "#80fa00".HexColor(), 15) },
        { "e18", new("e18", "Hidden Blade", "If attack contains 2- dices, gain 1.5x mult", "#80fa00".HexColor(), 7, Rarity.Rare) },
        { "e19", new("e19", "First Strike", "Consume 1 attack, gain 1x mult, disable this card", "#80fa00".HexColor(), 8, Rarity.Rare) },
        { "e20", new("e20", "Arcane Orb", "If attack contains only special dices, gain 2x mult", "#80fa00".HexColor(), 10, Rarity.Rare) },
        { "e21", new("e21", "Warrior Badge", "If attack contains only normal dices, +100 dmg", "#9e00f9".HexColor(), 7, Rarity.Rare) },
        { "e22", new("e22", "Preservation Ring", "Gain 0.2x mult, increase by 0.1x foreach unused reroll after each round, currently [v0]", "#9e00f9".HexColor(), 15, Rarity.Rare) },
        { "e23", new("e23", "Chaos Trinket", "+30 dmg for each dice not used in a pattern", "#9e00f9".HexColor(), 5) },
        { "e24", new("e24", "Automation Controller", "All high value (5+) always attack regardless of pattern", "#9e00f9".HexColor(), 5) },
        { "e25", new("e25", "Kingmaker's Sigil", "If all other card's dices is value 6, trigger All Sixes pattern", "#9e00f9".HexColor(), 5, Rarity.Rare) },
        { "e26", new("e26", "Mirror Prism", "Dice on this card allways attack with value x 3", "#00ffff".HexColor(), 7) },
        { "e27", new("e27", "Symmetry Staff", "If dice 1 and 5 are both even, +40 dmg", "#00ffff".HexColor(), 8) },
        { "e28", new("e28", "Centering Lotus", "If this card is middle, and its dice value is 6, gain 0.5x mult", "#00ffff".HexColor(), 5, Rarity.Rare) },
        { "e29", new("e29", "Trinity Staff", "If position 2,3,4 form a sequence, +100 dmg", "#00ffff".HexColor(), 10) },
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

    public EffectClause(string id, string name, string description, Color color, int cost, Rarity rarity = Rarity.Common)
    {
        this.id = id;
        this.name = name;
        this.description = description;
        this.cost = cost;
        this.color = color;
        this.rarity = rarity;
    }
}

public enum Rarity
{
    Common,
    Rare,
}