using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolingObject : Singleton<PoolingObject>
{
    [SerializeField] Transform dicePoolParent;
    [SerializeField] GameObject dicePrefabs;

    Queue<Dice> dicePool = new();
    
    public void CreateDicePool(int createValue)
    {
        for (int i = 0; i < createValue; i++)
        {
            var dice = Instantiate(dicePrefabs, dicePoolParent);
            var diceScript = dice.GetComponent<Dice>();
            dice.SetActive(false);
            dicePool.Enqueue(diceScript);
        }
    }

    public Dice GetDiceFromPool()
    {
        if (dicePool.Count == 0)
        {
            CreateDicePool(1);
            return GetDiceFromPool();
        }
    
        var dice = dicePool.Dequeue();
        return dice;
    }

    public void ReturnDiceToPool(Dice dice)
    {
        dice.gameObject.SetActive(false);
        dicePool.Enqueue(dice);
    }
}
