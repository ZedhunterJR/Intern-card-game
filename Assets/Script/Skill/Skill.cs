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

    [HideInInspector] public Dice diceFace;
    [HideInInspector] public EffectClause skillEffect;

    [HideInInspector] public float cardMultiplier = 1f;
    [HideInInspector] public bool isDisabled = false;

    [SerializeField] private TextMeshProUGUI cardName;
    [SerializeField] private TextMeshProUGUI cardDes;
    [SerializeField] private Image cardImage;
    public void SetDisable(bool disable)
    {
        isDisabled = disable;
        if (disable)
        {
            //animation to disable card and shit
        }
        else
        {
            //same but to enable card
        }
    }
    public float Multipler
    {
        get
        {
            return 1;
        }
    }


    public void AddDice(Dice dice)
    {
        dice.transform.SetParent(dicePlace);
        dice.transform.localScale = Vector3.one;
        dice.transform.localRotation = Quaternion.identity;
        diceFace = dice;
    }

    #region graphic
    public void ChangeEffect(EffectClause effect, Sprite effectSprite = null)
    {
        skillEffect = effect;

        var mainImage = GetComponent<Image>();
        mainImage.material = null; // Reset to default
        Material runtimeHueMaterialMain = new Material(Shader.Find("Custom/UIHueFromColor"));
        runtimeHueMaterialMain.color = skillEffect.color; // Now this will affect `_Color`
        mainImage.material = runtimeHueMaterialMain; // Apply new instance   

        cardName.text = effect.name;
        cardDes.text = effect.description;
        cardImage.sprite = effectSprite;
    }
    public void ReUpdate()
    {
    }

    // after player play card 
    public void ReturnDicesToHolder()
    {
        DiceManager.Instance.ReturnDice(diceFace);
    }
    #endregion
}

