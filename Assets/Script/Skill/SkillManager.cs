using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : Singleton<SkillManager>
{
    public List<Skill> SkillList;
    public Skill SkillBaseOnCondition(Skill skill, SkillCondition condition)
    {
        switch (condition)
        {
            case SkillCondition.Current: return skill;
        }
        return skill;
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
