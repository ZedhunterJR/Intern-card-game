using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Unity.VisualScripting;

public class GameManager : Singleton<GameManager>
{
    public RectTransform battleRect; 
    public RectTransform shopRect;
    public RectTransform iventoryRect;

    private bool isShopOpen = false;
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.E))
        {
            if (!isShopOpen)
            {
                isShopOpen = true;
                ShopOpen();
            }
            else
            {
                isShopOpen = false;
                ShopClose();
            }    
        } 
            
    }
    public void TestBasicDeck(List<GameObject> listSkill)
    {
        int count = 0;
        foreach (var item in listSkill)
        {
            var skill = item.GetComponent<Skill>();
            if (count < 4)
            {
                skill.ChangeSkillPosCondition(new() { SkillCondition.Fourth, SkillCondition.Fifth });
                skill.ChangeActivateCondition(Global.Instance.Conditions["c1"]);
                skill.ChangeEffect(Global.Instance.Effects["e1"]);
                count++;
            }
            else
            {
                skill.ChangeSkillPosCondition(new() { SkillCondition.Left, SkillCondition.Right });
                skill.ChangeActivateCondition(Global.Instance.Conditions["c2"]);
                skill.ChangeEffect(Global.Instance.Effects["e3"]);
            }
        }
    }

    public void ShopOpen()
    {
        battleRect.DOAnchorPos(new Vector2(0, -600f), 0.5f).SetEase(Ease.OutCubic);

        shopRect.DOAnchorPos(Vector2.zero, 0.5f).SetEase(Ease.OutCubic);

        ShopManager.Instance.Restock();
    }

    public void ShopClose()
    {
        shopRect.DOAnchorPos(new Vector2(0, 600f), 0.5f).SetEase(Ease.OutCubic);
        
        battleRect.DOAnchorPos(Vector2.zero, 0.5f).SetEase(Ease.OutCubic);
    }
}
