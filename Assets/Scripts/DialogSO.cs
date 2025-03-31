using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialog", menuName = "Dialog System/Dialog")]

public class DialogSO : ScriptableObject
{
    public int id;
    public string charaterName;
    public string text;
    public int nextid;
    public List<DialogChoiceSO> choice = new List<DialogChoiceSO>();
    public Sprite portrait;

    [Tooltip("초상화 리소스 경로(Resources 폴더 내의 경로)")]
    public string portraitPath;
}
