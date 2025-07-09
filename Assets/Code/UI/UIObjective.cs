using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class UIObjective : MonoBehaviour
{
    public GameObject panelObjective;
    public TextMeshProUGUI textQuestName;
    public TextMeshProUGUI textDescription;
    public TextMeshProUGUI textProgress;
    public Transform iconContainer;        // parent for icons
    public GameObject iconPrefab;          // prefab with Icon (Image) + Qty (TMP)

    private List<GameObject> currentIcons = new();
    private Inventory inventory;
    private QuestData currentQuest;

    private void Start()
    {
        inventory = FindObjectOfType<Inventory>();
        panelObjective.SetActive(false);

        QuestManager.Instance.OnQuestChanged += UpdateUI;
    }

    public void ShowQuest(QuestData quest)
    {
        Debug.Log("ShowQuest called for quest: " + quest.questName); // Log quest yang dipanggil
        currentQuest = quest;
        panelObjective.SetActive(true);  // Pastikan panel aktif
        UpdateUI();
    }

    public void Hide()
    {
        panelObjective.SetActive(false);
        currentQuest = null;
        ClearIcons();
    }

    private void UpdateUI()
    {
        ClearIcons();

        if (currentQuest == null || inventory == null) return;

        textQuestName.text = currentQuest.questName;
        textDescription.text = currentQuest.description;

        if (currentQuest is QuestMakanBajambaData bajambaQuest &&
            bajambaQuest.modularRequiredItems != null &&
            bajambaQuest.modularRequiredItems.Count > 0)
        {
            // Modular (multi-item)
            List<string> progressParts = new List<string>();

            foreach (var req in bajambaQuest.modularRequiredItems)
            {
                if (req.item == null) continue;
                int owned = inventory.CountOf(req.item);

                // Create icon for each required item
                GameObject icon = Instantiate(iconPrefab, iconContainer);
                icon.transform.Find("Icon").GetComponent<Image>().sprite = req.item.icon;
                currentIcons.Add(icon);

                // Progress part for this item
                string itemText = $"{req.item.name}: {owned}/{req.requiredAmount}";
                progressParts.Add(itemText);
            }

            textProgress.text = string.Join(", ", progressParts);
        }
        else
        {
            // Classic Quest (single type)
            int collected = 0;
            foreach (var item in currentQuest.requiredItems)
                collected += inventory.CountOf(item);

            if (currentQuest.requiredItems.Count > 0)
            {
                var firstItem = currentQuest.requiredItems[0];
                GameObject icon = Instantiate(iconPrefab, iconContainer);
                icon.transform.Find("Icon").GetComponent<Image>().sprite = firstItem.icon;
                currentIcons.Add(icon);

                textProgress.text = $"{collected}/{currentQuest.requiredItemAmount}";
            }
            else
            {
                textProgress.text = "";
            }
        }
    }


    private void ClearIcons()
    {
        foreach (var go in currentIcons) Destroy(go);
        currentIcons.Clear();
    }

    private void OnDestroy()
    {
        if (QuestManager.Instance != null)
            QuestManager.Instance.OnQuestChanged -= UpdateUI;
    }
}
