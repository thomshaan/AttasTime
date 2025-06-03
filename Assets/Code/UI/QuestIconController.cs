using UnityEngine;
using UnityEngine.UI;

public class QuestIconController : MonoBehaviour
{
    private GameObject iconInstance;
    private Image iconImage;
    private Transform target;

    [Header("Offset")]
    public Vector3 offset = new Vector3(0, 2.2f, 0);

    [Header("Icon Sprites")]
    public Sprite iconNotStarted;
    public Sprite iconInProgress;
    public Sprite iconCompleted;

    private void Start()
    {
        target = transform; // icon mengikuti NPC ini
        CreateIconUI();
        ShowNotStarted();
    }

    private void CreateIconUI()
    {
        if (iconInstance == null)
        {
            GameObject prefab = Resources.Load<GameObject>("UI/QuestIconUI");
            if (prefab != null)
            {
                iconInstance = Instantiate(prefab);
                iconImage = iconInstance.GetComponentInChildren<Image>();
                iconInstance.SetActive(false);
            }
            else
            {
                Debug.LogWarning("[QuestIconController] Prefab FloatingIconUI tidak ditemukan di Resources/UI");
            }
        }
    }

    public void ShowNotStarted()
    {
        SetIcon(iconNotStarted);
    }

    public void ShowInProgress()
    {
        SetIcon(iconInProgress);
    }

    public void ShowCompleted()
    {
        SetIcon(iconCompleted);
    }

    public void Hide()
    {
        if (iconInstance != null)
            iconInstance.SetActive(false);
    }

    private void SetIcon(Sprite sprite)
    {
        if (iconInstance == null) CreateIconUI();

        if (iconImage != null && sprite != null)
        {
            iconImage.sprite = sprite;
            iconInstance.SetActive(true);
        }
    }

    private void LateUpdate()
    {
        if (iconInstance != null && target != null)
        {
            iconInstance.transform.position = target.position + offset;
            iconInstance.transform.forward = Camera.main.transform.forward;
        }
    }

    private void OnDestroy()
    {
        if (iconInstance != null)
            Destroy(iconInstance);
    }
}
