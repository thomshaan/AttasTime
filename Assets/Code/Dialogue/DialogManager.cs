using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogManager : MonoBehaviour
{
    public static DialogManager Instance { get; private set; }

    [Header("UI References")]
    public GameObject dialogBox;
    public TMP_Text speakerNameText;
    public TMP_Text dialogText;
    public Button nextButton;
    public GameObject choicePanel;
    public Button yesButton;
    public Button noButton;

    private DialogData currentDialog;
    private int currentLineIndex;

    private Action yesCallback;
    private Action noCallback;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        dialogBox.SetActive(false);
        choicePanel.SetActive(false);

        nextButton.onClick.AddListener(OnNextClicked);
        yesButton.onClick.RemoveAllListeners();
        yesButton.onClick.AddListener(OnYesClicked);
        noButton.onClick.AddListener(OnNoClicked);
    }

    public void StartDialog(DialogData dialogData)
    {
        if (dialogData == null || dialogData.lines.Count == 0) return;



        currentDialog = dialogData;
        currentLineIndex = 0;
        dialogBox.SetActive(true);
        choicePanel.SetActive(false);
        nextButton.gameObject.SetActive(true);

        ShowLine();
    }

    public void StartSimpleDialog(string text, string speaker = "NPC")
    {
        var simpleDialog = ScriptableObject.CreateInstance<DialogData>();
        simpleDialog.lines = new List<DialogLine>()
        {
            new DialogLine { speakerName = speaker, text = text, isChoice = false }
        };
        StartDialog(simpleDialog);
    }

    public void StartChoiceDialog(string text, Action onYes, Action onNo, string speaker = "NPC")
    {
        Debug.Log($"StartChoiceDialog dipanggil, onYes null? {onYes == null}");
        Debug.Log($"StartChoiceDialog dipanggil, onNo null? {onNo == null}");

        yesCallback = onYes;
        noCallback = onNo;

        var choiceDialog = ScriptableObject.CreateInstance<DialogData>();
        choiceDialog.lines = new List<DialogLine>()
    {
        new DialogLine { speakerName = speaker, text = text, isChoice = true }
    };

        StartDialog(choiceDialog);
    }

    private void ShowLine()
    {
        var line = currentDialog.lines[currentLineIndex];
        speakerNameText.text = line.speakerName;
        dialogText.text = line.text;

        if (line.isChoice)
        {
            nextButton.gameObject.SetActive(false);
            choicePanel.SetActive(true);
        }
        else
        {
            nextButton.gameObject.SetActive(true);
            choicePanel.SetActive(false);
        }
    }

    private void OnNextClicked()
    {
        currentLineIndex++;
        if (currentLineIndex >= currentDialog.lines.Count)
        {
            EndDialog();
        }
        else
        {
            ShowLine();
        }
    }

    private void OnYesClicked()
    {
        Debug.Log("Yes button clicked");
        if (yesCallback != null)
        {
            Debug.Log("Memanggil yesCallback...");
            yesCallback.Invoke();
        }
        else
        {
            Debug.LogWarning("yesCallback bernilai null!");
        }
        EndDialog();
    }

    private void OnNoClicked()
    {
        Debug.Log("No button clicked");
        if (noCallback != null)
        {
            noCallback.Invoke();
            // Jangan langsung EndDialog(), biarkan callback yang mengatur dialog berikutnya
        }
        else
        {
            EndDialog(); // Kalau tidak ada callback, tutup dialog
        }
    }

    private void EndDialog()
    {
        dialogBox.SetActive(false);
        choicePanel.SetActive(false);

        yesCallback = null;
        noCallback = null;
        currentDialog = null;
    }
}
