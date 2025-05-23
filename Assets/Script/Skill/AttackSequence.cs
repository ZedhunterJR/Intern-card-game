using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using System.Net;

public class AttackSequence : Singleton<AttackSequence>
{
    [SerializeField] TextMeshProUGUI paternText;
    [SerializeField] TextMeshProUGUI pointText;
    [SerializeField] TextMeshProUGUI multText;
    [SerializeField] TextMeshProUGUI totalDamageText;
    [SerializeField] TextMeshProUGUI totalDamageRoundText;
    private float totalDamageValue = 0f;
    [SerializeField] Transform floatingHolder;
    [SerializeField] GameObject floatingPrefab;

    [SerializeField] Transform enemyPos;

    public Queue<VisualPointSeq> visualPointQueue;
    public List<Dice> dicesUse => SkillManager.Instance.diceInPlayed;

    private void Start()
    {
        visualPointQueue = SkillManager.Instance.visualPointQueue;
    }

    public IEnumerator CaculatePointSequence()
    {
        Debug.Log($"{visualPointQueue.Count}");
        var point = 0f;
        var mult = 1f;
        var dicePattern = DicePattern.None;
        var totalDamge = 0f;

        while (visualPointQueue.Count > 0)
        {
            var q = visualPointQueue.Dequeue();
            if (q.dicePattern != DicePattern.None)
            {
                if (dicePattern != q.dicePattern)
                {
                    var oldPoint = Global.Instance.GetPatternPoints(dicePattern);
                    var newPoint = Global.Instance.GetPatternPoints(q.dicePattern);
                    dicePattern = q.dicePattern;

                    point += newPoint - oldPoint;
                    paternText.text = q.dicePattern.ToString();
                }
            }
            else
            {
                float oldPoint = point;
                float oldMult = mult;

                point += q.point;
                pointText.text = point.ToString();
                mult += q.mult;
                multText.text = mult.ToString();

                TextEffectHelper.UpdateTextWithPunchEffect(pointText, oldPoint, point);
                TextEffectHelper.UpdateTextWithPunchEffect(multText, oldMult, mult);

                Vector3 spawnPos = Vector3.zero;
                float spawnPointValue = 0f;
                if (q.point != 0 || q.mult != 0)
                {
                    if (q.skill != null)
                    {
                        spawnPos = q.skill.transform.position;
                        spawnPos = new Vector3(spawnPos.x, spawnPos.y + 1f);
                    }
                    if (q.dice != null)
                    {
                        spawnPos = q.dice.transform.position;
                    }
                    spawnPointValue = q.point != 0 ? q.point : q.mult;
                }
                var text = Instantiate(floatingPrefab, spawnPos, Quaternion.identity, floatingHolder);
                text.GetComponent<TextMeshProUGUI>().text = $"+{spawnPointValue}";
                this.Invoke(() => Destroy(text), .5f);
            }
            yield return new WaitForSeconds(0.5f);
        }
        var oldTotalDamage = 0f;
        totalDamge = point * mult;
        TextEffectHelper.UpdateTextWithPunchEffect(totalDamageText, oldTotalDamage, totalDamge);
        Debug.Log($"Total Damage: {totalDamge}");
        StartCoroutine(DiceAttackSequence(totalDamge));
        yield return null;
    }

    private IEnumerator DiceAttackSequence(float totalDamage)
    {
        List<Dice> diceToLaunch = new List<Dice>();
        List<Dice> allDices = new(DiceManager.Instance.diceList);
        foreach (var dice in dicesUse)
        {
            if (dice == null) continue;
            if (dice.usedInAttack || dice.includedInPoint)
            {
                diceToLaunch.Add(dice);
                allDices.Remove(dice);
            }
        }

        //remove anim, add later
        foreach (var dice in allDices)
        {
            PoolingObject.Instance.ReturnDiceToPool(dice);
        }
        for (int i = 0; i < diceToLaunch.Count; i++)
        {
            var die = diceToLaunch[i]; // capture reference directly
            die.gameObject.transform.DOMove(enemyPos.position, 0.5f);

            yield return new WaitForSeconds(0.5f);EnemyManager.Instance.TakeDamage(totalDamage / diceToLaunch.Count);
            totalDamageValue += totalDamage / diceToLaunch.Count;

            PoolingObject.Instance.ReturnDiceToPool(die);
        }

        yield return new WaitForSeconds(1f);
        EnemyManager.Instance.Endturn();

        yield return new WaitForSeconds(1f);
        if (GameManager.Instance.currentHp > 0)
            DiceManager.Instance.StartTurn();
        ResetNewTurn();
    }

    public void UpdatePatternOnDicePlace()
    {
        var dices = SkillManager.Instance.PlayedDices(out var extra);
        var toDetect = new List<Dice>(extra);
        foreach (var dice in dices)
        {
            if (dice != null)
            {
                toDetect.Add(dice);
            }
        }
        DicePattern patt = SkillManager.Instance.DetectDicePattern(toDetect);

        paternText.text = patt.ToString();
        var point = Global.Instance.GetPatternPoints(patt);

        pointText.text = point.DecimalFormat(2);

        float oldPoint = 0;
        float.TryParse(pointText.text, out oldPoint);
        TextEffectHelper.UpdateTextWithPunchEffect(pointText, oldPoint, point);

        float oldMult = 1;
        float.TryParse(multText.text, out oldMult);
        TextEffectHelper.UpdateTextWithPunchEffect(multText, oldMult, 1);
    }
    public void ResetNewTurn()
    {
        paternText.text = DicePattern.None.ToString();
        pointText.text = "0";
        multText.text = "0";
        totalDamageText.text = "0";
    }
}
