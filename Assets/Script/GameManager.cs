using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    public void TestBasicDeck(List<GameObject> listSkill)
    {
        int count = 0;
        foreach (var item in listSkill)
        {
            var skill = item.GetComponent<Skill>();
            if (count < 4)
            {
                skill.skillPosCondition.Add(SkillCondition.Current);
                skill.activateCondition = "c1";
                skill.skillEffect = "e1";
                count++;
            }
            else
            {
                skill.skillPosCondition.Add(SkillCondition.Left);
                skill.skillPosCondition.Add(SkillCondition.Right);
                skill.activateCondition = "c2";
                skill.skillEffect = "e3";
                item.GetComponent<Image>().color = Color.red;
            }
        }
    }
}
