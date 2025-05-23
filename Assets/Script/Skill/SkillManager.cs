using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq; 
using UnityEngine;

public class SkillManager : Singleton<SkillManager>
{
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
        //GameManager.Instance.ChangeGameStatus(GameStatus.Battle);
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
    public bool canPlay = true;
    public void PlayCard()
    {
        if (!canPlay) return;
        canPlay = false;

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

        foreach (var a in actionHelpers)
        {
            a.Value?.Invoke();
        }

        StartCoroutine(AttackSequence.Instance.CaculatePointSequence());

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
                if (index == 0 || index == 4) return;
                //Debug.Log(index);
                if (diceInPlayed[index - 1] != null && diceInPlayed[index + 1] != null) // Effect e2
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
                //print("Change this effect!");
                EnqueuePoint(30 * skill.v0, skill);
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
                if (skill.v0 == 0)
                {
                    EnqueueMult(0.5f, skill);
                    skill.v0 = 1;
                }
                //print("not implemented the disable part");
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
                //print("change this effect");
                if (GameManager.Instance.CurrentTurnBeforeEnemyAttack == 1)
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
                EnqueueMult(1.5f, skill);
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

            //batch 2
            case "e30":
                EnqueuePoint(20, skill);
                break;
            case "e31":
                if (index == 0 || index == 4) return;
                //Debug.Log(index);
                if (diceInPlayed[index - 1] == null && diceInPlayed[index + 1] == null) // Effect e2
                {
                    EnqueueMult(0.7f, skill);
                }
                break;
            case "e32":
                if (diceInPlayed[0] != null && diceInPlayed[4] != null)
                {
                    if (diceInPlayed[0].currentFace == diceInPlayed[4].currentFace)
                        EnqueuePoint(20, skill);
                }
                if (diceInPlayed[1] != null && diceInPlayed[3] != null)
                {
                    if (diceInPlayed[1].currentFace == diceInPlayed[3].currentFace)
                        EnqueuePoint(20, skill);
                }
                break;
            case "e33":
                if (diceInPlayed[1] != null && diceInPlayed[3] != null)
                {
                    if (diceInPlayed[1].currentFace % 2 == 1 && diceInPlayed[3].currentFace % 2 == 1)
                        EnqueueMult(0.5f, skill);
                }
                break;
            case "e34":
                if (currentDicePattern == DicePattern.Sequence5)
                    EnqueueMult(1f, skill);
                break;
            case "e35":
                print("later");
                break;
            case "e36":
                bool eli36 = true;
                foreach (var dice in AllPlayedDices())
                {
                    if (dice.currentFace > 3)
                    {
                        eli = false;
                        break;
                    }
                }
                if (eli36)
                    EnqueueMult(0.5f, skill);
                break;
            case "e37":
                if (GameManager.Instance.currentHp < GameManager.Instance.maxHp / 2f)
                    EnqueueMult(0.5f, skill);
                break;
            case "e38":
                List<int> diceFaces = new List<int>();
                foreach (var dice in diceInPlayed)
                {
                    if (dice != null)
                        diceFaces.Add(dice.currentFace);
                }
                bool allUnique38 = diceFaces.Count == diceFaces.Distinct().Count();
                if (allUnique38)
                    EnqueuePoint(100, skill);
                break;
            case "e39":
                if (index == 0)
                    if (diceInPlayed[0] != null)
                        if (diceInPlayed[0].currentFace == 1)
                        {
                            EnqueueMult(1f, skill);
                            EnqueuePoint(1, skill); 
                        }
                break;
            case "e40":
                if (diceInPlayed[index] != null)
                    if (diceInPlayed[index].CurrentDiceType != DiceType.Normal)
                        EnqueuePoint(30, skill);
                break;
            case "e41":
                if (GameManager.Instance.currentHp == GameManager.Instance.maxHp)
                    EnqueuePoint(20, skill);
                break;
        }
    }
    List<DicePattern> priority = new List<DicePattern> 
    {
        DicePattern.SixOfAKind,
        DicePattern.FiveOfAKind,
        DicePattern.FullSixes,
        DicePattern.Sequence6,
        DicePattern.TwoTriplet,
        DicePattern.FullHouse,
        DicePattern.FourOfAKind,
        DicePattern.ThreeOfAKind,
        DicePattern.Sequence5,
        DicePattern.ThreePair,
        DicePattern.Sequence4,
        DicePattern.TwoPair,
        DicePattern.OnePair,
        DicePattern.Sequence3,
        DicePattern.Single
    };
    public DicePattern DetectDicePattern(List<Dice> diceList)
    {
        if (diceList == null || diceList.Count == 0)
            return DicePattern.None;

        List<int> values = diceList
            .Where(d => d.currentFace != -1)
            .Select(d => d.currentFace)
            .ToList();

        if (values.Count == 0)
            return DicePattern.None;

        Dictionary<DicePattern, List<Dice>> matchedPatterns = new();

        void AddPattern(DicePattern pattern, List<int> valueSubset)
        {
            var used = new List<Dice>();
            foreach (var val in valueSubset)
            {
                var die = diceList.FirstOrDefault(d => d.currentFace == val && !used.Contains(d));
                if (die != null)
                    used.Add(die);
            }
            if (used.Count > 0)
                matchedPatterns[pattern] = used;
        }

        var counts = values.GroupBy(x => x).ToDictionary(g => g.Key, g => g.Count());
        var countList = counts.Values.OrderByDescending(v => v).ToList();
        var unique = values.Distinct().OrderBy(x => x).ToList();
        int diceCount = values.Count;

        // --- Pattern Detection ---

        if (diceCount >= 6 && countList[0] == 6)
            AddPattern(DicePattern.SixOfAKind, Enumerable.Repeat(counts.First(kv => kv.Value == 6).Key, 6).ToList());

        if (values.All(x => x == 6) && diceCount == 5)
            AddPattern(DicePattern.FullSixes, values);

        if (countList[0] == 5)
            AddPattern(DicePattern.FiveOfAKind, Enumerable.Repeat(counts.First(kv => kv.Value == 5).Key, 5).ToList());

        if (countList[0] == 4)
            AddPattern(DicePattern.FourOfAKind, Enumerable.Repeat(counts.First(kv => kv.Value == 4).Key, 4).ToList());

        if (countList.Count >= 2 && countList[0] == 3 && countList[1] == 3)
            AddPattern(DicePattern.TwoTriplet, counts.Where(kv => kv.Value == 3).Take(2).SelectMany(kv => Enumerable.Repeat(kv.Key, 3)).ToList());

        if (countList.Count >= 2 && countList[0] == 3 && countList[1] == 2)
        {
            var triple = counts.First(kv => kv.Value == 3).Key;
            var pair = counts.First(kv => kv.Value == 2).Key;
            AddPattern(DicePattern.FullHouse, Enumerable.Repeat(triple, 3).Concat(Enumerable.Repeat(pair, 2)).ToList());
        }

        if (countList[0] == 3)
            AddPattern(DicePattern.ThreeOfAKind, Enumerable.Repeat(counts.First(kv => kv.Value == 3).Key, 3).ToList());

        if (countList.Count >= 3 && countList[0] == 2 && countList[1] == 2 && countList[2] == 2)
            AddPattern(DicePattern.ThreePair, counts.Where(kv => kv.Value == 2).Take(3).SelectMany(kv => Enumerable.Repeat(kv.Key, 2)).ToList());

        if (countList.Count >= 2 && countList[0] == 2 && countList[1] == 2)
            AddPattern(DicePattern.TwoPair, counts.Where(kv => kv.Value == 2).Take(2).SelectMany(kv => Enumerable.Repeat(kv.Key, 2)).ToList());

        if (countList[0] == 2)
            AddPattern(DicePattern.OnePair, Enumerable.Repeat(counts.First(kv => kv.Value == 2).Key, 2).ToList());

        // Sequence
        int maxSeq = 1, curr = 1, start = 0;
        for (int i = 1; i < unique.Count; i++)
        {
            if (unique[i] == unique[i - 1] + 1)
            {
                curr++;
                if (curr > maxSeq)
                {
                    maxSeq = curr;
                    start = i - curr + 1;
                }
            }
            else
                curr = 1;
        }

        if (maxSeq >= 3)
        {
            var seq = unique.Skip(start).Take(maxSeq).ToList();
            DicePattern p = maxSeq switch
            {
                >= 6 => DicePattern.Sequence6,
                5 => DicePattern.Sequence5,
                4 => DicePattern.Sequence4,
                3 => DicePattern.Sequence3,
                _ => DicePattern.Single
            };
            AddPattern(p, seq);
        }

        // Always fallback to Single (highest die)
        int maxVal = values.Max();
        var highDie = diceList.FirstOrDefault(d => d.currentFace == maxVal);
        if (highDie != null)
            matchedPatterns[DicePattern.Single] = new() { highDie };

        // --- Apply best match from priority list ---
        foreach (var pattern in priority)
        {
            if (matchedPatterns.ContainsKey(pattern))
            {
                foreach (var d in matchedPatterns[pattern])
                    d.usedInAttack = true;
                return pattern;
            }
        }

        return DicePattern.None;
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
            case "e5":
                actionHelpers.Remove((skill, "e5"));
                GameManager.Instance.startRoundActionHelpers.Remove((skill, "e5"));
                break;
            case "e10":
                actionHelpers.Remove((skill, "e10"));
                break;
            case "e12":
                GameManager.Instance.startRoundActionHelpers.Remove((skill, "e12"));
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
            case "e35":
                EnemyManager.Instance.enemyAttackActionHelpers.Remove((skill, "e35"));
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
            case "e5":
                skill.v0 = 0;
                actionHelpers[(skill, "e5")] = () => skill.v0 += 30;
                GameManager.Instance.startRoundActionHelpers[(skill, "e5")] = () => skill.v0 = 0;
                break;
            case "e10":
                actionHelpers[(skill, "e10")] = () => skill.v0 -= 5;
                skill.v0 = 50;
                break;
            case "e12":
                GameManager.Instance.startRoundActionHelpers[(skill, "e12")] = () => skill.v0 = 0;
                //actionHelpers[(skill, "e5")] = () => skill.v0 += 30;
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
                    skill.v0 += 0.05f * GameManager.Instance.CurrentNumOfReroll;
                };
                skill.v0 = 0.2f;
                break;
            case "e35":
                EnemyManager.Instance.enemyAttackActionHelpers[(skill, "e35")] = (dmg) =>
                {
                    skill.v0 += 0.2f;
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
