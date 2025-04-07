using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    public static DialogManager instance { get; private set; }

    [Header("Dialog References")]
    [SerializeField] private DialogDatabaseSO dialogDatabase;

    [Header("UI References")]
    [SerializeField] private GameObject dialogPanel;

    [SerializeField] private Image portraitImage;

    [SerializeField] private TextMeshProUGUI characterNameText;
    [SerializeField] private TextMeshProUGUI dialogText;
    [SerializeField] private Button NextButton;

    [Header("Dialog Settings")]
    [SerializeField] private float typingSpeed = 0.05f;
    [SerializeField] private bool useTyperwiterEffect = true;

    [Header("Dialog Choices")]
    [SerializeField] private GameObject choicesPanel;
    [SerializeField] private GameObject choiceButtonPrefab;

    private bool isTyping = false;
    private Coroutine typingCoroutine;

    private DialogSO currentDialog;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if(dialogDatabase != null)
        {
            dialogDatabase.Initalize();
        }
        else
        {
            Debug.LogError("Dialog Database is not assinged to Dialog Manager");
        }
        if(NextButton != null)
        {
            NextButton.onClick.AddListener(NextDialog);
        }
        else
        {
            Debug.LogError("Next Button is not assigned");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //UI 초기화, 대화시작
        CloseDialog();
        StartDialog(1);
        StartDialog(1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //ID로 대화 시작
    public void StartDialog(int dialogId)
    {
        DialogSO dialog = dialogDatabase.GetDialogById(dialogId);
        if(dialog != null)
        {
            StartDialog(dialog);
        }
        else
        {
            Debug.LogError($"Dialog with ID {dialogId} not found!");
        }
    }

    //다이얼로그SO로 대화 시작
    public void StartDialog(DialogSO dialog)
    {
        if (dialog == null) return;

        currentDialog = dialog;
        ShowDialog();
        dialogPanel.SetActive(true);
    }

    public void ShowDialog()
    {
        if(currentDialog == null)
        {
            return;
        }
        characterNameText.text = currentDialog.charaterName;

        if(useTyperwiterEffect)
        {
            StartTypingEffect(currentDialog.text);
        }
        else
        {
            dialogText.text = currentDialog.text;
        }

        dialogText.text = currentDialog.text;

        //초상화 설정
        if(currentDialog.portrait !=  null)
        {
            portraitImage.sprite = currentDialog.portrait;
            portraitImage.gameObject.SetActive(false);
        }
        else if(!string.IsNullOrEmpty(currentDialog.portraitPath))
        {
            Sprite portrait = Resources.Load<Sprite>(currentDialog.portraitPath);
            if(portrait != null)
            {
                portraitImage.sprite = portrait;
                portraitImage.gameObject.SetActive(true);
            }
            else
            {
                Debug.LogWarning($"Portrait not found at path : {currentDialog.portraitPath}");
                portraitImage.gameObject.SetActive(false);
            }
        }
        else
        {
            portraitImage.gameObject.SetActive(false);
        }

        //선택지 표시
        ClearChoices();
        if(currentDialog.choice != null && currentDialog.choice.Count > 0)
        {
            ShowChoices();
            NextButton.gameObject.SetActive(false);
        }
        else
        {
            NextButton.gameObject.SetActive(true);

        }
    }

    //다음 대화 진행 
    public void NextDialog()
    {
        if(isTyping)
        {
            StopTypingEffect();
            dialogText.text = currentDialog.text;
            isTyping = false;
            return;
        }

        if(currentDialog != null && currentDialog.nextid > 0)
        {
            DialogSO nextDialog = dialogDatabase.GetDialogById(currentDialog.nextid);
            if(nextDialog != null)
            {
                currentDialog = nextDialog;
                ShowDialog();
            }
            else
            {
                CloseDialog();
            }
        }
        else
        {
            CloseDialog();
        }
    }

    //텍스트 타이핑 효과 코루틴
    private IEnumerator TypeText(string text)
    {
        dialogText.text = "";
        foreach(char c in text)
        {
            dialogText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
        isTyping = false;
    }

    private void StopTypingEffect()
    {
        if(typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }
    }

    //타이핑 효과
    private void StartTypingEffect(string text)
    {
        isTyping = true;
        if(typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine );
        }
        typingCoroutine = StartCoroutine(TypeText(text));
    }

    //대화종료
    public void CloseDialog()
    {
        dialogPanel.SetActive(false);
        currentDialog = null;
        StopTypingEffect();
    }

    //선택지 초기화
    private void ClearChoices()
    {
        foreach(Transform child in choicesPanel.transform)
        {
            Destroy(child.gameObject);
        }
        choicesPanel.SetActive(false);
    }

    //선택지 선택 처리
    public void SelectChoice(DialogChoiceSO choice)
    {
        if(choice != null && choice.nextid>0)
        {
            DialogSO nextDialog = dialogDatabase.GetDialogById(choice.nextid);
            if(nextDialog != null )
            {
                currentDialog = nextDialog;
                ShowDialog();
            }
            else
            {
                CloseDialog();
            }
        }
        else
        {
            CloseDialog();
        }
    }

    //선택지 표시
    private void ShowChoices()
    {
        choicesPanel.SetActive(true);
        foreach(var choice in currentDialog.choice)
        {
            GameObject choiceGO = Instantiate(choiceButtonPrefab, choicesPanel.transform);
            TextMeshProUGUI buttonText = choiceGO.GetComponentInChildren<TextMeshProUGUI>();
            Button button = choiceGO.GetComponent<Button>();

            if(buttonText != null )
            {
                buttonText.text = choice.text;
            }
            if(button != null)
            {
                DialogChoiceSO choiceSO = choice;
                button.onClick.AddListener(()=>SelectChoice(choiceSO));
            }
        }
    }
}
