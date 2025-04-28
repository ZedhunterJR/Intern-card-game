using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class IventoryManager : Singleton<IventoryManager>
{
    [SerializeField] List<Skill> arraySkill;
    public RectTransform ivenRect;

    public void OpenIventory(bool isOpen, CardPartType type = CardPartType.None)
    {
        if (isOpen)
        {
            UpdateInventory(type);
            ivenRect.DOAnchorPos(Vector2.zero, 0.5f);
        }
        else
        {
            ReverseUpdateSkill();
            ivenRect.DOAnchorPos(new Vector2(0, 600f), 0.5f);
        }
    }

    public void UpdateInventory(CardPartType type = CardPartType.None)
    {
        var skillRef = SkillManager.Instance.Skills();
        for (int i = 0; i < skillRef.Count; i++)
        {
            arraySkill[i].ChangeSkillPosCondition(skillRef[i].skillPosCondition);
            arraySkill[i].ChangeActivateCondition(skillRef[i].activateCondition);
            arraySkill[i].ChangeEffect(skillRef[i].skillEffect);

            arraySkill[i].transform.Find("pos_outline").gameObject.SetActive(false);
            arraySkill[i].transform.Find("con_outline").gameObject.SetActive(false);
            arraySkill[i].transform.Find("effect_outline").gameObject.SetActive(false);
            switch (type)
            {
                case CardPartType.Position:
                    arraySkill[i].transform.Find("pos_outline").gameObject.SetActive(true);
                    break;
                case CardPartType.Condition:
                    arraySkill[i].transform.Find("con_outline").gameObject.SetActive(true);
                    break;
                case CardPartType.Effect:
                    arraySkill[i].transform.Find("effect_outline").gameObject.SetActive(true);
                    break;
            }
        }
    }
    public void ReverseUpdateSkill()
    {
        var skillRef = SkillManager.Instance.Skills();
        for (int i = 0; i < skillRef.Count; i++)
        {
            skillRef[i].ChangeSkillPosCondition(arraySkill[i].skillPosCondition);
            skillRef[i].ChangeActivateCondition(arraySkill[i].activateCondition);
            skillRef[i].ChangeEffect(arraySkill[i].skillEffect);
        }
    }
}

public enum CardPartType
{
    None,
    Position,
    Condition,
    Effect,
}

