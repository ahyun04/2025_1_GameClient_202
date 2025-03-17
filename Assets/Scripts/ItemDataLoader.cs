using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class ItemDataLoader : MonoBehaviour
{
    [SerializeField]
    private string jsonFileName = "items";

    private List<ItemData> itemList;

    // Start is called before the first frame update
    void Start()
    {
        LoadItemData();
    }

    void LoadItemData()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>(jsonFileName);
        if (jsonFile != null)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(jsonFile.text);
            string correntText = Encoding.UTF8.GetString(bytes);

            //
            itemList = JsonConvert.DeserializeObject<List<ItemData>>(correntText);

            Debug.Log($"�ε�� ������ ��{itemList.Count}");
            foreach (var item in itemList)
            {
                Debug.Log($"������ : {EncodeKorean(item.itemName)}, ���� : {EncodeKorean(item.description)}");
            }
        }
        else
        {
            Debug.LogError($"JSON ������ ã�� �� �����ϴ� : {jsonFileName}");
        }
    }
    //�ѱ� ���ڵ��� ���� ���� �Լ�
    private string EncodeKorean(string text)
    {
        if (string.IsNullOrEmpty(text)) return "";  //���ؽ�Ʈ�� null ���̸� �Լ��� ������
        byte[] bytes = Encoding.Default.GetBytes(text); //string�� byte �迭�� ��ȯ�� ��
        return Encoding.UTF8.GetString(bytes); //���ڵ��� UTF8�� �ٲ۴�
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
