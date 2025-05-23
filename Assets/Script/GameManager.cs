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
    public Skill NoSkill = null;

    public int currentRound = 0;
    public Dictionary<(Skill, string), Action> startRoundActionHelpers = new();
    public bool CanInteract => (SkillManager.Instance.canPlay && gameStatus == GameStatus.Battle) || gameStatus == GameStatus.Shop;
    public int turnBeforeEnemyFirstAttack, maxNumOfReroll;
    public int CurrentTurnBeforeEnemyAttack { get; private set; }
    public int CurrentNumOfReroll { get; private set; }
    public void SetTurns(int value = -1, bool set = false)
    {
        if (!set)
            CurrentTurnBeforeEnemyAttack += value;
        else
            CurrentTurnBeforeEnemyAttack = value;
        turnsNum.text = CurrentTurnBeforeEnemyAttack.ToString();
    }
    public void SetRerolls(int value = -1, bool set = false)
    {
        if (!set)
            CurrentNumOfReroll += value;
        else
            CurrentNumOfReroll = value;
        rerollsNum.text = CurrentNumOfReroll.ToString();
    }
    public float currentGold;
    /// <summary>
    /// current += value;
    /// To buy, check ModifyGold(-price). If false, that means there is not enough gold
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool ModifyGold(float value)
    {
        if (currentGold + value < 0)
            return false;
        currentGold += value;
        //update text here ->

        return true;
    }
    public float currentHp, maxHp;
    [SerializeField] List<Sprite> hpSprite = new();
    [SerializeField] Image hpImage;
    [SerializeField] TextMeshProUGUI hpText;
    /// <summary>
    /// if return false, that means hp = 0 -> should be gameover
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool UpdateHp(float value)
    {
        currentHp += value;
        currentHp = Mathf.Clamp(currentHp, 0, maxHp);

        FlashHpImage(() => UpdateHpImage());
        //update text here ->

        if (currentHp == 0)
        {
            ChangeGameStatus(GameStatus.Lose);
            return false;
        }
        return true;
    }
    void UpdateHpImage()
    {
        float hpPercent = (float)currentHp / maxHp * 100f;

        if (hpPercent <= 0f)
        {
            hpImage.gameObject.SetActive(false);
            return;
        }
        else
        {
            hpImage.gameObject.SetActive(true);
        }

        if (hpPercent > 80f)
            hpImage.sprite = hpSprite[0];
        else if (hpPercent > 60f)
            hpImage.sprite = hpSprite[1];
        else if (hpPercent > 40f)
            hpImage.sprite = hpSprite[2];
        else if (hpPercent > 20f)
            hpImage.sprite = hpSprite[3];
        else
            hpImage.sprite = hpSprite[4];

        hpText.text = $"{(int)currentHp}/{(int)maxHp}";
    }

    void FlashHpImage(System.Action onComplete)
    {
        float originalAlpha = hpImage.color.a;
        float flashAlpha = 0.3f; // độ mờ, có thể tùy chỉnh

        Sequence seq = DOTween.Sequence();

        for (int i = 0; i < 3; i++)
        {
            seq.Append(hpImage.DOFade(flashAlpha, 0.1f));  // Làm mờ
            seq.Append(hpImage.DOFade(originalAlpha, 0.1f)); // Hiện rõ lại
        }

        seq.OnComplete(() => onComplete?.Invoke());
    }

    [SerializeField] TextMeshProUGUI turnsNum;
    [SerializeField] TextMeshProUGUI rerollsNum;
    public GameStatus GameStatus => gameStatus;
    
    public RectTransform shopRect;
    public RectTransform inforRect;
    DataSpriteManager dataSpriteManager;

    private bool isShopOpen = false;
    private void Awake()
    {
        ChangeGameStatus(GameStatus.Init);
        dataSpriteManager = DataSpriteManager.Instance;
    }

    private void Start()
    {
        ChangeGameStatus(GameStatus.Battle);
    }

    private void Update()
    {
            
    }

    public void ChangeGameStatus(GameStatus gameStatus)
    {
        var tempStatus = this.gameStatus;
        if (this.gameStatus != gameStatus)
        {
            this.gameStatus = gameStatus;
            switch (gameStatus)
            {
                case GameStatus.Init:
                    UpdateHpImage();
                    break;
                case GameStatus.Pause:
                    break;
                case GameStatus.Battle:
                    if (tempStatus != GameStatus.Pause)
                    {
                        foreach (var helper in startRoundActionHelpers)
                        {
                            helper.Value?.Invoke();
                        }
                        SetRerolls(maxNumOfReroll, true);
                        DiceManager.Instance.currentDiceNum = DiceManager.Instance.baseDiceNum;
                        DiceManager.Instance.StartTurn();
                        AttackSequence.Instance.ResetNewTurn();
                        EnemyManager.Instance.SpawnEnemy(currentRound);
                        SetTurns(turnBeforeEnemyFirstAttack + EnemyManager.Instance.currentEnemy.attackInterval, true);
                        currentRound++;
                    }
                    break;
                case GameStatus.Shop:
                    ShopOpen();
                    break;
                case GameStatus.Lose:
                    Debug.Log("Thua");
                    //SkillManager.Instance.EnemyTest.EnemyState = EnemyState.Attack;
                    break;
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
                skill.ChangeEffect(Global.Instance.Effects["e1"], dataSpriteManager.GetSpriteCard("e1"));
                count++;
            }
            else
            {
                //skill.ChangeSkillPosCondition(new() { SkillCondition.Left, SkillCondition.Right });
                //skill.ChangeActivateCondition(Global.Instance.Conditions["c2"]);
                skill.ChangeEffect(Global.Instance.Effects["e2"], dataSpriteManager.GetSpriteCard("e2"));
            }
        }
    }

    public void ShopOpen()
    {
        shopRect.DOAnchorPos(new Vector2(12, 95), 0.5f).SetEase(Ease.OutCubic);

        ShopManager.Instance.Restock();
        ShopManager.Instance.RelicItem();
    }

    public void ShopClose()
    {
        shopRect.DOAnchorPos(new Vector2(12, 500f), 0.5f).SetEase(Ease.OutCubic);
        ChangeGameStatus(GameStatus.Battle);
    }
    
    public void InforOpen()
    {
        inforRect.transform.position = Vector2.zero;
        inforRect.transform.localScale = Vector2.zero;
        inforRect.DOScale(Vector2.one, 0.5f).SetEase(Ease.InCubic);
        ChangeGameStatus(GameStatus.Pause);
    }

    public void InforClose()
    {
        inforRect.DOAnchorPos(new Vector2(0, 600f), 0.5f).SetEase(Ease.OutCubic);
        ChangeGameStatus (GameStatus.Battle);
    }
}

public enum GameStatus
{
    None,
    Init,
    Pause, 
    Battle, 
    Shop, 
    Lose,
}
