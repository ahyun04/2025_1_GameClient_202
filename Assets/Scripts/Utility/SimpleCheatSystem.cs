using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Rendering;

public class SimpleCheatSystem : MonoBehaviour
{
    public static SimpleCheatSystem Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject cheatPanel;
    [SerializeField] private TMP_InputField commandInput;
    [SerializeField] private TextMeshProUGUI outputText;

    [Header("Settings")]
    [SerializeField] private KeyCode toggleKey = KeyCode.F1;

    private Dictionary<string, System.Action<string[]>> commands;
    private List<string> outputLines = new List<string>();
    private bool isActive = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitialInzeCommands();

        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (cheatPanel != null)
        {
            cheatPanel.SetActive(false);
        }
        Log("치트 시스템 준비 완료. F1 키로 열기 ");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            TogglePanel();
        }
        if (isActive && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)))
        {
            ExecuteCommand();
        }
    }

    private void InitialInzeCommands()
    {
        commands = new Dictionary<string, System.Action<string[]>>
        {
            { "god" , ToggleGodMode },
            { "kill" , KillPlayer },
            { "clear" , ClearConsole },
            { "help" , ShowHelp }

        };
    }

    private void ToggleGodMode(string[] args)
    {
        Log("무적 모드 토글됨");
        ShowToast("무적 모드 ON/OFF", ToastMessage.MessageType.Success);
        // TODO: 실제 무적 모드 구현 
    }

    private void KillPlayer(string[] args)
    {
        Log("플레이어 사망");
        ShowToast("플레이어 사망!", ToastMessage.MessageType.Error);
        // TODO : 플레이어 사망 처리 구현 
    }

    private void ClearConsole(string[] args)
    {
        outputLines.Clear();
        if (outputText != null)
            outputText.text = "";
        Log("콘솔 정리됨");
    }

    private void ShowHelp(string[] args)
    {
        Log("=== 치트 명령어 ===");
        Log("god - 무적 모드");
        Log("kill - 플레이어 죽이기");
        Log("clear - 콘솔 정리");
    }

    private void Log(string message, bool isError = false)
    {
        string coloredMessage = isError ? $"<color=red>{message}</color>" : message;
        outputLines.Add(coloredMessage);

        if (outputLines.Count > 10)
            outputLines.RemoveAt(0);

        if (outputText != null)
        {
            outputText.text = string.Join("\n", outputLines);
        }
    }

    private void ShowToast(string message, ToastMessage.MessageType type = ToastMessage.MessageType.info)
    {
        if (ToastMessage.Instance != null)
        {
            ToastMessage.Instance.ShowMessage($"[치트] {message}", type);
        }
    }

    private void ExecuteCommand()
    {
        if (commandInput == null || string.IsNullOrEmpty(commandInput.text))
            return;

        string command = commandInput.text.Trim().ToLower();
        string[] parts = command.Split(' ');

        Log($"> {command}");

        if (commands.ContainsKey(parts[0]))
        {
            commands[parts[0]](parts);
        }
        else
        {
            Log($"알 수 없는 명령어 : {parts[0]}", true);

        }

        commandInput.text = "";
        commandInput.ActivateInputField();
    }

    private void TogglePanel()
    {
        isActive = !isActive;

        if (cheatPanel != null)
        {
            cheatPanel.SetActive(isActive);

            if (isActive && commandInput != null)
            {
                commandInput.Select();
                commandInput.ActivateInputField();
            }
        }
    }
}
