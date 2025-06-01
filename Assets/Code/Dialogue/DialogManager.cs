using System;
using System.Collections;
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

    [Header("Modular UI Manager")]
    public UIOnOffManager uiManager;

    [Header("Animation and Dialog")]
    public DialogUIAnimator dialogAnimator;

    private DialogData currentDialog;
    private int currentLineIndex;

    private Action yesCallback;
    private Action noCallback;

    private bool isAnimating = false;

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
        if (isAnimating)
        {
            Debug.LogWarning("[DialogManager] Animasi sedang berjalan, abaikan StartDialog");
            return;
        }

        if (dialogData == null || dialogData.lines.Count == 0) return;

        currentDialog = dialogData;
        currentLineIndex = 0;

        dialogBox.SetActive(true);
        choicePanel.SetActive(false);
        nextButton.gameObject.SetActive(true);

        StartCoroutine(StartDialogRoutine());
    }

    private IEnumerator StartDialogRoutine()
    {
        isAnimating = true;

        dialogAnimator.Show();

        uiManager.Hide("InteractButton");
        uiManager.Hide("Joystick");
        uiManager.Hide("Inventory");

        yield return new WaitForSeconds(0.1f); // wait a bit to ensure everything shown

        ShowLine();

        isAnimating = false;
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
        if (isAnimating) return;

        currentLineIndex++;
        if (currentDialog == null || currentLineIndex >= currentDialog.lines.Count)
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
        if (isAnimating) return;

        Debug.Log("Yes button clicked");
        yesCallback?.Invoke();
        EndDialog();
    }

    private void OnNoClicked()
    {
        if (isAnimating) return;

        Debug.Log("No button clicked");
        noCallback?.Invoke();
        EndDialog();
    }

    private void EndDialog()
    {
        if (isAnimating) return;
        StartCoroutine(EndDialogRoutine());
    }

    private IEnumerator EndDialogRoutine()
    {
        isAnimating = true;

        dialogAnimator.Hide();

        yield return new WaitForSeconds(dialogAnimator.animationDuration);

        uiManager.Show("InteractButton");
        uiManager.Show("Joystick");
        uiManager.Show("Inventory");

        dialogBox.SetActive(false);
        choicePanel.SetActive(false);

        yesCallback = null;
        noCallback = null;
        currentDialog = null;

        isAnimating = false;
    }
}
