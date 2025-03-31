using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "DialogChoice", menuName = "Dialog System/Choice")]

public class DialogChoiceSO : ScriptableObject
{
    public string text;
    public int nextid;
    public int choiceText;
}
