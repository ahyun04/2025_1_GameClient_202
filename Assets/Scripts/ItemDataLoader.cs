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

            Debug.Log($"로드된 아이템 수{itemList.Count}");
            foreach (var item in itemList)
            {
                Debug.Log($"아이템 : {EncodeKorean(item.itemName)}, 설명 : {EncodeKorean(item.description)}");
            }
        }
        else
        {
            Debug.LogError($"JSON 파일을 찾을 수 없습니다 : {jsonFileName}");
        }
    }
    //한글 이코딩을 위한 헬퍼 함수
    private string EncodeKorean(string text)
    {
        if (string.IsNullOrEmpty(text)) return "";  //ㅔ텍스트가 null 값이면 함수를 끝낸다
        byte[] bytes = Encoding.Default.GetBytes(text); //string을 byte 배열로 변환한 후
        return Encoding.UTF8.GetString(bytes); //인코딩을 UTF8로 바꾼다
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
