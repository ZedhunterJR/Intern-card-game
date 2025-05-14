using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using System.Text.RegularExpressions;

public class Skill : MonoBehaviour
{
    [SerializeField] private Transform dicePlace;

    [HideInInspector] public Dice diceFace;
    [HideInInspector] public EffectClause skillEffect;

    [HideInInspector] public float cardMultiplier = 1f;
    [HideInInspector] public bool isDisabled = false;

    [SerializeField] private TextMeshProUGUI cardName;
    [SerializeField] private TextMeshProUGUI cardDes;

    //for specific card effect
    [HideInInspector] public float v0, v1, v2;
    [HideInInspector] public string s0, s1, s2;

    [SerializeField] private Image cardImage;
    public GameObject informationPanel;

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

    public void ChangeEffect(EffectClause effect, Sprite effectSprite = null)
    {
        SkillManager.Instance.RemoveEffectFromSkill(this, skillEffect);
        skillEffect = effect;
        SkillManager.Instance.AddEffectToSkill(this, skillEffect);

        var mainImage = GetComponent<Image>();
        mainImage.material = null; // Reset to default
        Material runtimeHueMaterialMain = new Material(Shader.Find("Custom/UIHueFromColor"));
        runtimeHueMaterialMain.color = skillEffect.color; // Now this will affect `_Color`
        mainImage.material = runtimeHueMaterialMain; // Apply new instance   

        cardName.text = effect.name;

        // Thay thế các [v0], [v1], [v2], [s0], [s1], [s2] bằng giá trị tương ứng
        string description = FormatSkillDescription(effect.description);

        cardDes.text = description;
        cardImage.sprite = effectSprite;
    }

    public string FormatSkillDescription(string rawDescription)
    {
        // Mã màu vàng sẫm
        const string valueColor = "#986801";

        return Regex.Replace(rawDescription, @"\[(v[0-2]|s[0-2])\]", match =>
        {
            string token = match.Groups[1].Value;
            string replacement = token switch
            {
                "v0" => v0.ToString(),
                "v1" => v1.ToString(),
                "v2" => v2.ToString(),
                "s0" => s0,
                "s1" => s1,
                "s2" => s2,
                _ => token
            };

            // Giữ lại ngoặc vuông, và bọc số/chuỗi trong thẻ màu
            return $"[<color={valueColor}>{replacement}</color>]";
        });
    }

    public void UpdatePanelInfor()
    {
        var des = FormatSkillDescription(skillEffect.description);
        cardDes.text = des;
    }
}

