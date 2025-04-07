using UnityEngine;
using UnityEngine.AI;

public enum MorningDestination {
    Market,
    CityCenter
}

public class NPCSchedule : MonoBehaviour
{
    [Header("Destination Points")]
    public Transform Market;
    public Transform CityCenter;
    public Transform Mosque;
    public Transform[] Houses;

    [Header("NPC House Assignment")]
    public int AssignedHouseIndex = 0;

    private NavMeshAgent agent;
    private Transform currentTarget;
    private MorningDestination morningDestination;

    private float checkInterval = 5f;
    private float nextCheckTime;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        morningDestination = (MorningDestination)Random.Range(0, 2);

        if (agent == null)
        {
            Debug.LogError($"{gameObject.name} is missing NavMeshAgent!");
        }

        if (LightingManager.Instance == null)
        {
            Debug.LogError("LightingManager.Instance is NULL!");
        }
        else
        {
            Debug.Log($"LightingManager Time: {LightingManager.Instance.TimeOfDay}");
        }
    }

    private void Update()
    {
        if (LightingManager.Instance == null) return;

        if (Time.time >= nextCheckTime)
        {
            int currentHour = Mathf.FloorToInt(LightingManager.Instance.TimeOfDay);
            Transform target = GetDestinationForHour(currentHour);

            if (target != null && target != currentTarget)
            {
                float distance = Vector3.Distance(transform.position, target.position);
                if (distance > 0.5f)
                {
                    agent.SetDestination(target.position);
                    currentTarget = target;
                    Debug.Log($"{gameObject.name} is moving to: {target.name} at hour {currentHour}");
                }
            }
            nextCheckTime = Time.time + checkInterval;
        }
    }

    Transform GetDestinationForHour(int hour)
    {
        Debug.Log($"Checking hour: {hour}");

        if (hour >= 6 && hour < 12)
        {
            Debug.Log("Morning: going to market or city center");
            return morningDestination == MorningDestination.Market ? Market : CityCenter;
        }
        else if (hour == 12)
        {
            Debug.Log("12PM: going to mosque");
            return Mosque;
        }
        else if (hour > 12 && hour < 18)
        {
            Debug.Log("Afternoon: back to market or city center");
            return morningDestination == MorningDestination.Market ? Market : CityCenter;
        }
        else if (hour == 18)
        {
            Debug.Log("6PM: going to mosque again");
            return Mosque;
        }
        else if (hour >= 20)
        {
            Debug.Log("After 8PM: going home");
            if (AssignedHouseIndex >= 0 && AssignedHouseIndex < Houses.Length)
                return Houses[AssignedHouseIndex];
        }

        Debug.Log("No movement for this hour");
        return null;
    }
}
