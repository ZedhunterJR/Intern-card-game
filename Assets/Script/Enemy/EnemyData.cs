using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class EnemyData : ScriptableObject
{
    public string enemyName;
    public RuntimeAnimatorController animCon;
    public float baseHp;
    public float dmg;
    public int attackInterval;
}
