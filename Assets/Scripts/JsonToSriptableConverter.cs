#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using Unity.VisualScripting;

public enum ConversionType
{
    Items,
    Dialogs
}

[SerializeField]
public class DialogRowData
{
    public int? id;
    public string characterName;
    public string text;
    public int? nextId;
    public string portraitPath;
    public string choiceText;
    public int? choiceNextid;
}

public class JsonToSriptableConverter : EditorWindow
{
    private string jsonFilePath = "";
    private string outputFolder = "Assets/ScriptableObject";
    private bool createDatabase = true;
    private ConversionType conversionType = ConversionType.Items;

    [MenuItem("Tools/JSON to Scriptable Object")]

    public static void ShowWindow()
    {
        GetWindow<JsonToSriptableConverter>("JSON to Scriptable Object");
    }

    private void OnGUI()
    {
        GUILayout.Label("JSON to scriptable Object Converter", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        if(GUILayout.Button("Sellect JSON File"))
        {
            jsonFilePath = EditorUtility.OpenFilePanel("Select JSON File", "", "json");
        }

        EditorGUILayout.LabelField("Select File : ", jsonFilePath);
        EditorGUILayout.Space();

        conversionType = (ConversionType)EditorGUILayout.EnumPopup("Conversion Type :", conversionType);
        if(conversionType == ConversionType.Items)
        {
            outputFolder = "Assets/ScriptableOjects/Items";
        }
        else if(conversionType == ConversionType.Dialogs)
        {
            outputFolder = "Assets/ScriptableOjects/Dialogs";
        }

        outputFolder = EditorGUILayout.TextField("Output Folder : ", outputFolder);
        createDatabase = EditorGUILayout.Toggle("Create Database Asset", createDatabase);
        EditorGUILayout.Space();

        if(GUILayout.Button("Convert to Scriptable Object"))
        {
            if(string.IsNullOrEmpty(jsonFilePath))
            {
                EditorUtility.DisplayDialog("Error", "Please select a json file firest!", "OK");
                return;
            }

            switch(conversionType)
            {
                case ConversionType.Items:
                    ConvertJsonToItemScriptableObjects();
                    break;
                case ConversionType.Dialogs:
                    ConvertJsonToDialogScriptableObject();
                    break;
                        
            }
            //ConvertJsonToItemScriptableObjects();
        }
    }
    private void ConvertJsonToItemScriptableObjects()
    {
        if(!Directory.Exists(outputFolder))
        {
            Directory.CreateDirectory(outputFolder);
        }

        string jsonText = File.ReadAllText(jsonFilePath);

        try
        {
            List<ItemData> itemDAtaList = JsonConvert.DeserializeObject<List<ItemData>>(jsonText);

            List<ItemSO> createdItems = new List<ItemSO>();

            foreach(var itemData in itemDAtaList)
            {
                ItemSO itemSO = ScriptableObject.CreateInstance<ItemSO>();

                itemSO.id = itemData.id;
                itemSO.itemName = itemData.itemName;
                itemSO.nameEng = itemData.nameEng;
                itemSO.description = itemData.description;

                if(System.Enum.TryParse(itemData.itemTypeString, out ItemType parsedType))
                {
                    itemSO.itemType = parsedType;
                }
                else
                {
                    Debug.LogWarning($"������'{itemData.itemName}'�� �������� ���� Ÿ�� : {itemData.itemTypeString}");
                }

                itemSO.price = itemData.price;
                itemSO.power = itemData.power;
                itemSO.level = itemData.level;
                itemSO.isStackable = itemData.isStackable;

                //������ �ε�(��ΰ� �ִ°��)
                if(!string.IsNullOrEmpty(itemData.iconPath))
                {
                    itemSO.icon = AssetDatabase.LoadAssetAtPath<Sprite>($"Assets/Resources/{itemData.iconPath}.png");
                    if(itemSO.icon == null)
                    {
                        Debug.LogWarning($"������ '{itemData.nameEng}'�� �������� ã�� �� �����ϴ� : {itemData.iconPath}");
                    }
                }

                string assetPath = $"{outputFolder}/item_{itemData.id.ToString("D4")}_{itemData.nameEng}.asset";
                AssetDatabase.CreateAsset(itemSO, assetPath );

                itemSO.name = $"item_{itemData.id.ToString("D4")}+{itemData.nameEng}";
                createdItems.Add( itemSO );

                EditorUtility.SetDirty( itemSO );
            }

            if(createDatabase && createdItems.Count > 0 )
            {
                ItemDatabaseSO database = ScriptableObject.CreateInstance<ItemDatabaseSO>();
                database.items = createdItems;

                AssetDatabase.CreateAsset(database, $" (outputFolder)/IteDatabase.asset");
                EditorUtility.SetDirty( database );
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        catch (System.Exception e)
        {
            EditorUtility.DisplayDialog("Error", $"Failed to Convert Json : {e.Message}","OK");
            Debug.LogError($"JSON ��ȯ ���� : {e}");
        }
    }

    //��ȭ
    private void ConvertJsonToDialogScriptableObject()
    {
        if (!Directory.Exists(outputFolder))
        {
            Directory.CreateDirectory(outputFolder);
        }
        string jsonText = File.ReadAllText(jsonFilePath);

        try
        {
            //JSON �Ľ�
            List<DialogRowData> rowDataList = JsonConvert.DeserializeObject<List<DialogRowData>>(jsonText);

            //��ȭ ������ �籸��
            Dictionary<int, DialogSO>dialogMap = new Dictionary<int, DialogSO>();
            List<DialogSO> creatDialog = new List<DialogSO>();

            //1�ܰ� : ��ȭ �׸� ����
            foreach(var rowData in rowDataList)
            {
                //id�ִ� ���� ��ȭ�� ó��
                if(rowData.id.HasValue)
                {
                    DialogSO dialogSO = ScriptableObject.CreateInstance<DialogSO>();

                    //������ ����
                    dialogSO.id = rowData.id.Value;
                    dialogSO.charaterName = rowData.characterName;
                    dialogSO.text = rowData.text;
                    dialogSO.nextid = rowData.nextId.HasValue ? rowData.nextId.Value : -1;
                    dialogSO.portraitPath = rowData.portraitPath;
                    dialogSO.choice = new List<DialogChoiceSO>();

                    if(!string.IsNullOrEmpty(rowData.portraitPath))
                    {
                        dialogSO.portrait = Resources.Load<Sprite>(rowData.portraitPath);
                        if(dialogSO.portrait == null)
                        {
                            Debug.LogWarning($"��ȭ {rowData.id}�� �ʻ�ȭ�� ã�� �� �����ϴ�");
                        }
                    }
                    dialogMap[dialogSO.id] = dialogSO;
                    creatDialog.Add(dialogSO);
                }
            }

            //2�ܰ� : ������ �׸� ó�� �� ����
            foreach(var rowData in rowDataList)
            {
                if(!rowData.id.HasValue && !string.IsNullOrEmpty(rowData.choiceText)&& rowData.choiceNextid.HasValue)
                {
                    int parentId = -1;

                    int currentIndex = rowDataList.IndexOf(rowData);
                    for(int i = currentIndex -1; i >= 0; i--)
                    {
                        if (rowDataList[i].id.HasValue)
                        {
                            parentId = rowDataList[i].id.Value;
                            break;
                        }
                    }

                    if(parentId == -1)
                    {
                        Debug.LogWarning($"������ '{rowData.choiceText}'�� �θ� ��ȭ�� ã�� �� �����ϴ�");
                    }
                    if(dialogMap.TryGetValue(parentId, out DialogSO parentDialog))
                    {
                        DialogChoiceSO choiceSO = ScriptableObject.CreateInstance<DialogChoiceSO>();
                        choiceSO.text = rowData.choiceText;
                        choiceSO.nextid = rowData.choiceNextid.Value;

                        string choiceAssetPath = $"{outputFolder}/Choice_{parentId}_{parentDialog.choice.Count + 1}.asset";
                        AssetDatabase.CreateAsset(choiceSO, choiceAssetPath);
                        EditorUtility.SetDirty(choiceSO);

                        parentDialog.choice.Add(choiceSO);
                    }
                    else
                    {
                        Debug.LogWarning($"������ '{rowData.choiceText}'�� ������ ��ȭ (ID : {parentId})�� ã�� �� �����ϴ�");
                    }
                }
            }

            //3�ܰ�
            foreach(var dialog in creatDialog)
            {
                string assetPath = $"{outputFolder}/Dialog_{dialog.id.ToString("D4")}.asset";
                AssetDatabase.CreateAsset( dialog, assetPath );

                dialog.name = $"Dialog_{dialog.id.ToString("D4")}";

                EditorUtility.SetDirty ( dialog );
            }

            //������ ���̽� ���� 
            if(createDatabase && creatDialog.Count > 0)
            {
                DialogDatabaseSO database = ScriptableObject.CreateInstance<DialogDatabaseSO>();
                database.dialogs = creatDialog;

                AssetDatabase.CreateAsset(database, $"{outputFolder}/DialogDatabase.assets");
                EditorUtility.SetDirty( database );
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog("Success", $"Creatd {creatDialog.Count} dialog scriptable objects!", "OK");
        }
        catch (System.Exception e)
        {
            EditorUtility.DisplayDialog("Error", $"Failed to convert JSON: {e.Message}", "OK");
            Debug.LogError($"JSON ��ȯ ���� : {e}");
        }
    }
}

#endif