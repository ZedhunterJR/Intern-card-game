using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataSpriteManager : Singleton<DataSpriteManager>
{
    public Dictionary<string, Sprite> EffectSprites = new Dictionary<string, Sprite>();

    private void Awake()
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>("Sprite");

        foreach (Sprite s in sprites)
        {
            EffectSprites[s.name] = s;
        }
    }
}
