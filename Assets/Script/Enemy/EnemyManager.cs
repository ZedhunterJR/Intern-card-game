using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using System;

public class EnemyManager : Singleton<EnemyManager>
{
    [SerializeField] private List<EnemyData> bosses = new();
    [SerializeField] private List<EnemyData> enemyTier1 = new();
    [SerializeField] private List<EnemyData> enemyTier2 = new();

    [SerializeField] private GameObject enemyPlace;
    [SerializeField] private Transform hp_bar;
    [SerializeField] private TextMeshProUGUI enemyText, attackDamage, attackInterval, enemyTextCurrentHP;
    public EnemyData currentEnemy;
    [HideInInspector] public float enemyMaxHp, enemyCurrentHp;

    public Dictionary<(Skill, string), Action<float>> enemyAttackActionHelpers = new();
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

        enemyCurrentHp = enemyMaxHp;
        enemyText.text = currentEnemy.enemyName;
        attackDamage.text = currentEnemy.dmg.ToString();
        enemyTextCurrentHP.text = $"{enemyCurrentHp}/{enemyMaxHp}";
        attackInterval.text = currentEnemy.attackInterval.ToString();

        hp_bar.transform.localScale = new Vector3(0, 1, 1);
        hp_bar.transform.DOScale(Vector3.one, 1f);
    }

    public void TakeDamage(float dmg)
    {
        if (enemyCurrentHp == 0)
            return;

        //hurt anim ->

        enemyCurrentHp -= dmg;
        enemyCurrentHp = Mathf.Clamp(enemyCurrentHp, 0, enemyMaxHp);
        hp_bar.transform.localScale = new Vector3(enemyCurrentHp / enemyMaxHp, 1, 1);
        UpdateInformationAfterPlay();

        OnTakeDamage();
        if (enemyCurrentHp == 0)
        {
            OnKillEnemyEvent();
        }

    }

    public void UpdateInformationAfterPlay()
    {
        enemyTextCurrentHP.text = $"{enemyCurrentHp.DecimalFormat(0)}/{enemyMaxHp}";
        attackInterval.text = GameManager.Instance.CurrentTurnBeforeEnemyAttack.ToString();
    }

    public bool Endturn()
    {
        if (enemyCurrentHp == 0)
        {
            enemyPlace.GetComponent<Animator>().Play("die");
            this.Invoke(() =>
            {
                GameManager.Instance.ChangeGameStatus(GameStatus.Shop);
            }, 2f);
            return true;
        }

        if (GameManager.Instance.CurrentTurnBeforeEnemyAttack == 0)
        {
            enemyPlace.GetComponent<Animator>().Play("attack");
            Pulse();
            GameManager.Instance.SetTurns(currentEnemy.attackInterval, true);
            var dead = !GameManager.Instance.UpdateHp(-currentEnemy.dmg);
            if (!dead) foreach (var item in enemyAttackActionHelpers.Values)
                    item?.Invoke(currentEnemy.dmg);

            return dead;
            //return true;
        }
        return false;
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

    private void Pulse()
    {
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.4f)
           .Append(Camera.main.DOColor(Color.red, 0.15f))
           .Append(Camera.main.DOColor(Color.black, 0.15f));
    }
}
