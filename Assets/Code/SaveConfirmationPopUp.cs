using UnityEngine;

public class SaveConfirmationPopup : MonoBehaviour
{
    public GameObject panel;
    private int selectedSlot;
    private SelectSaveManager saveManager;

    public void Show(int slot)
    {
        selectedSlot = slot;
        panel.SetActive(true);
        saveManager = FindObjectOfType<SelectSaveManager>();
    }

    public void OnYes()
    {
        saveManager.LoadSlot(selectedSlot);
    }

    public void OnNo()
    {
        panel.SetActive(false);
    }
}
