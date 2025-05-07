using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkillManager : Singleton<SkillManager>
{
    // To Spawn Enenmy Test
    [SerializeField] Transform enemySpawnPlace;
    [SerializeField] EnemyTest enemyPrefabs;
    private EnemyTest enemyTest;
    public EnemyTest EnemyTest => enemyTest;

    public List<GameObject> SkillCardList;
    public List<Skill> Skills()
    {
        var skillList = new List<Skill>();
        foreach (var s in SkillCardList)
        {
            skillList.Add(s.GetComponent<Skill>());
        }
        return skillList;
    }
    public List<Dice> PlayedDices()
    {
        List<Dice> diceList = new List<Dice>();
        foreach (var s in Skills())
        {
            if (s.diceFace != null)
            {
                diceList.Add(s.diceFace);
            }    
        } 
        return diceList;
    }

    private void Start()
    {
        SkillCardList = CardHolder.Instance.cards;
        enemyTest = Instantiate(enemyPrefabs, enemySpawnPlace).GetComponent<EnemyTest>();
        enemyTest.Init();
        GameManager.Instance.GameStatus = GameStatus.Battle;
    }

    public void PlayCard()
    {
        if (GameManager.Instance.GameStatus != GameStatus.Battle)
        {
            return;
        }
        if (GameManager.Instance.NumOfTurns <= 0)
        {
            Debug.Log("Không đủ lượt đánh");
            return;
        }
        var playedDices = PlayedDices();
        var pattern = DetectDicePattern(playedDices);
        var mult = 1f;
        var point = 0f;

        GameManager.Instance.SubtractTurns();

        switch (pattern)
        {
            case DicePattern.Single:
                point += 10;
                // Handle Single
                break;
            case DicePattern.OnePair:
                point += 20;
                // Handle One Pair
                break;
            case DicePattern.TwoPair:
                point += 40;
                // Handle Two Pair
                break;
            case DicePattern.ThreeOfAKind:
                point += 30;
                // Handle Three of a Kind
                break;
            case DicePattern.FourOfAKind:
                point += 60;
                // Handle Four of a Kind
                break;
            case DicePattern.FiveOfAKind:
                point += 100;
                // Handle Five of a Kind
                break;
            case DicePattern.SixOfAKind:
                point += 200;
                // Handle Six of a Kind
                break;
            case DicePattern.FullHouse:
                point += 80;
                // Handle Full House
                break;
            case DicePattern.TwoTriplet:
                point += 200;
                // Handle Two Triplets
                break;
            case DicePattern.ThreePair:
                point += 200;
                // Handle Three Pair
                break;
            case DicePattern.FullSixes:
                point += 200;
                // Handle Full Sixes
                break;
            case DicePattern.Sequence3:
                point += 30;
                // Handle Sequence of 3
                break;
            case DicePattern.Sequence4:
                point += 50;
                // Handle Sequence of 4
                break;
            case DicePattern.Sequence5:
                point += 80;
                // Handle Sequence of 5
                break;
            case DicePattern.Sequence6:
                point += 200;
                // Handle Sequence of 6
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        var a = ProcessCardEffect(Skills(), pattern);
        point += a.point;
        mult += a.mult;
        var total = point * mult;

        foreach (var d in playedDices)
        {
            if (d.usedInAttack)
            {
                point += d.currentFace;
            }    
        }    
        print($"Pattern used: {pattern}. Damage: {total}");

        enemyTest.Damage(total);
    }

    public (float point, float mult) ProcessCardEffect(List<Skill> skills, DicePattern dicePattern)
    {
        float point = 0;
        float mult = 0;

        for (int i = 0; i < skills.Count; i++)
        {
            switch (skills[i].skillEffect.id)
            {
                case "e1":
                    point += 1;
                    break;
                case "e2":
                    break;
                case "e3":
                    switch (dicePattern)
                    {
                        case DicePattern.OnePair:
                            point += 8;
                            break;
                        case DicePattern.TwoPair:
                            point += 16;
                            break;
                        case DicePattern.FullHouse:
                            point += 8;
                            break;
                    }
                    break;
            }
        }

        return (point, mult);
    }    

    public DicePattern DetectDicePattern(List<Dice> diceList)
    {
        if (diceList == null || diceList.Count == 0)
            throw new ArgumentException("Must provide at least one die.");

        // Reset used flags
        foreach (var die in diceList)
            die.usedInAttack = false;

        List<int> values = diceList.Select(d => d.currentFace).ToList();

        var counts = values.GroupBy(x => x).ToDictionary(g => g.Key, g => g.Count());
        var countList = counts.Values.OrderByDescending(v => v).ToList();
        var unique = values.Distinct().OrderBy(x => x).ToList();
        int diceCount = values.Count;

        // Helper: mark dice with matching value and desired count
        void MarkDiceUsed(int value, int count)
        {
            var matched = diceList.Where(d => d.currentFace == value && !d.usedInAttack).Take(count);
            foreach (var d in matched) d.usedInAttack = true;
        }

        // Helper: mark multiple values
        void MarkMultipleUsed(Dictionary<int, int> valueCount)
        {
            foreach (var pair in valueCount)
                MarkDiceUsed(pair.Key, pair.Value);
        }

        // Six of a kind
        if (diceCount >= 6 && countList[0] == 6)
        {
            int val = counts.First(kv => kv.Value == 6).Key;
            MarkDiceUsed(val, 6);
            return DicePattern.SixOfAKind;
        }

        // Full sixes (5 dice, all are 6)
        if (values.All(x => x == 6) && diceCount == 5)
        {
            foreach (var d in diceList)
                d.usedInAttack = true;
            return DicePattern.FullSixes;
        }

        // Five of a kind
        if (countList[0] == 5)
        {
            int val = counts.First(kv => kv.Value == 5).Key;
            MarkDiceUsed(val, 5);
            return DicePattern.FiveOfAKind;
        }

        // Four of a kind
        if (countList[0] == 4)
        {
            int val = counts.First(kv => kv.Value == 4).Key;
            MarkDiceUsed(val, 4);
            return DicePattern.FourOfAKind;
        }

        // Two triplets
        if (countList.Count >= 2 && countList[0] == 3 && countList[1] == 3)
        {
            var triplets = counts.Where(kv => kv.Value == 3).Take(2).ToDictionary(kv => kv.Key, kv => 3);
            MarkMultipleUsed(triplets);
            return DicePattern.TwoTriplet;
        }

        // Full house (3 + 2)
        if (countList.Count >= 2 && countList[0] == 3 && countList[1] == 2)
        {
            var triple = counts.First(kv => kv.Value == 3).Key;
            var pair = counts.First(kv => kv.Value == 2).Key;
            MarkDiceUsed(triple, 3);
            MarkDiceUsed(pair, 2);
            return DicePattern.FullHouse;
        }

        // Three of a kind
        if (countList[0] == 3)
        {
            int val = counts.First(kv => kv.Value == 3).Key;
            MarkDiceUsed(val, 3);
            return DicePattern.ThreeOfAKind;
        }

        // Three pairs
        if (countList.Count >= 3 && countList[0] == 2 && countList[1] == 2 && countList[2] == 2)
        {
            var pairs = counts.Where(kv => kv.Value == 2).Take(3).ToDictionary(kv => kv.Key, kv => 2);
            MarkMultipleUsed(pairs);
            return DicePattern.ThreePair;
        }

        // Two pair
        if (countList.Count >= 2 && countList[0] == 2 && countList[1] == 2)
        {
            var pairs = counts.Where(kv => kv.Value == 2).Take(2).ToDictionary(kv => kv.Key, kv => 2);
            MarkMultipleUsed(pairs);
            return DicePattern.TwoPair;
        }

        // One pair
        if (countList[0] == 2)
        {
            int val = counts.First(kv => kv.Value == 2).Key;
            MarkDiceUsed(val, 2);
            return DicePattern.OnePair;
        }

        // Sequences
        int maxSequence = 1;
        int current = 1;
        int startIndex = 0;

        for (int i = 1; i < unique.Count; i++)
        {
            if (unique[i] == unique[i - 1] + 1)
            {
                current++;
                if (current > maxSequence)
                {
                    maxSequence = current;
                    startIndex = i - current + 1;
                }
            }
            else
            {
                current = 1;
            }
        }

        if (maxSequence >= 3)
        {
            var sequence = unique.Skip(startIndex).Take(maxSequence).ToList();

            foreach (var val in sequence)
            {
                var die = diceList.FirstOrDefault(d => d.currentFace == val && !d.usedInAttack);
                if (die != null) die.usedInAttack = true;
            }

            return maxSequence switch
            {
                >= 6 => DicePattern.Sequence6,
                5 => DicePattern.Sequence5,
                4 => DicePattern.Sequence4,
                3 => DicePattern.Sequence3,
                _ => DicePattern.Single
            };
        }

        // Single (mark the die with the highest value)
        int maxVal = diceList.Max(d => d.currentFace);
        var highestDie = diceList.FirstOrDefault(d => d.currentFace == maxVal);
        if (highestDie != null) highestDie.usedInAttack = true;
        return DicePattern.Single;
    }


}

public enum DicePattern
{
    Single,
    OnePair,
    TwoPair,
    ThreeOfAKind,
    FourOfAKind,
    FiveOfAKind,
    SixOfAKind,
    FullHouse,
    TwoTriplet,
    ThreePair,
    FullSixes,
    Sequence3,
    Sequence4,
    Sequence5,
    Sequence6
}