using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;

public class Skill : MonoBehaviour
{
    [SerializeField] private Transform dicePlace;
    [SerializeField] private Transform multPanel;
    [SerializeField] private Image[] positionArrows;

    [SerializeField] private Transform conditionPanel;
    [SerializeField] private Transform effectPanel;

    [HideInInspector] public Dice diceFace;
    [HideInInspector] public List<SkillCondition> skillPosCondition = new();
    [HideInInspector] public ConditionClause activateCondition;
    [HideInInspector] public EffectClause skillEffect;

    [HideInInspector] public float cardMultiplier = 1f;
    public float Multipler
    {
        get
        {
            float multPos = Global.Instance.CalculatePosMul(skillPosCondition);
            float conPos = activateCondition != null ? activateCondition.multiplier : 1f;
            return multPos * conPos * cardMultiplier;
        }
    }


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

    #region graphic

    public void ChangeActivateCondition(ConditionClause con)
    {
        activateCondition = con;

        var image = conditionPanel.GetComponent<Image>();
        image.material = null; // Reset to default
        Material runtimeHueMaterial = new Material(Shader.Find("Custom/UIHueFromColor"));
        runtimeHueMaterial.color = activateCondition.color;
        image.material = runtimeHueMaterial; // Apply new instance

        conditionPanel.GetComponentInChildren<TextMeshProUGUI>().text = activateCondition.description;

        UpdateMultPanel();
    }
    public void ChangeEffect(EffectClause effect)
    {
        skillEffect = effect;

        var image = effectPanel.GetComponent<Image>();
        image.material = null; // Reset to default
        Material runtimeHueMaterial = new Material(Shader.Find("Custom/UIHueFromColor"));
        runtimeHueMaterial.color = skillEffect.color;
        image.material = runtimeHueMaterial; // Apply new instance

        var mainImage = GetComponent<Image>();
        mainImage.material = null; // Reset to default
        Material runtimeHueMaterialMain = new Material(Shader.Find("Custom/UIHueFromColor"));
        runtimeHueMaterialMain.color = skillEffect.color; // Now this will affect `_Color`
        mainImage.material = runtimeHueMaterialMain; // Apply new instance   

        effectPanel.GetComponentInChildren<TextMeshProUGUI>().text = skillEffect.description;
    }

    public void UpdateMultPanel()
    {
        multPanel.GetComponentInChildren<TextMeshProUGUI>().text = Multipler.DecimalFormat(2, 1) + "x";
    }
    public void ChangeSkillPosCondition(List<SkillCondition> conditions)
    {
        Global.Instance.UpdatePositionArrowGraphic(conditions, positionArrows);
        skillPosCondition = conditions;
        UpdateMultPanel();
    }

    public void ReUpdate()
    {
        ChangeSkillPosCondition(skillPosCondition);
        ChangeActivateCondition(activateCondition);
        ChangeEffect(skillEffect);
    }
    #endregion
}

