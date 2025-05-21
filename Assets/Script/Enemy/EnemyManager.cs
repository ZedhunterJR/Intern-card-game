using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class EnemyManager : Singleton<EnemyManager>
{
    [SerializeField] private List<EnemyData> bosses = new();
    [SerializeField] private List<EnemyData> enemyTier1 = new();
    [SerializeField] private List<EnemyData> enemyTier2 = new();

    [SerializeField] private GameObject enemyPlace;
    [SerializeField] private Transform hp_bar;
    [SerializeField] private TextMeshProUGUI enemyText, roundReq;
    public EnemyData currentEnemy;
    [HideInInspector] public float enemyMaxHp, enemyCurrentHp;
    private void Start()
    {
        
    }

    public void SpawnEnemy(int round)
    {
        currentEnemy = GetNextEnemy(round);
        enemyPlace.GetComponent<Animator>().runtimeAnimatorController = currentEnemy.animCon;
        enemyPlace.GetComponent<Animator>().Play("idle");
        enemyPlace.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 0);
        enemyPlace.GetComponent<SpriteRenderer>().DOFade(1, 1);

        enemyMaxHp = currentEnemy.baseHp * RoundRequirement(round);
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

        if (GameManager.Instance.CurrentTurnBeforeEnemyAttack == 0)
        {
            enemyPlace.GetComponent<Animator>().Play("attack");
            GameManager.Instance.SetTurns(currentEnemy.attackInterval, true);

            if (!GameManager.Instance.UpdateHp(-currentEnemy.dmg))
            {
                return 1;
            }
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
    private EnemyData GetNextEnemy(int round)
    {
        switch (round)
        {
            case 0: return enemyTier1.GetRandom();
            case 1: return enemyTier1.GetRandom();
            case 2: return enemyTier1.GetRandom();
            case 3: return bosses[0];
            case 4: return enemyTier1.GetRandom();
            case 5: return enemyTier2.GetRandom();
            case 6: return enemyTier2.GetRandom();
            case 7: return bosses[1];
            case 8: return enemyTier2.GetRandom();
            case 9: return enemyTier2.GetRandom();
            case 10: return enemyTier2.GetRandom();
            case 11: return bosses[2];
        }
        return enemyTier1.GetRandom();
    }
    public float RoundRequirement(int round)
    {
        switch (round)
        {
            case 0: return 1f;
            case 1: return 1.5f;
            case 2: return 1.8f;
            case 3: return 3f;
            case 4: return 2.5f;
            case 5: return 3f;
            case 6: return 3.8f;
            case 7: return 5f;
            case 8: return 4f;
            case 9: return 5f;
            case 10: return 6f;
            case 11: return 10f;
        }
        return 1f;
    }
}
