using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Unity.VisualScripting;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] GameStatus gameStatus;
    public GameStatus GameStatus
    {
        get { return gameStatus; }
        set 
        { 
            gameStatus = value;
            switch (value)
            {
                case GameStatus.Init:
                    break;
                case GameStatus.Pause:
                    break;
                case GameStatus.Battle:
                    break;
                case GameStatus.Shop:
                    ShopOpen();
                    break;
                default:
                    break;
            }
        }
    }
    public RectTransform shopRect;
    public RectTransform iventoryRect;

    private bool isShopOpen = false;
    private void Awake()
    {
        gameStatus = GameStatus.Init;
    }

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
                //skill.ChangeSkillPosCondition(new() { SkillCondition.Fourth, SkillCondition.Fifth });
                //skill.ChangeActivateCondition(Global.Instance.Conditions["c1"]);
                skill.ChangeEffect(Global.Instance.Effects["e1"]);
                count++;
            }
            else
            {
                //skill.ChangeSkillPosCondition(new() { SkillCondition.Left, SkillCondition.Right });
                //skill.ChangeActivateCondition(Global.Instance.Conditions["c2"]);
                skill.ChangeEffect(Global.Instance.Effects["e3"]);
            }
        }
    }

    public void ShopOpen()
    {
        shopRect.DOAnchorPos(new Vector2(15, 130), 0.5f).SetEase(Ease.OutCubic);

        ShopManager.Instance.Restock();
    }

    public void ShopClose()
    {
        shopRect.DOAnchorPos(new Vector2(15, 500f), 0.5f).SetEase(Ease.OutCubic);
        gameStatus = GameStatus.Battle;
        SkillManager.Instance.EnemyTest.Init();
    }
}

public enum GameStatus
{
    Init,
    Pause, 
    Battle, 
    Shop, 
}
