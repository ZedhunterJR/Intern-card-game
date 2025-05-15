using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyTest : MonoBehaviour
{
    [SerializeField] EnemyData enemyData;
    [SerializeField] EnemyState enemyState;
    [SerializeField] Image healthBar; 
    private float maxHealth;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public EnemyState EnemyState
    {
        get { return enemyState; }
        set
        {
            enemyState = value;
            switch (value)
            {
                case EnemyState.Idle:
                    animator.Play("Idle");
                    break;
                case EnemyState.Attack:
                    animator.Play("Attack");
                    break;
                case EnemyState.Die:
                    break;
            }
        }
    }

    public void Init()
    {
        enemyData = new EnemyData()
        {
            hp = 100
        };
        maxHealth = enemyData.hp;
        enemyState = EnemyState.Idle;
        healthBar.fillAmount = enemyData.hp / maxHealth;
    }

    public void Damage(float damage)
    {
        enemyData.hp -= damage;
        healthBar.fillAmount = enemyData.hp / maxHealth;
        if (enemyData.hp <= 0)
        {
            enemyState = EnemyState.Die;
            GameManager.Instance.ChangeGameStatus(GameStatus.Shop);
        }
        else
        {
            // kiem tra so lan danh bai <= 0 thi EnemyState = Attack
            if (GameManager.Instance.CurrentNumOfTurn <= 0)
            {
                GameManager.Instance.ChangeGameStatus(GameStatus.Lose);
            }
        }
    }
}

[System.Serializable]
public class EnemyData
{
    public float hp; 
}

public enum EnemyState
{
    Idle,
    Attack, 
    Die, 
}
