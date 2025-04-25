using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : Singleton<SkillManager>
{
    public List<GameObject> SkillCardList;
    private List<Skill> Skills()
    {
        var skillList = new List<Skill>();
        foreach (var s in SkillCardList)
        {
            skillList.Add(s.GetComponent<Skill>());
        }
        return skillList;
    }
    private readonly Dictionary<string, float> conditionMultiplier = new()
    {
        { "c1", 1f },
        { "c2", 1.1f },
        { "c3", 1.2f },
        { "c4", 1.2f },
        { "c5", 1.1f },
        { "c6", 1f },
        { "c7", 1.5f },
        { "c8", 1.5f },
        { "c9", 2f },
    };

    private void Start()
    {
        SkillCardList = CardHolder.Instance.cards;
    }

    public void PlaySkill()
    {
        var skillList = Skills();
        foreach (var s in skillList)
        {
            if (CheckCondition(s, s.skillPosCondition, s.activateCondition))
            {
                CalculatePoint(s, s.skillPosCondition, s.skillEffect, s.activateCondition);
            }
            else
            {
                Debug.Log($"{s.name} khong dat dieu kien");
            }
        }
    }

    public bool CheckCondition(Skill current, List<SkillCondition> skillConditions, string id)
    {
        var skills = GetSkillsBasedOnSkillCondition(current, skillConditions);
        switch (id)
        {
            case "c1":
                { 
                    foreach (var s in skills)
                    {
                        if (s == null)
                            continue;
                        if (s.diceFace != null)
                            return true;
                    }
                } return false;
            case "c2":
                {
                    int same = -1;
                    foreach (var s in skills)
                    {
                        if (s == null)
                            return false;
                        if (s.diceFace == null)
                            return false;
                        if (s.diceFace.currentFace != same && same != -1)
                        {
                            return false;
                        }
                        same = s.diceFace.currentFace;
                    }
                } return true;
        }
        return false;
    }
    public void CalculatePoint(Skill current, List<SkillCondition> skillConditions, string id, string condtion)
    {
        var skills = GetSkillsBasedOnSkillCondition(current, skillConditions);
        float finalPoint = 0;
        switch (id)
        {
            case "e1":
                if (current.diceFace != null)
                    finalPoint = current.diceFace.currentFace;
                break;
            case "e3":
                if (current.diceFace != null)
                    finalPoint = current.diceFace.currentFace * 2;
                break;
        }
        float mulCon = conditionMultiplier[condtion];
        finalPoint *= mulCon * CalculateMul(skillConditions);

        Debug.Log($"{current.name} diem la: {finalPoint}");
    }

    private List<Skill> GetSkillsBasedOnSkillCondition(Skill skill, List<SkillCondition> skillConditions)
    {
        List<Skill> result = new();
        var skillList = Skills();

        foreach (SkillCondition condition in skillConditions)
        {
            Skill skillToAdd = null; // Temporary variable to hold the skill to add

            switch (condition)
            {
                case SkillCondition.Current:
                    skillToAdd = skill;
                    break;
                case SkillCondition.Left:
                    int indexL = skillList.IndexOf(skill);
                    if (indexL > 0)
                    {
                        skillToAdd = skillList[indexL - 1];
                    }
                    break;
                case SkillCondition.Right:
                    int indexR = skillList.IndexOf(skill);
                    if (indexR < 4)
                    {
                        skillToAdd = skillList[indexR + 1];
                    }
                    break;
                case SkillCondition.First:
                    skillToAdd = SkillCardList[0].GetComponent<Skill>();
                    break;
                case SkillCondition.Second:
                    skillToAdd = SkillCardList[1].GetComponent<Skill>();
                    break;
                case SkillCondition.Third:
                    skillToAdd = SkillCardList[2].GetComponent<Skill>();
                    break;
                case SkillCondition.Fourth:
                    skillToAdd = SkillCardList[3].GetComponent<Skill>();
                    break;
                case SkillCondition.Fifth:
                    skillToAdd = SkillCardList[4].GetComponent<Skill>();
                    break;
                default:
                    break;
            }

            // Add the skill only if it's not null and not already in the result list
            if (!result.Contains(skillToAdd))
            {
                result.Add(skillToAdd);
            }
        }

        return result;
    }

    public float CalculateMul(List<SkillCondition> skillConditions)
    {
        return 1 + (skillConditions.Count - 1) * 0.2f;
    }
}
public enum SkillCondition
{
    Current,
    Left,
    Right,
    First,
    Second,
    Third,
    Fourth,
    Fifth,
}
