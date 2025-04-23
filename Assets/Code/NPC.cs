using UnityEngine;
using UnityEngine.AI;

public enum MorningDestination { Market, CityCenter, Home }

public class NPCSchedule : MonoBehaviour
{
    [Header("Destination Points")]
    public Transform Market;
    public Transform CityCenter;
    public Transform Mosque;
    public Transform[] Houses;

    [Header("NPC House Assignment")]
    public int AssignedHouseIndex = 0;
    public MorningDestination morningDestination;

    private NavMeshAgent agent;
    private Animator animator;
    private Transform currentTarget;

    private bool isHidden = false;
    private Renderer[] renderers;

    private int lastCheckedHour = -1;
    private float arrivalThreshold = 0.5f;
    private int nextDepartureHour = -1;

    private bool hasSpawnedToday = false;
    private float homeStayTimer = 0f;
    private bool arrivedAtHome = false;

    private bool waitingToGoOutAgain = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        renderers = GetComponentsInChildren<Renderer>();
        HideNPC(); // Start hidden until 7 AM
    }

    void Update()
    {
        if (LightingManager.Instance == null || agent == null || !agent.isOnNavMesh) return;

        int currentHour = Mathf.FloorToInt(LightingManager.Instance.TimeOfDay);

        if (currentHour != lastCheckedHour)
        {
            lastCheckedHour = currentHour;

            if (currentHour == 7)
            {
                SpawnForTheDay();
            }

            // Mosque at 12 PM and 6 PM
            if ((currentHour == 12 || currentHour == 18) && !isHidden)
            {
                SetDestination(Mosque);
            }

            // Active hours for back and forth movement
            if ((currentHour >= 7 && currentHour < 12) || (currentHour > 12 && currentHour < 18))
            {
                if (!isHidden && !arrivedAtHome && !waitingToGoOutAgain)
                {
                    SetDestination(GetRandomPublicPlace());
                }
            }

            // Return home after 8 PM
            if (currentHour >= 20 && !isHidden)
            {
                SetDestination(Houses[AssignedHouseIndex]);
                arrivedAtHome = true;
            }

            // NPC was hidden at 7 AM and randomly goes out later
            if (isHidden && nextDepartureHour > 0 && currentHour >= nextDepartureHour && currentHour < 12)
            {
                ShowNPC();
                SetDestination(GetRandomPublicPlace());
                nextDepartureHour = -1;
            }
        }

        if (!isHidden && agent.remainingDistance <= arrivalThreshold && !agent.pathPending)
        {
            agent.isStopped = true;
            if (animator) animator.SetBool("run", false);

            if (currentTarget == Houses[AssignedHouseIndex] && !arrivedAtHome)
            {
                arrivedAtHome = true;
                homeStayTimer = Random.Range(5f, 15f); // Stay 5-15 seconds
            }
        }

        // Countdown to hide if at home
        if (arrivedAtHome)
        {
            homeStayTimer -= Time.deltaTime;
            if (homeStayTimer <= 0f)
            {
                // 50% chance to go out again if during active hours
                if ((currentHour >= 7 && currentHour < 12) || (currentHour > 12 && currentHour < 18))
                {
                    if (Random.value < 0.5f)
                    {
                        waitingToGoOutAgain = true;
                        SetDestination(GetRandomPublicPlace());
                    }
                    else
                    {
                        HideNPC();
                    }
                }
                else
                {
                    HideNPC();
                }
                arrivedAtHome = false;
            }
        }
    }

    void SpawnForTheDay()
    {
        hasSpawnedToday = true;
        Transform destination = GetMorningDestination();

        if (destination == Houses[AssignedHouseIndex])
        {
            isHidden = true;
            nextDepartureHour = Random.Range(8, 12); // Will leave house between 8-11 AM
        }
        else
        {
            ShowNPC();
            SetDestination(destination);
        }
    }

    void SetDestination(Transform target)
    {
        if (target == null || agent == null) return;

        currentTarget = target;
        agent.isStopped = false;
        agent.SetDestination(GetOffsetPosition(target.position));
        if (animator) animator.SetBool("run", true);

        waitingToGoOutAgain = false; // Reset any waiting flag
    }

    void HideNPC()
    {
        isHidden = true;
        if (agent) agent.isStopped = true;
        if (animator) animator.SetBool("run", false);
        foreach (var r in renderers) r.enabled = false;
    }

    void ShowNPC()
    {
        isHidden = false;
        foreach (var r in renderers) r.enabled = true;
        if (agent) agent.isStopped = false;
        if (animator) animator.SetBool("run", true);
    }

    Vector3 GetOffsetPosition(Vector3 basePos)
    {
        Vector2 offset = Random.insideUnitCircle * 3f;
        Vector3 pos = basePos + new Vector3(offset.x, 0, offset.y);

        if (NavMesh.SamplePosition(pos, out NavMeshHit hit, 2f, NavMesh.AllAreas))
        {
            return hit.position;
        }

        return basePos;
    }

    Transform GetMorningDestination()
    {
        switch (morningDestination)
        {
            case MorningDestination.Market:
                return Market;
            case MorningDestination.CityCenter:
                return CityCenter;
            case MorningDestination.Home:
                return Houses[AssignedHouseIndex];
        }
        return Market;
    }

    Transform GetRandomPublicPlace()
    {
        int rand = Random.Range(0, 3);
        if (rand == 0) return Market;
        if (rand == 1) return CityCenter;
        return Houses[AssignedHouseIndex];
    }
}
