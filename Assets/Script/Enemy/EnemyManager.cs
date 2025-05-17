using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class EnemyManager : Singleton<EnemyManager>
{
    [SerializeField] private List<RuntimeAnimatorController> bossAnim = new();
    [SerializeField] private List<RuntimeAnimatorController> enemyTier1 = new();
    [SerializeField] private List<RuntimeAnimatorController> enemyTier2 = new();

    [SerializeField] private GameObject enemyPlace;
    [SerializeField] private Transform hp_bar;
    [SerializeField] private TextMeshProUGUI enemyText, roundReq;
    public RuntimeAnimatorController currentEnemy;
    [HideInInspector] public float enemyMaxHp, enemyCurrentHp;
    private void Start()
    {
        
    }

    public void SpawnEnemy(int round)
    {
        currentEnemy = GetNextEnemy(round);
        enemyPlace.GetComponent<Animator>().runtimeAnimatorController = currentEnemy;
        enemyPlace.GetComponent<Animator>().Play("idle");
        enemyPlace.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 0);
        enemyPlace.GetComponent<SpriteRenderer>().DOFade(1, 1);

        enemyMaxHp = RoundRequirement(round);
        OnSpawnEnemyEvent();
        enemyText.text = currentEnemy.name;
        roundReq.text = enemyMaxHp.ToString();

        enemyCurrentHp = enemyMaxHp;
        hp_bar.transform.localScale = new Vector3(0, 1, 1);
        hp_bar.transform.DOScale(Vector3.one, 1f);
    }
    /// <summary>
    /// 0: not dead
    /// 1: dead
    /// 2: already dead, return
    /// </summary>
    /// <param name="dmg"></param>
    /// <returns></returns>
    public int TakeDamage(float dmg)
    {
        if (enemyCurrentHp == 0)
            return 2;

        //hurt anim ->

        enemyCurrentHp -= dmg;
        enemyCurrentHp = Mathf.Clamp(enemyCurrentHp, 0, enemyMaxHp);
        hp_bar.transform.localScale = new Vector3(enemyCurrentHp / enemyMaxHp, 1, 1);

        OnTakeDamage();
        if (enemyCurrentHp == 0)
        {
            OnKillEnemyEvent();
        }
        if (enemyCurrentHp == 0)
        {
            enemyPlace.GetComponent<Animator>().Play("die");
            this.Invoke(() =>
            {
                GameManager.Instance.ChangeGameStatus(GameStatus.Shop);
            }, 2f);
            return 1;
        }

        if (GameManager.Instance.CurrentNumOfTurn == 0 && enemyCurrentHp > 0)
        {
            enemyPlace.GetComponent<Animator>().Play("attack");
            GameManager.Instance.ChangeGameStatus(GameStatus.Lose);
        }

        return 0;
    }
    public void OnSpawnEnemyEvent()
    {

    }
    public void OnKillEnemyEvent()
    {

    }
    public void OnTakeDamage()
    {

    }

    /// <summary>
    /// Maybe enemy should only appear once
    /// </summary>
    /// <param name="round"></param>
    /// <returns></returns>
    private RuntimeAnimatorController GetNextEnemy(int round)
    {
        switch (round)
        {
            case 0: return enemyTier1.GetRandom();
            case 1: return enemyTier1.GetRandom();
            case 2: return bossAnim[0];
            case 3: return enemyTier1.GetRandom();
            case 4: return enemyTier2.GetRandom();
            case 5: return bossAnim[1];
            case 6: return enemyTier2.GetRandom();
            case 7: return enemyTier2.GetRandom();
            case 8: return bossAnim[2];
        }
        return enemyTier1.GetRandom();
    }
    public float RoundRequirement(int round)
    {
        switch (round)
        {
            case 0: return 50f;
            case 1: return 70f;
            case 2: return 120f;
            case 3: return 150f;
            case 4: return 200f;
            case 5: return 300f;
            case 6: return 380f;
            case 7: return 500f;
            case 8: return 1000f;
        }
        return 50f;
    }
}
