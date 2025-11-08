using UnityEngine;
using UnityEngine.AI; 

public class HunterAI : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform player;
    private SimpleFPC playerController; 

    [Header("Patrol")]
    public Transform[] patrolPoints;
    private int currentPatrolIndex;
    private Transform currentPatrolTarget;

    [Header("Speeds")]
    public float patrolSpeed = 3f;
    public float searchSpeed = 4f; 
    public float chaseSpeed = 6f;

    [Header("Awareness")]
    public float sightRange = 20f;
    public float hearingRange = 50f; 
    public float hearingThreshold = 4.2f; 
    public float catchDistance = 4f; 

    [Header("Audio")]
    public AudioClip patrolSound; 
    public AudioClip chaseSound; 
    public float patrolVolume = 1.0f;
    public float chaseVolume = 0.1f;
    private AudioSource stateAudioSource; 
    private AudioLowPassFilter audioFilter;

    public float normalCutoff = 22000f; 
    public float occludedCutoff = 1500f; 

    private Vector3 lastHeardPosition;
    private float searchTimer;
    public float searchWaitTime = 5.0f; 

    private enum State { PATROL, HEARD_SOUND, CHASE }
    private State currentState;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerController = player.GetComponent<SimpleFPC>(); 
        
        stateAudioSource = GetComponent<AudioSource>(); 
        audioFilter = GetComponent<AudioLowPassFilter>(); 
        
        if (stateAudioSource == null || audioFilter == null)
        {
            Debug.LogError("HunterAI is missing its AudioSource or AudioLowPassFilter!");
            return;
        }

        if (playerController == null)
            Debug.LogError("HunterAI cannot find the SimpleFPC script on the Player!");

        currentState = State.PATROL;
        agent.speed = patrolSpeed;
        ChangeAudio(patrolSound, patrolVolume); 
        
        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            Debug.LogError("HunterAI has no patrol points assigned!");
            return;
        }
        currentPatrolIndex = 0;
        currentPatrolTarget = patrolPoints[currentPatrolIndex];
        agent.SetDestination(currentPatrolTarget.position);
    }

    void Update()
    {
        switch (currentState)
        {
            case State.PATROL:
                Patrol();
                break;
            case State.HEARD_SOUND: 
                Search();
                break;
            case State.CHASE:
                Chase();
                break;
        }

        HandleAudioOcclusion();
    }
    
    void ChangeAudio(AudioClip newClip, float newVolume)
    {
        stateAudioSource.volume = newVolume;

        if (stateAudioSource.clip == newClip)
            return;

        stateAudioSource.clip = newClip;
        stateAudioSource.Play();
    }

    bool CanHearPlayer()
    {
        if (playerController == null) return false;

        if (playerController.currentNoiseLevel <= hearingThreshold)
            return false;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer > hearingRange)
            return false;
        
        return true;
    }

    void Patrol()
    {
        agent.speed = patrolSpeed; 
        ChangeAudio(patrolSound, patrolVolume); 

        if (CanSeePlayer())
        {
            currentState = State.CHASE;
        }
        else if (CanHearPlayer())
        {
            currentState = State.HEARD_SOUND;
            lastHeardPosition = player.position; 
        }

        if (patrolPoints.Length > 0 && agent.pathPending == false && agent.remainingDistance < 0.5f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            currentPatrolTarget = patrolPoints[currentPatrolIndex];
            agent.SetDestination(currentPatrolTarget.position);
        }
    }

    void Search()
    {
        agent.speed = searchSpeed; 
        ChangeAudio(patrolSound, patrolVolume); 
        agent.SetDestination(lastHeardPosition);

        if (CanSeePlayer())
        {
            currentState = State.CHASE;
            return;
        }
        
        if (CanHearPlayer())
        {
            lastHeardPosition = player.position;
            agent.SetDestination(lastHeardPosition);
        }

        if (agent.pathPending == false && agent.remainingDistance < 0.5f)
        {
            searchTimer += Time.deltaTime;
            
            if (searchTimer >= searchWaitTime)
            {
                currentState = State.PATROL;
                searchTimer = 0f;
            }
        }
    }
    
    void Chase()
    {
        agent.speed = chaseSpeed; 
        ChangeAudio(chaseSound, chaseVolume); 
        agent.SetDestination(player.position); 

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
    
        if (distanceToPlayer <= catchDistance)
        {
            if (playerController != null)
            {
                playerController.Die(stateAudioSource); 
            }

            agent.isStopped = true; 
            agent.enabled = false;
            this.enabled = false; 
            return;
        }
    
        if (!CanSeePlayer())
        {
            if (!CanHearPlayer())
            {
                currentState = State.PATROL;
            
                if (patrolPoints.Length > 0)
                {
                    float closestDistance = Mathf.Infinity;
                    int closestIndex = 0;
                    for (int i = 0; i < patrolPoints.Length; i++)
                    {
                        float dist = Vector3.Distance(transform.position, patrolPoints[i].position);
                        if (dist < closestDistance)
                        {
                            closestDistance = dist;
                            closestIndex = i;
                        }
                    }
                    currentPatrolIndex = closestIndex;
                    currentPatrolTarget = patrolPoints[currentPatrolIndex];
                    agent.SetDestination(currentPatrolTarget.position);
                }
            }
            else
            {
                currentState = State.HEARD_SOUND;
            
                lastHeardPosition = player.position;
            }
        }
    }

    void HandleAudioOcclusion()
    {
        if (player == null || audioFilter == null) return;
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        
        if (Physics.Raycast(transform.position, directionToPlayer, out RaycastHit hit, distanceToPlayer))
        {
            if (hit.transform.CompareTag("Player"))
                audioFilter.cutoffFrequency = normalCutoff;
            else 
                audioFilter.cutoffFrequency = occludedCutoff;
        }
        else
            audioFilter.cutoffFrequency = normalCutoff;
    }

    bool CanSeePlayer()
    {
        if (player == null) return false;
        if (Vector3.Distance(transform.position, player.position) > sightRange)
            return false;

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        if (Physics.Raycast(transform.position, directionToPlayer, out RaycastHit hit, sightRange))
        {
            if (hit.transform.CompareTag("Player"))
                return true;
        }
        return false;
    }
}