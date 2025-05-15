using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class SkillManager : Singleton<SkillManager>
{
    // To Spawn Enenmy Test
    [SerializeField] Transform enemySpawnPlace;
    [SerializeField] EnemyTest enemyPrefabs;
    private EnemyTest enemyTest;
    public EnemyTest EnemyTest => enemyTest;

    public Dictionary<(Skill, string), Action> actionHelpers = new();

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
    public List<Dice> PlayedDices(out List<Dice> extraDices)
    {
        List<Dice> diceList = new List<Dice>();
        extraDices = new();
        foreach (var s in Skills())
        {
            if (s.diceFace != null)
            {
                diceList.Add(s.diceFace);
                if (s.diceFace.CurrentDiceType == DiceType.Twin)
                    extraDices.Add(s.diceFace);
            }    
            else
            {
                diceList.Add(null);
            }
        }
        return diceList;
    }

    private void Start()
    {
        SkillCardList = CardHolder.Instance.cards;
        enemyTest = Instantiate(enemyPrefabs, enemySpawnPlace).GetComponent<EnemyTest>();
        enemyTest.Init();
        GameManager.Instance.ChangeGameStatus(GameStatus.Battle);
    }

    //related to point/sequencing
    public Queue<VisualPointSeq> visualPointQueue = new(); 
    private void EnqueuePattern(DicePattern patt)
    {
        visualPointQueue.Enqueue(new(dicePattern: patt));
        currentDicePattern = patt;
    }
    private void EnqueuePoint(float p, Skill skill = null, Dice dice = null)
    {
        var vp = new VisualPointSeq(point: p);
        vp.skill = skill;
        vp.dice = dice;
        visualPointQueue.Enqueue(vp);
    }
    private void EnqueueMult(float m, Skill skill = null, Dice dice = null)
    {
        var vp = new VisualPointSeq(mult: m);
        vp.skill = skill;
        vp.dice = dice;
        visualPointQueue.Enqueue(vp);
    }
    public List<Dice> diceInPlayed = new();
    private List<Dice> extraDices = new();
    private List<Skill> playedSkills = new();
    private List<Dice> AllPlayedDices()
    {
        var re = new List<Dice>(extraDices);
        foreach (var dice in diceInPlayed)
        {
            if (dice != null)
            {
                re.Add(dice);
            }
        }
        return re;
    }
    private DicePattern currentDicePattern;
    public void PlayCard()
    {
        //you shouldn't see the play/reroll button during shopping anyway
        if (GameManager.Instance.GameStatus != GameStatus.Battle)
        {
            return;
        }
        //will add a timeout for sequencing point, during which cannot press play
        if (GameManager.Instance.CurrentNumOfTurn <= 0)
        {
            Debug.Log("Không đủ lượt đánh");
            return;
        }


        playedSkills = Skills();
        diceInPlayed = PlayedDices(out extraDices);
        EnqueuePattern(DetectDicePattern(AllPlayedDices()));
        for (int i = 0; i< 5; i++)
        {
            ProcessCardEffect(playedSkills[i], i);
        }

        foreach (var diceValue in diceInPlayed)
        {
            if (diceValue != null)
            {
                if (diceValue.usedInAttack || diceValue.includedInPoint)
                {
                    EnqueuePoint(diceValue.CalculatePoint(), dice: diceValue);
                }
            }
        }

        GameManager.Instance.SetTurns();

        StartCoroutine(AttackSequence.Instance.CaculatePointSequence());

        foreach (var a in actionHelpers)
        {
            a.Value?.Invoke();
        }
        //after coroutine of point and shit
        //DiceManager.Instance.StartTurn();

        //enemyTest.Damage(100);
    }

    public void ProcessCardEffect(Skill skill, int index)
    {
        switch (skill.skillEffect.id)
        {
            case "e1":
                EnqueuePoint(1, skill);
                break;
            case "e2":
                if (index == 0 && index == 4) return;
                if (diceInPlayed[index - 1] != null && diceInPlayed[index + 1] != null)
                {
                    if (diceInPlayed[index - 1].currentFace == diceInPlayed[index + 1].currentFace)
                        EnqueuePoint(30, skill);
                }
                break;
            case "e3":
                switch (currentDicePattern)
                {
                    case DicePattern.OnePair:
                        EnqueuePoint(8, skill);
                        break;
                    case DicePattern.TwoPair:
                        EnqueuePoint(16, skill);
                        break;
                    case DicePattern.FullHouse:
                        EnqueuePoint(8, skill);
                        break;
                    case DicePattern.ThreePair:
                        EnqueuePoint(24, skill);
                        break;
                }
                break;
            case "e4":
                EnqueueMult(skill.v0, skill);
                break;
            case "e5":
                EnqueuePoint(30 * (GameManager.Instance.maxNumOfTurn - GameManager.Instance.CurrentNumOfTurn), skill);
                break;
            case "e6":
                if (currentDicePattern == DicePattern.Single)
                {
                    if (diceInPlayed[index] != null)
                        if (diceInPlayed[index].usedInAttack)
                            EnqueuePoint(40, skill);
                }
                break;
            case "e7":
                if (currentDicePattern == DicePattern.Sequence3 || currentDicePattern == DicePattern.Sequence4
                    || currentDicePattern == DicePattern.Sequence5 || currentDicePattern == DicePattern.Sequence6)
                {
                    EnqueuePoint(20, skill);
                }
                break;
            case "e8":
                for (int i = 1; i < 5; i++)
                {
                    if (diceInPlayed[i] == null || diceInPlayed[i - 1] == null)
                        continue;
                    if (diceInPlayed[i].currentFace == diceInPlayed[i-1].currentFace)
                    {
                        EnqueuePoint(20, skill);
                        break;
                    }
                }
                break;
            case "e9":
                if (currentDicePattern == DicePattern.ThreeOfAKind && currentDicePattern == DicePattern.TwoTriplet)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        if (diceInPlayed[i] == null) continue;
                        if (diceInPlayed[i].usedInAttack && i == index)
                        {
                            EnqueuePoint(50, skill); break;
                        }
                    }
                }
                if (currentDicePattern == DicePattern.FullHouse)
                {
                    print("I suck so if this shows up, i'll do it later");
                }
                break;
            case "e10":
                EnqueuePoint(skill.v0, skill);
                break;
            case "e11":
                foreach (var dice in AllPlayedDices())
                {
                    if (dice.currentFace == 6)
                        EnqueuePoint(10, skill);
                }
                break;
            case "e12":
                EnqueueMult(0.5f, skill);
                print("not implemented the disable part");
                break;
            case "e13":
                EnqueuePoint(skill.v0, skill);
                break;
            case "e14":
                bool eli = true;
                foreach (var dice in AllPlayedDices())
                {
                    if (dice.currentFace < 5)
                    {
                        eli = false;
                        break;
                    }
                }
                if (eli)
                    EnqueueMult(0.7f, skill);
                break;
            case "e15":
                if (GameManager.Instance.CurrentNumOfTurn == 1)
                    EnqueueMult(0.7f, skill);
                break;
            case "e16":
                if (Enum.TryParse(skill.s0, out DicePattern patt))
                {
                    if (currentDicePattern == patt)
                        EnqueueMult(0.8f, skill);
                }
                break;
            case "e17":
                if (AllPlayedDices().Count >= 3)
                    EnqueuePoint(30, skill);
                break;
            case "e18":
                if (AllPlayedDices().Count < 3)
                    EnqueueMult(1.5f, skill);
                break;
            case "e19":
                EnqueueMult(1f, skill);
                GameManager.Instance.SetTurns();
                break;
            case "e20":
                var eli20 = true;
                foreach (var d in AllPlayedDices())
                {
                    if (d.CurrentDiceType == DiceType.Normal)
                    {
                        eli20 = false;
                        break;
                    }
                }
                if (eli20)
                    EnqueueMult(2f, skill);
                break;
            case "e21":
                var eli21 = true;
                foreach (var d in AllPlayedDices())
                {
                    if (d.CurrentDiceType != DiceType.Normal)
                    {
                        eli20 = false;
                        break;
                    }
                }
                if (eli21)
                    EnqueuePoint(100f, skill);
                break;
            case "e22":
                EnqueueMult(skill.v0, skill);
                break;
            case "e23":
                foreach (var d in AllPlayedDices())
                {
                    if (!d.usedInAttack)
                        EnqueuePoint(20, skill);
                }
                break;
            case "e24":
                foreach (var d in AllPlayedDices())
                {
                    if (d.currentFace >= 5)
                    {
                        d.includedInPoint = true;
                        EnqueuePoint(10, skill);
                    }
                }
                break;
            case "e25":
                var eli25 = true;
                for (int i = 0; i < 5; i++)
                {
                    if (i == index)
                        continue;
                    if (diceInPlayed[i] == null)
                    {
                        eli25 = false;
                        break;
                    }
                    if (diceInPlayed[i].currentFace != 6)
                    {
                        eli25 = false;
                        break;
                    }
                }
                if (eli25)
                    EnqueuePattern(DicePattern.FullSixes);
                break;
            case "e26":
                if (diceInPlayed[index] != null)
                {
                    EnqueuePoint(diceInPlayed[index].currentFace * 10f, skill);
                    diceInPlayed[index].includedInPoint = true;
                }
                break;
            case "e27":
                if (diceInPlayed[0] == null && diceInPlayed[4] == null)
                    break;
                if (diceInPlayed[0].currentFace % 2 == 0 && diceInPlayed[4].currentFace % 2 == 0)
                {
                    EnqueuePoint(40, skill);
                }
                break;
            case "e28":
                if (index == 2)
                {
                    if (diceInPlayed[2] == null)
                        break;
                    if (diceInPlayed[2].currentFace == 6)
                        EnqueueMult(0.5f, skill);
                }
                break;
            case "e29":
                if (currentDicePattern == DicePattern.Sequence5 || currentDicePattern == DicePattern.Sequence6)
                    EnqueuePoint(100, skill);
                if (currentDicePattern == DicePattern.Sequence3 || currentDicePattern == DicePattern.Sequence4)
                {
                    if (diceInPlayed[1] == null || diceInPlayed[2] == null || diceInPlayed[3] == null)
                        if (diceInPlayed[1].usedInAttack && diceInPlayed[2].usedInAttack && diceInPlayed[3].usedInAttack)
                            EnqueueMult(100, skill);
                }
                break;

        }
    }    

    public DicePattern DetectDicePattern(List<Dice> diceList)
    {
        if (diceList == null || diceList.Count == 0)
            throw new ArgumentException("There should be a check to not play when no dice is present");
        List<int> values = diceList
            .Where(d => d.currentFace != -1)
            .Select(d => d.currentFace)
            .ToList();
        if (diceList.Count == 0)
            return DicePattern.None;
        // Reset used flags
        foreach (var die in diceList)
            die.usedInAttack = false;

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

    public void RemoveEffectFromSkill(Skill skill, EffectClause effect)
    {
        if (effect == null) return;
        switch (effect.id)
        {
            case "e4":
                actionHelpers.Remove((skill, "e4"));
                GameManager.Instance.startRoundActionHelpers.Remove((skill, "e4"));
                DiceManager.Instance.DiceRerollListener.Remove((skill, "e4"));
                break;
            case "e10":
                actionHelpers.Remove((skill, "e10"));
                break;
            case "e13":
                actionHelpers.Remove((skill, "e13"));
                break;
            case "e16":
                GameManager.Instance.startRoundActionHelpers.Remove((skill, "e16"));
                break;
            case "e22":
                GameManager.Instance.startRoundActionHelpers.Remove((skill, "e22"));
                break;
        }
        skill.v0 = 0;
        skill.v1 = 0;
        skill.v2 = 0;
        skill.s0 = "";
        skill.s1 = "";
        skill.s2 = "";
    }
    public void AddEffectToSkill(Skill skill, EffectClause effect)
    {
        switch (effect.id)
        {
            case "e4":
                actionHelpers[(skill, "e4")] = () => skill.v0 = 0;
                GameManager.Instance.startRoundActionHelpers[(skill, "e4")] = () => skill.v0 = 0;
                DiceManager.Instance.DiceRerollListener[(skill, "e4")] = (value) => skill.v0 += value;
                break;
            case "e10":
                actionHelpers[(skill, "e10")] = () => skill.v0 -= 5;
                skill.v0 = 50;
                break;
            case "e13":
                actionHelpers[(skill, "e13")] = () => skill.v0 += 5;
                skill.v0 = 20;
                break;
            case "e16":
                GameManager.Instance.startRoundActionHelpers[(skill, "e16")] = () =>
                {
                    var patt = DicePattern.None;
                    while (patt == DicePattern.None)
                    {
                        patt = Extensions.GetRandomEnum<DicePattern>();
                    }
                    skill.s0 = patt.ToString();
                };
                break;
            case "e22":
                GameManager.Instance.startRoundActionHelpers[(skill, "e22")] = () =>
                {
                    skill.v0 += 0.01f * GameManager.Instance.CurrentNumOfReroll;
                };
                skill.v0 = 0.2f;
                break;
        }
    }


}

public enum DicePattern
{
    None,
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

public class VisualPointSeq
{
    public float point;
    public float mult;
    public DicePattern dicePattern = DicePattern.None;
    public Skill skill;
    public Dice dice;
    public VisualPointSeq(float point = 0f, float mult = 0f, DicePattern dicePattern = DicePattern.None)
    {
        this.point = point;
        this.mult = mult;
        this.dicePattern = dicePattern;
    }
}
