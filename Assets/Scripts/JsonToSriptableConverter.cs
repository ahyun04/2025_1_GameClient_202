#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public class JsonToSriptableConverter : EditorWindow
{
    private string jsonFilePath = "";
    private string outputFolder = "Assets/ScriptableObject/items";
    private bool createDatabase = true;

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
            ConvertJsonToScriptableObject();
        }
    }
    private void ConvertJsonToScriptableObject()
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
                    Debug.LogWarning($"아이템'{itemData.itemName}'의 유허하지 않은 타입 : {itemData.itemTypeString}");
                }

                itemSO.price = itemData.price;
                itemSO.power = itemData.power;
                itemSO.level = itemData.level;
                itemSO.isStackable = itemData.isStackable;

                //아이콘 로드(경로가 있는경우)
                if(!string.IsNullOrEmpty(itemData.iconPath))
                {
                    itemSO.icon = AssetDatabase.LoadAssetAtPath<Sprite>($"Assets/Resources/{itemData.iconPath}.png");
                    if(itemSO.icon == null)
                    {
                        Debug.LogWarning($"아이템 '{itemData.nameEng}'의 아이콘을 찾을 수 없습니다 : {itemData.iconPath}");
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
            Debug.LogError($"JSON 변환 오류 : {e}");
        }
    }
}

#endif