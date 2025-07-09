using UnityEngine;

public class UIOnOffManager : MonoBehaviour
{
    [System.Serializable]
    public class UIElement
    {
        public string name;
        public GameObject target;
    }

    public UIElement[] uiElements;

    public void Show(string name)
    {
        foreach (var elem in uiElements)
        {
            if (elem.name == name && elem.target != null)
            {
                elem.target.SetActive(true);
                return;
            }
        }
        Debug.LogWarning($"[UIOnOffManager] No UI element found with name: {name}");
    }

    public void Hide(string name)
    {
        foreach (var elem in uiElements)
        {
            if (elem.name == name && elem.target != null)
            {
                elem.target.SetActive(false);
                return;
            }
        }
        Debug.LogWarning($"[UIOnOffManager] No UI element found with name: {name}");
    }

    public void Toggle(string name)
    {
        foreach (var elem in uiElements)
        {
            if (elem.name == name && elem.target != null)
            {
                bool isActive = elem.target.activeSelf;
                elem.target.SetActive(!isActive);
                return;
            }
        }
        Debug.LogWarning($"[UIOnOffManager] No UI element found with name: {name}");
    }
}
