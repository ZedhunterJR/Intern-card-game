using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using TMPro;

public class KeywordTextField : TextMeshProUGUI
{
    public List<string> DetectedKeywords { get; private set; } = new();

    public static readonly Dictionary<string, (string display, string color, string description)> KeywordDict = new()
    {
        { "[this]",  ("This", "red", "This card")},
        { "[lr]",  ("L/R", "red", "Cards on the left and right")},
        { "[all]",  ("All", "red", "")},
        { "[even]",  ("Even", "red", "")},
        { "[odd]",  ("Odd", "red", "")},
        { "[edge]",  ("Edge", "red", "")},
        { "[adj]",  ("Adj", "red", "")},
        { "[thisdice]", ("thisDice", "red", "The die placed on this card") },
        { "[posdice]", ("posDice", "red", "All dice on the position cards") },
        { "[eachhas]",  ("EachHas", "red", "Each position card must fulfill...") },
        { "[has]",  ("Has", "red", "All position combined has at least...") },
    };

    protected override void OnEnable()
    {
        base.OnEnable();
        TMPro_EventManager.TEXT_CHANGED_EVENT.Add(OnTextChanged);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(OnTextChanged);
    }

    private void OnTextChanged(Object obj)
    {
        if (obj != this) return;

        ParseKeywords();
    }

    private void ParseKeywords()
    {
        DetectedKeywords.Clear();
        string processed = text;

        foreach (var pair in KeywordDict)
        {
            string id = pair.Key;
            string display = $"<color={pair.Value.color}>{pair.Value.display}</color>";

            if (processed.Contains(id))
            {
                processed = processed.Replace(id, display);
                DetectedKeywords.Add(id);
            }
        }

        text = processed; // Replace raw [id] tags with styled labels
    }

}

