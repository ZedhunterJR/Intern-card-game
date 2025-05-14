using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class AttackSequence : Singleton<AttackSequence>
{
    [SerializeField] TextMeshProUGUI paternText;
    [SerializeField] TextMeshProUGUI pointText;
    [SerializeField] TextMeshProUGUI multText;
    [SerializeField] Transform floatingHolder;
    [SerializeField] GameObject floatingPrefab;

    [SerializeField] Transform enemyPos;

    public Queue<VisualPointSeq> visualPointQueue;
    public List<Dice> dicesUse;

    private void Start()
    {
        visualPointQueue = SkillManager.Instance.visualPointQueue;
        dicesUse = DiceManager.Instance.diceList;
    }

    public IEnumerator CaculatePointSequence()
    {
        Debug.Log($"{visualPointQueue.Count}");
        var point = 0f;
        var mult = 1f;
        var dicePattern = DicePattern.None;

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
                point += q.point;
                pointText.text = point.ToString();
                mult += q.mult;
                multText.text = mult.ToString();

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
        StartCoroutine(DiceAttackSequence());
        yield return null;
    }

    private IEnumerator DiceAttackSequence()
    {
        foreach (var dice in dicesUse)
        {
            if (dice.usedInAttack || dice.includedInPoint)
            {
                dice.gameObject.transform.DOMove(enemyPos.position, .5f).OnComplete(() =>
                    PoolingObject.Instance.ReturnDiceToPool(dice));
                yield return new WaitForSeconds(.5f);
            }
            else
            {
                PoolingObject.Instance.ReturnDiceToPool(dice);
            }
        }
        //yield return new WaitForSeconds(2f);
        DiceManager.Instance.StartTurn();


        yield return null;
    }
}
