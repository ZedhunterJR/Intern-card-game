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
        foreach (var s in Skills())
        {
            if (s.diceFace != null)
            {
                diceList.Add(s.diceFace);
            }    
            else
            {
                diceList.Add(null);
            }
        }
        extraDices = new();
        return diceList;
    }

    private void Start()
    {
        SkillCardList = CardHolder.Instance.cards;
        enemyTest = Instantiate(enemyPrefabs, enemySpawnPlace).GetComponent<EnemyTest>();
        enemyTest.Init();
        GameManager.Instance.GameStatus = GameStatus.Battle;
    }

    public void ReturnDicesToHolderAfterPlayed()
    {
        foreach (var card in SkillCardList)
        {
            var cardScript = card.GetComponent<Skill>();
            cardScript.ReturnDicesToHolder();
        }

        switch (GameManager.Instance.GameStatus)
        {
            case GameStatus.Battle:
                StartCoroutine(DiceManager.Instance.RerollAnim(DiceManager.Instance.diceList, false));
                break;
            case GameStatus.Shop:
                break;
            default:
                break;
        }
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
    private List<Dice> diceInPlayed = new();
    private List<Dice> extraDices = new();
    private List<Skill> playedSkills = new();
    private List<Dice> AllPlayedDices()
    {
        var re = new List<Dice>(extraDices);
        foreach (var dice in diceInPlayed)
        {
            if (dice != null)
                re.Add(dice);
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
        visualPointQueue.Enqueue(new(dicePattern: DetectDicePattern(diceInPlayed)));
        for (int i = 0; i< 5; i++)
        {
            ProcessCardEffect(playedSkills[i], i);
        }

        GameManager.Instance.SetTurns();

        /*switch (pattern)
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
        }*/


        while (visualPointQueue.Count > 0)
        {
            var q = visualPointQueue.Dequeue();
            if (q.dicePattern != DicePattern.None)
                print("queueing " + q.dicePattern.ToString());
            else
                print($"point: {q.point}, mult: {q.mult}");
        }


        foreach (var a in actionHelpers)
        {
            a.Value?.Invoke();
        }
        ReturnDicesToHolderAfterPlayed();
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
                //listener
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
                //special dices
                break;
            case "e21":
                //normal dices
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

    public void RemoveEffectFromSkill(Skill skill, EffectClause effect)
    {
        switch (effect.id)
        {
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

public class ActionHelper
{
    public string id;
    public Action action;
    public ActionHelper(string id, Action action)
    {
        this.id = id;
        this.action = action;
    }
}