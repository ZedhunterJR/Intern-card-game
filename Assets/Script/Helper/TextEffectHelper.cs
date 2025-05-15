using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextEffectHelper
{
    public static void UpdateTextWithPunchEffect(TextMeshProUGUI textElement, float oldValue, float newValue,
       Vector3? punchRotation = null, float duration = 0.1f, int vibrato = 10, float elasticity = 1f)
    {
        if (Mathf.Approximately(oldValue, newValue)) return;

        textElement.text = newValue.ToString();

        textElement.transform
            .DOPunchRotation(punchRotation ?? new Vector3(0, 0, 10f), duration, vibrato, elasticity)
            .SetEase(Ease.Linear);
    }
}
