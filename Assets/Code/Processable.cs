using UnityEngine;

public class ProcessableObject : MonoBehaviour
{
    public CommodityType resultCommodity;
    public int yieldAmount = 1;

    [Header("Processing Time")]
    public float requiredHoldTime = 15f; // seconds
    private float holdTimer = 0f;
    private bool isProcessing = false;

    [Header("UI Feedback")]
    public UnityEngine.UI.Image progressBar; // Optional, for progress bar

    private void Update()
    {
        if (isProcessing)
        {
            if (Input.GetKey(KeyCode.E))
            {
                holdTimer += Time.deltaTime;
                UpdateProgressBar();

                if (holdTimer >= requiredHoldTime)
                {
                    CompleteProcessing();
                }
            }
            else
            {
                ResetProcessing();
            }
        }
    }

    private void CompleteProcessing()
    {
        PlayerInventory player = FindObjectOfType<PlayerInventory>();
        if (player != null)
        {
            player.AddCommodity(resultCommodity, yieldAmount);
        }

        ResetProcessing();
        gameObject.SetActive(false); // optional: destroy or disable after processing
    }

    private void ResetProcessing()
    {
        holdTimer = 0f;
        isProcessing = false;
        UpdateProgressBar();
    }

    private void UpdateProgressBar()
    {
        if (progressBar != null)
        {
            progressBar.fillAmount = holdTimer / requiredHoldTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isProcessing = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ResetProcessing();
        }
    }
}
