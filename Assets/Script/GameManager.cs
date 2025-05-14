using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using TMPro;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] GameStatus gameStatus;

    public Dictionary<(Skill, string), Action> startRoundActionHelpers = new();
    public int maxNumOfTurn, maxNumOfReroll;
    public int CurrentNumOfTurn { get; private set; }
    public int CurrentNumOfReroll { get; private set; }
    public void SetTurns(int value = -1, bool set = false)
    {
        if (!set)
            CurrentNumOfTurn += value;
        else
            CurrentNumOfTurn = value;
        turnsNum.text = CurrentNumOfTurn.ToString();
    }
    public void SetRerolls(int value = -1, bool set = false)
    {
        if (!set)
            CurrentNumOfReroll += value;
        else
            CurrentNumOfReroll = value;
        rerollsNum.text = CurrentNumOfReroll.ToString();
    }

    [SerializeField] TextMeshProUGUI turnsNum;
    [SerializeField] TextMeshProUGUI rerollsNum;
    public GameStatus GameStatus
    {
        get { return gameStatus; }
        set 
        { 
            gameStatus = value;
            switch (value)
            {
                case GameStatus.Init:
                    //temp
                    gameStatus = GameStatus.Battle;
                    break;
                case GameStatus.Pause:
                    break;
                case GameStatus.Battle:
                    foreach (var helper in startRoundActionHelpers)
                    {
                        helper.Value?.Invoke();
                    }
                    SetTurns(maxNumOfTurn, true);
                    SetRerolls(maxNumOfReroll, true);
                    DiceManager.Instance.StartTurn();
                    break;
                case GameStatus.Shop:
                    ShopOpen();
                    break;
                case GameStatus.Lose:
                    Debug.Log("Thua");
                    SkillManager.Instance.EnemyTest.EnemyState = EnemyState.Attack;
                    break;
            }
        }
    }
    public RectTransform shopRect;
    public RectTransform iventoryRect;
    DataSpriteManager dataSpriteManager;

    private bool isShopOpen = false;
    private void Awake()
    {
        gameStatus = GameStatus.Init;
        dataSpriteManager = DataSpriteManager.Instance;
    }

    private void Start()
    {
        
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
                skill.ChangeEffect(Global.Instance.Effects["e1"], dataSpriteManager.EffectSprites["e1"]);
                count++;
            }
            else
            {
                //skill.ChangeSkillPosCondition(new() { SkillCondition.Left, SkillCondition.Right });
                //skill.ChangeActivateCondition(Global.Instance.Conditions["c2"]);
                skill.ChangeEffect(Global.Instance.Effects["e10"], dataSpriteManager.EffectSprites["e10"]);
            }
        }
    }

    public void ShopOpen()
    {
        shopRect.DOAnchorPos(new Vector2(12, 95), 0.5f).SetEase(Ease.OutCubic);

        ShopManager.Instance.Restock();
    }

    public void ShopClose()
    {
        shopRect.DOAnchorPos(new Vector2(12, 500f), 0.5f).SetEase(Ease.OutCubic);
        gameStatus = GameStatus.Battle;

        SetTurns(maxNumOfTurn, true);
        SetRerolls(maxNumOfReroll, true);

        SkillManager.Instance.EnemyTest.Init();
        //SkillManager.Instance.ReturnDicesToHolderAfterPlayed();
    }
    
}

public enum GameStatus
{
    Init,
    Pause, 
    Battle, 
    Shop, 
    Lose,
}
