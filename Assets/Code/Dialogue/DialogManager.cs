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

    // Player movement lock
    private MonoBehaviour playerMovement; // Use your player movement script here
    private System.Reflection.PropertyInfo canMoveProp;

    private void Awake()
    {
        // Singleton pattern
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

        // Get reference to player controller (by tag)
        var playerGO = GameObject.FindGameObjectWithTag("Player");
        if (playerGO)
        {
            // CHANGE "PlayerController" TO YOUR ACTUAL MOVEMENT SCRIPT!
            playerMovement = playerGO.GetComponent<MonoBehaviour>(); // e.g. playerGO.GetComponent<PlayerController>()
            // Reflection to get the canMove property (change if your field is different)
            canMoveProp = playerMovement?.GetType().GetProperty("canMove");
            if (canMoveProp == null)
            {
                Debug.LogWarning("[DialogManager] No 'canMove' property found on player movement script. Player won't be locked.");
            }
        }
        else
        {
            Debug.LogWarning("[DialogManager] Player not found for movement lock!");
        }
    }

    public void StartDialog(DialogData dialogData)
    {
        // Before starting the dialog, stop movement
        ThirdPersonController playerController = FindObjectOfType<ThirdPersonController>();
        if (playerController != null)
        {
            playerController.HaltMovement();  // This stops the movement and locks the character
        }

        // Continue with your dialog setup
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

        // 🚩 Lock player movement
        SetPlayerCanMove(false);

        yield return new WaitForSeconds(0.1f);

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

        yesCallback?.Invoke();
        EndDialog();
    }

    private void OnNoClicked()
    {
        if (isAnimating) return;

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

        // 🚩 Unlock player movement
        SetPlayerCanMove(true);

        dialogBox.SetActive(false);
        choicePanel.SetActive(false);

        yesCallback = null;
        noCallback = null;
        currentDialog = null;

        isAnimating = false;
    }

    /// <summary>
    /// Set the player's movement ability via "canMove" property.
    /// </summary>
    private void SetPlayerCanMove(bool canMove)
    {
        if (playerMovement != null && canMoveProp != null)
        {
            canMoveProp.SetValue(playerMovement, canMove, null);
        }

        if (!canMove)
        {
            Debug.Log("[DialogManager] Movement locked during dialog.");
        }
        else
        {
            Debug.Log("[DialogManager] Movement unlocked after dialog.");
        }

        // Halt movement immediately if dialog is shown
        if (!canMove && playerMovement != null)
        {
            var haltMethod = playerMovement.GetType().GetMethod("HaltMovement");
            if (haltMethod != null)
                haltMethod.Invoke(playerMovement, null); // Call HaltMovement() when dialog shows
        }
    }


}
