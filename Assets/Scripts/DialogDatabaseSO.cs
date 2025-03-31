using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogDatabase", menuName = "Dialog System/Database")]
public class DialogDatabaseSO : ScriptableObject
{
    public List<DialogSO> dialogs = new List<DialogSO>();

    private Dictionary<int, DialogSO> dialogById;

    public void Initalize()
    {
        dialogById = new Dictionary<int, DialogSO>();

        foreach(var dialog in dialogs)
        {
            if(dialog != null)
            {
                dialogById[dialog.id] = dialog;
            }
        }
    }

    public DialogSO GetDialogById(int id)
    {
        if(dialogById == null)
        {
            Initalize();
        }
        if(dialogById.TryGetValue(id, out DialogSO dialog))
        {
            return dialog;
        }
        return null;
    }
}
