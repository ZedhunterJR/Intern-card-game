using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dice : MonoBehaviour
{
    public int currentFace;
    public List<int> diceFaces;
    public List<Sprite> diceSprites;
    [SerializeField] Image diceImage;
    [SerializeField] Image filter;
    [SerializeField] List<Sprite> filterSprites;
    private Dictionary<string, Sprite> filterSpritesDict = new();

    public DiceType CurrentDiceType = DiceType.Normal;

    public bool usedInAttack = false;
    public bool includedInPoint = false;

    private void Awake()
    {
        foreach (var s in filterSprites)
        {
            filterSpritesDict[s.name] = s;
        }
    }
    
    public void StartRound()
    {
        includedInPoint = false;
        usedInAttack = false;
    }

    public int CalculatePoint()
    {
        if (CurrentDiceType == DiceType.Gold)
            return currentFace * 10;
        if (CurrentDiceType == DiceType.Twin)
            return currentFace * 2;
        if (CurrentDiceType == DiceType.Rock)
            return 20;
        if (CurrentDiceType == DiceType.Burn)
            return 0;
        return currentFace;
    }

    public void UpdateDiceInternal()
    {
        if (CurrentDiceType == DiceType.Rock)
        {
            filter.sprite = filterSpritesDict["rock"];
            includedInPoint = true;
        }
        else
        {
            diceImage.sprite = diceSprites[currentFace - 1];
            if (CurrentDiceType == DiceType.Normal)
                filter.sprite = filterSpritesDict["blank"];
            if (CurrentDiceType == DiceType.Gold)
                filter.sprite = filterSpritesDict["gold"];
            if (CurrentDiceType == DiceType.Frozen)
                filter.sprite = filterSpritesDict["frozen"];
            if (CurrentDiceType == DiceType.Twin)
                filter.sprite = filterSpritesDict["twin"];
            if (CurrentDiceType == DiceType.Gem)
                filter.sprite = filterSpritesDict["gem"];
        }
    }

}

public enum DiceType
{
    Normal,
    Gold,
    Twin,
    Rock,
    Frozen,
    Gem,
    Low,
    High,
    Burn,
}