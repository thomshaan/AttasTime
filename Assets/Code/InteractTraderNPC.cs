using UnityEngine;

public class NPCInteract : MonoBehaviour
{
    private bool isPlayerNearby = false;
    private TraderNPC trader;

    void Start()
    {
        trader = GetComponent<TraderNPC>();
        if (trader == null)
            Debug.LogWarning("TraderNPC component not found!");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("E key was pressed");
        }

        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Interacting with NPC!");
            trader.AutomaticBuy();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            Debug.Log("Player entered NPC range");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            Debug.Log("Player left NPC range");
        }
    }
}
