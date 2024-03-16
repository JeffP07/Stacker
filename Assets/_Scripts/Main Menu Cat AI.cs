using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class MainMenuCatAI : MonoBehaviour {
    [Header("Parameters")]
    public float speed;
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

    private Animator animator;
    private string currentAction = "wander";
    private NavMeshAgent agent;
    private float wanderTime = 30f;
    Vector2 smoothDeltaPosition = Vector2.zero;
    Vector2 velocity = Vector2.zero;
    Coroutine coroutine;
    RaycastHit hit;

    void Start() {
        animator = GetComponent<Animator>();
        animator.SetInteger("state", STATE_RUN);
        agent = GetComponent<NavMeshAgent>();
        agent.updatePosition = false;
        StartCoroutine(RandomPlay());
    }

    void Update() {
        GetCatVelocity();
        switch (currentAction) {
            case "wander":
                animator.SetInteger("state", STATE_RUN);
                Wander();
                break;
            case "findtoy":
                animator.SetInteger("state", STATE_RUN);
                Debug.Log("find case");
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
        Debug.Log("find");
        if (toyTarget == null) {
            return;
        }
        Vector3 toyLocation = toyTarget.transform.position;

        NavMeshHit navMeshHit;
        NavMesh.SamplePosition(toyLocation, out navMeshHit, 0.2f, -1);
        bool worked = agent.SetDestination(navMeshHit.position);
        if (!worked) {
            currentAction = "wander";
            agent.speed = 1f;
            return;
        }
        agent.speed = 1;
        bool cast = Physics.Raycast(transform.position, transform.forward, out hit, 0.6f, 1 << LayerMask.NameToLayer("Mouse"));
        if (cast) {
            NavMesh.SamplePosition(transform.position + (toyLocation - transform.position) * 0.5f, out navMeshHit, 0.1f, -1);
            agent.SetDestination(navMeshHit.position);
            currentAction = "playtoy";
        }
    }
    IEnumerator PlayToy() {
        agent.speed = 0.1f;
        yield return new WaitForSeconds(5.0f + Random.Range(0, 5));

        agent.speed = 1f;
        wanderTime = 0;
        currentAction = "wander";
        coroutine = null;
    }

    public void Wander() {
        agent.speed = 1f;
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

    public IEnumerator RandomPlay() {
        while (true) {
            int randTime = Random.Range(10, 45);
            yield return new WaitForSeconds(randTime);
            currentAction = "findtoy";
        }
    }
}
