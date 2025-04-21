using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceManager : MonoBehaviour
{
    public List<Dice> diceList;
    public List<ButtonUI> diceFace;

    public void RerollDices(List<Dice> diceList)
    {
        foreach (Dice dice in diceList)
        {
            dice.Reroll();
        }
    }

    public void UpdateDiceGraphic(Dice dice)
    {

    }
}

public class Dice
{
    public int currentFace;
    public List<int> diceFaces;
    public void Reroll()
    {
        currentFace = diceFaces.GetRandom();
    }
}
