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

    [Header("Interaction")]
    public float greetRadius = 3f;
    public float interactRadius = 2f;
    private Transform player;
    public BaseCharacterAnimatorHandler animHandler;

    [Header("Chance Settings")]
    [Range(0, 1)] public float helloChance = 0.5f;      // 50% chance for dialog greeting
    [Range(0, 1)] public float xpRewardChance = 0.1f;   // 10% chance for XP gift on hello
    [Range(5, 10)] public int minXpReward = 5;
    [Range(5, 10)] public int maxXpReward = 10;

    private NavMeshAgent agent;
    private Renderer[] renderers;
    private Transform currentTarget;
    private int lastCheckedHour = -1;
    private float arrivalThreshold = 0.5f;
    private int nextDepartureHour = -1;
    private bool hasSpawnedToday = false;
    private float homeStayTimer = 0f;
    private bool arrivedAtHome = false;
    private bool waitingToGoOutAgain = false;
    private bool isHidden = false;
    private bool isGreeting = false;
    private bool isInteracting = false;
    private bool isBlocked = false;
    private bool isDialogGreeting = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null)
            Debug.LogWarning("[NPCSchedule] No player found with tag 'Player'.");
        agent = GetComponent<NavMeshAgent>();
        renderers = GetComponentsInChildren<Renderer>();
        if (animHandler == null) animHandler = GetComponent<BaseCharacterAnimatorHandler>();
        if (player == null)
        {
            var go = GameObject.FindGameObjectWithTag("Player");
            if (go) player = go.transform;
        }
        HideNPC(); // Start hidden until 7 AM
    }

    void Update()
    {
        if (LightingManager.Instance == null || agent == null || !agent.isOnNavMesh) return;

        int currentHour = Mathf.FloorToInt(LightingManager.Instance.TimeOfDay);

        // --- SCHEDULE ---
        if (currentHour != lastCheckedHour)
        {
            lastCheckedHour = currentHour;
            if (currentHour == 7) SpawnForTheDay();
            if ((currentHour == 12 || currentHour == 18) && !isHidden) SetDestination(Mosque);
            if ((currentHour >= 7 && currentHour < 12) || (currentHour > 12 && currentHour < 18))
            {
                if (!isHidden && !arrivedAtHome && !waitingToGoOutAgain) SetDestination(GetRandomPublicPlace());
            }
            if (currentHour >= 20 && !isHidden)
            {
                SetDestination(Houses[AssignedHouseIndex]);
                arrivedAtHome = true;
            }
            if (isHidden && nextDepartureHour > 0 && currentHour >= nextDepartureHour && currentHour < 12)
            {
                ShowNPC();
                SetDestination(GetRandomPublicPlace());
                nextDepartureHour = -1;
            }
        }

        // Arrived at destination
        if (!isHidden && agent.remainingDistance <= arrivalThreshold && !agent.pathPending)
        {
            agent.isStopped = true;
            animHandler.SetBool("jalan", false);

            if (currentTarget == Houses[AssignedHouseIndex] && !arrivedAtHome)
            {
                arrivedAtHome = true;
                homeStayTimer = Random.Range(5f, 15f);
            }
        }

        // Countdown to hide if at home
        if (arrivedAtHome)
        {
            homeStayTimer -= Time.deltaTime;
            if (homeStayTimer <= 0f)
            {
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

        // --- GREETING WHEN PLAYER IS NEAR ---
        if (!isHidden && !isInteracting && player != null)
        {
            float distToPlayer = Vector3.Distance(transform.position, player.position);

            if (!isGreeting && distToPlayer < greetRadius && agent.velocity.magnitude > 0.1f)
            {
                agent.isStopped = true;
                FacePlayer();

                // Decide random: just wave, say hello (dialog), or both
                float roll = Random.value;
                if (roll < helloChance)
                {
                    isDialogGreeting = true;
                    animHandler?.PlayAnim("jualBeli", 2f); // Use "greet" alias (make sure mapped)
                    DialogManager.Instance.StartSimpleDialog("Halo, Atta!", "Warga");
                    // Optional XP gift
                    if (Random.value < xpRewardChance)
                    {
                        int xp = Random.Range(minXpReward, maxXpReward + 1);
                        PlayerStats.Instance?.AddXP(xp);
                        DialogManager.Instance.StartSimpleDialog($"Halo Atta! Ini ada {xp} XP!", "Warga");
                    }
                }
                else
                {
                    isDialogGreeting = false;
                    animHandler?.PlayAnim("jualBeli", 2f); // Just wave
                }
                isGreeting = true;
            }
            else if (isGreeting && distToPlayer >= greetRadius)
            {
                agent.isStopped = false;
                isGreeting = false;
                isDialogGreeting = false;
                animHandler.SetBool("jalan", true);
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
        animHandler.SetBool("jalan", true);
        waitingToGoOutAgain = false;
    }

    void HideNPC()
    {
        isHidden = true;
        if (agent) agent.isStopped = true;
        animHandler.SetBool("jalan", false);
        foreach (var r in renderers) r.enabled = false;
        isGreeting = false; isInteracting = false; isBlocked = false;
    }

    void ShowNPC()
    {
        isHidden = false;
        foreach (var r in renderers) r.enabled = true;
        if (agent) agent.isStopped = false;
        animHandler.SetBool("jalan", false);
    }

    Vector3 GetOffsetPosition(Vector3 basePos)
    {
        Vector2 offset = Random.insideUnitCircle * 3f;
        Vector3 pos = basePos + new Vector3(offset.x, 0, offset.y);

        if (NavMesh.SamplePosition(pos, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            return hit.position;

        return basePos;
    }

    Transform GetMorningDestination()
    {
        switch (morningDestination)
        {
            case MorningDestination.Market: return Market;
            case MorningDestination.CityCenter: return CityCenter;
            case MorningDestination.Home: return Houses[AssignedHouseIndex];
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

    void FacePlayer()
    {
        Vector3 look = player.position - transform.position;
        look.y = 0;
        if (look.sqrMagnitude > 0.01f)
            transform.rotation = Quaternion.LookRotation(look);
    }

    // -------- INTERACTION LOGIC --------

    public void Interact()
    {
        if (isHidden || isInteracting || player == null) return;
        if (Vector3.Distance(transform.position, player.position) > interactRadius) return;

        isInteracting = true;
        agent.isStopped = true;
        FacePlayer();
        animHandler?.PlayAnim("jualBeli", 2f);

        // Replace dialog below as needed
        DialogManager.Instance.StartSimpleDialog("Halo Atta!", "Warga");
        Invoke(nameof(EndInteraction), 2f);
    }

    void EndInteraction()
    {
        isInteracting = false;
        agent.isStopped = false;
       animHandler.SetBool("jalan", true);
    }

    void OnCollisionStay(Collision collision)
    {
        if (isHidden || isBlocked) return;
        if (collision.gameObject.CompareTag("Player"))
        {
            isBlocked = true;
            agent.isStopped = true;
            animHandler?.PlayAnim("jualBeli", 1.5f);

            // Show excuse dialog (random chance)
            if (Random.value < 0.75f)
                DialogManager.Instance.StartSimpleDialog("Permisi, Atta", "Warga");

            Invoke(nameof(OnBumpResume), 1.5f);
        }
    }

    void OnBumpResume()
    {
        isBlocked = false;
        agent.isStopped = false;
        animHandler.SetBool("jalan", true);
    }
}
