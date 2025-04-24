using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Skill : MonoBehaviour
{
    [SerializeField] private Transform dicePlace;

    public Dice diceFace;

    public List<SkillCondition> skillPosCondition;
    public string activateCondition;

    public string skillEffect;
    public void AddDice(Dice dice)
    {
        dice.transform.SetParent(dicePlace);
        dice.transform.localScale = Vector3.one;
        dice.transform.localRotation = Quaternion.identity;
        //var diceTransfrom = dice.GetComponent<RectTransform>();
        //DOTween.To(() => diceTransfrom.offsetMin, x => diceTransfrom.offsetMin = x, Vector2.zero, 0.1f);
        //DOTween.To(() => diceTransfrom.offsetMax, x => diceTransfrom.offsetMax = x, Vector2.zero, 0.1f);
        diceFace = dice;
    }
}

