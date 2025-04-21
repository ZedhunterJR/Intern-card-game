using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    public int diceFace = -1;

    public List<SkillCondition> skillPosCondition;
    public Func<List<Skill>, bool> activateCondition = (skill) =>
    {
        var sum = 0; 
        foreach (var sk in skill)
        {
            sum += sk.diceFace;
        }
        if (sum > 5)
            return true;
        return false;
    };


    public Action<List<Skill>> skillEffect = (skill) =>
    {
        
    };

    public void ActivateSkill()
    {

        List<Skill> skillList = new();
        foreach (SkillCondition skillCondition in skillPosCondition)
        {
            var sk = SkillManager.Instance.SkillBaseOnCondition(this, skillCondition);
            skillList.Add(sk);
        }

        if (activateCondition != null)
        {
            if (activateCondition(skillList))
            {
                skillEffect?.Invoke(skillList);
            }
        }
    }
}

