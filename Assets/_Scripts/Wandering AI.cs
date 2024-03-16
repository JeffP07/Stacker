using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using System.Collections.Generic;

public class WanderingAI : MonoBehaviour {
    [Header("References")]
    public Transform player;

    [Header("Parameters")]
    public float walkSpeed;
    public float runSpeed;
    public float obstacleRange;
    public GameObject toyTarget;
    public float wanderRadius;
    public float maxWanderTime;

    [Header("Enums")]
    const int STATE_IDLE = 0;
    const int STATE_WALK = 1;
    const int STATE_SIT = 2;
    const int STATE_RUN = 3;
    const int STATE_PLAY_TOY = 4;
    const int STATE_JUMP = 5;
    const int STATE_TURN = 6;

    [Header("Sounds")]
    public List<AudioClip> meowClips;
    private AudioSource audioSource;
    public AudioClip purr;
    public AudioClip meowchirp;
    public AudioSource purrSource;

    private Animator animator;
    private string currentAction = "wander";
    private NavMeshAgent agent;
    private float wanderTime = 30f;
    Vector2 smoothDeltaPosition = Vector2.zero;
    Vector2 velocity = Vector2.zero;
    Coroutine coroutine;
    RaycastHit hit;
    private bool isAnnoying = false;

    void Start() {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        animator.SetInteger("state", STATE_WALK);
        agent = GetComponent<NavMeshAgent>();
        agent.updatePosition = false;
        audioSource = transform.GetComponent<AudioSource>();
        StartCoroutine(PlayMeowLoop());
        StartCoroutine(AnnoyLoop());
        purrSource.mute = true;
        purrSource.Play();
    }

    void Update() {
        GetCatVelocity();
        switch (currentAction) {
            case "wander":
                animator.SetInteger("state", STATE_WALK);
                if (isAnnoying) {
                    AnnoyPlayer();
                }
                else {
                    Wander();
                }
                break;
            case "findtoy":
                purrSource.mute = true;
                animator.SetInteger("state", STATE_RUN);
                FindToy();
                break;
            case "playtoy":
                animator.SetInteger("state", STATE_PLAY_TOY);
                if (coroutine == null) {
                    coroutine = StartCoroutine(PlayToy());
                }
                break;
        }
    }

    public void FindToy() {
        if (toyTarget == null) {
            return;
        }
        Vector3 toyLocation = toyTarget.transform.position;

        NavMeshHit navMeshHit;
        NavMesh.SamplePosition(toyLocation, out navMeshHit, 0.2f, -1);
        bool worked = agent.SetDestination(navMeshHit.position);
        if (!worked) {
            currentAction = "wander";
            agent.speed = walkSpeed;
            return;
        }
        agent.speed = runSpeed;
        bool cast = Physics.Raycast(transform.position, transform.forward, out hit, 0.6f, 1 << LayerMask.NameToLayer("Mouse"));
        if (cast) {
            NavMesh.SamplePosition(transform.position + (toyLocation - transform.position) * 0.5f, out navMeshHit, 0.1f, -1);
            agent.SetDestination(navMeshHit.position);
            currentAction = "playtoy";
        }
    }
    IEnumerator PlayToy() {
        audioSource.PlayOneShot(meowchirp);
        toyTarget = null;
        agent.speed = walkSpeed/5.0f;
        yield return new WaitForSeconds(15.0f + Random.Range(0, 15));

        agent.speed = walkSpeed;
        wanderTime = 0;
        currentAction = "wander";
        coroutine = null;
    }

    public void Wander() {
        agent.speed = walkSpeed;
        wanderTime += Time.deltaTime;
        bool raycastWall = false;
        bool cast = Physics.Raycast(transform.position + transform.up * 0.25f, transform.forward, out hit, 0.5f, -1);
        Debug.DrawRay(transform.position + transform.up * 0.25f, transform.forward * 0.5f, Color.green);
        if (cast) {
            if (hit.collider.CompareTag("Wall")) {
                raycastWall = true;
            }
        }
        if (wanderTime > maxWanderTime || (!agent.pathPending && !agent.hasPath) || raycastWall) {
            Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
            randomDirection += transform.position + transform.forward;
            NavMeshHit navMeshHit;
            NavMesh.SamplePosition(randomDirection, out navMeshHit, wanderRadius, -1);
            agent.destination = navMeshHit.position;
            agent.stoppingDistance = 0;
            wanderTime = 0;
            Debug.DrawRay(transform.position, navMeshHit.position, Color.red);
        }
    }

    public void GetCatVelocity() {
        Vector3 worldDeltaPosition = agent.nextPosition - transform.position;

        // Map 'worldDeltaPosition' to local space
        float dx = Vector3.Dot(transform.right, worldDeltaPosition);
        float dy = Vector3.Dot(transform.forward, worldDeltaPosition);
        Vector2 deltaPosition = new Vector2(dx, dy);

        // Low-pass filter the deltaMove
        float smooth = Mathf.Min(1.0f, Time.deltaTime / 0.15f);
        smoothDeltaPosition = Vector2.Lerp(smoothDeltaPosition, deltaPosition, smooth);

        // Update velocity if time advances
        if (Time.deltaTime > 1e-5f) {
            velocity = smoothDeltaPosition / Time.deltaTime;
        }

        animator.SetFloat("velX", velocity.x);
        animator.SetFloat("velY", velocity.y);
    }
    
    void OnAnimatorMove() {
        // Update position to agent position
        transform.position = agent.nextPosition;
        
    }
    
    public void StopPlay() {
        if (coroutine != null) {
            StopCoroutine(coroutine);
            coroutine = null;
        }
    }

    public void SetAction(string action) {
        currentAction = action;
    }

    public void SetToyTarget(GameObject toyTarget) {
        this.toyTarget = toyTarget;
    }

    IEnumerator PlayMeowLoop() {
        while (true) {
            int timeToMeow = Random.Range(30, 60);
            yield return new WaitForSeconds(timeToMeow);
            audioSource.PlayOneShot(meowClips[Random.Range(0,meowClips.Count)]);
        }
    }

    IEnumerator AnnoyLoop() {
        while (true) {
            if (currentAction == "wander") {
                int timeToNextAnnoy = Random.Range(120, 180);
                yield return new WaitForSeconds(timeToNextAnnoy);
                purrSource.mute = false;
                isAnnoying = true;
                int timeToAnnoy = Random.Range(10, 30);
                yield return new WaitForSeconds(timeToAnnoy);

                isAnnoying = false;
                purrSource.mute = true;
            }
            else {
                yield return null;
            }
        }
    }

    private void AnnoyPlayer() {
        NavMeshHit navMeshHit;
        NavMesh.SamplePosition(player.position, out navMeshHit, wanderRadius, -1);
        wanderTime = 0;
        animator.SetInteger("state", STATE_RUN);
        agent.speed = runSpeed;
        agent.SetDestination(navMeshHit.position);
        purrSource.mute = false;
    }
}
