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
    [SerializeField] private Sprite[] positionArrowSprites;
    [SerializeField] private string[] positionArrowHelpers;
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
            float conPos = activateCondition != null? activateCondition.multiplier : 1f;
            return multPos * conPos * cardMultiplier;
        } 
    }

    //private
    private Dictionary<string, Sprite> posArrowDict = new Dictionary<string, Sprite>();

    private void Awake()
    {
        for (int i = 0; i < positionArrowHelpers.Length; i++)
        {
            posArrowDict[positionArrowHelpers[i]] = positionArrowSprites[i];
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
    public void ChangeSkillPosCondition(List<SkillCondition> positions)
    {
        foreach (var item in positionArrows)
        {
            item.sprite = posArrowDict["blank"];
        }
        List<int> taken = new List<int>() { 0, 1, 2, 3, 4 };
        skillPosCondition = positions;
        if (skillPosCondition.Contains(SkillCondition.Left))
        {
            positionArrows[0].sprite = posArrowDict["left"];
            taken.Remove(0);
        }
        if (skillPosCondition.Contains(SkillCondition.Right))
        {
            positionArrows[4].sprite = posArrowDict["right"];
            taken.Remove(4);
        }
        if (skillPosCondition.Contains(SkillCondition.Current))
        {
            positionArrows[2].sprite = posArrowDict["this"];
            taken.Remove(2);
        }
        if (skillPosCondition.Contains(SkillCondition.First))
        {
            int avai = taken[0];
            positionArrows[avai].sprite = posArrowDict["1st"];
            taken.Remove(avai);
        }
        if (skillPosCondition.Contains(SkillCondition.Second))
        {
            int avai = taken[0];
            positionArrows[avai].sprite = posArrowDict["2nd"];
            taken.Remove(avai);
        }
        if (skillPosCondition.Contains(SkillCondition.Third))
        {
            int avai = taken[0];
            positionArrows[avai].sprite = posArrowDict["3rd"];
            taken.Remove(avai);
        }
        if (skillPosCondition.Contains(SkillCondition.Fourth))
        {
            int avai = taken[0];
            positionArrows[avai].sprite = posArrowDict["4th"];
            taken.Remove(avai);
        }
        if (skillPosCondition.Contains(SkillCondition.Fifth))
        {
            int avai = taken[0];
            positionArrows[avai].sprite = posArrowDict["5th"];
            taken.Remove(avai);
        }

        UpdateMultPanel();
    }
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
    #endregion
}

