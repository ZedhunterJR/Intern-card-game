using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dice : MonoBehaviour
{
    #region Register Variable
    public int currentFace;
    public List<int> diceFaces;
    public List<Sprite> diceSprites;
    [SerializeField] Image diceImage;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        diceImage = GetComponent<Image>(); 
    }
    
    public void OnStart()
    {
        Reroll();
    }
    #endregion

    #region Methods
    public void Reroll()
    {
        currentFace = diceFaces.GetRandom();
        int index = diceFaces.IndexOf(currentFace);
        ChangeImage(index);
    }

    void ChangeImage(int index)
    {
        diceImage.sprite = diceSprites[index];
    }
    #endregion
}
