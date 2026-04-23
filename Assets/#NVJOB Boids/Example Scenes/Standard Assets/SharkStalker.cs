// using UnityEngine;

// public class SharkStalker : MonoBehaviour
// {
//     public enum SharkState { Patrol, Circle, Flee }
//     public SharkState currentState = SharkState.Patrol;

//     [Header("Targets")]
//     public Transform playerCamera;     
//     public VRCameraShaker shaker;      

//     [Header("Movement Settings")]
//     public float patrolSpeed = 5f;
//     public float circleSpeed = 8f;
//     public float fleeSpeed = 15f;
//     public float rotationSpeed = 2f;

//     [Header("Circling Mechanics")]
//     public float detectionRadius = 120f;   
//     public float minCircleRadius = 15f;    // Increased for Scale 4 Sharks!
//     public float shakeTriggerDistance = 35f; // Increased for Scale 4 Sharks!
//     public float maxWanderDistance = 150f; 
    
//     private float currentOrbitRadius;
//     private float orbitDirectionMultiplier; // Solves the "clumping" issue

//     [Header("Defense Mechanics (Gaze)")]
//     public float requiredStareTime = 1.5f; 
//     [Range(0.8f, 1.0f)]
//     public float gazePrecision = 0.85f;    
    
//     private float currentStareTime = 0f;
//     private float fleeTimer = 0f;

//     void Start()
//     {
//         currentOrbitRadius = detectionRadius;
        
//         // Randomize speeds so they don't sync up
//         patrolSpeed += Random.Range(-1.5f, 1.5f);
//         circleSpeed += Random.Range(-1.5f, 1.5f);

//         // Randomize orbit distance and direction so they attack from different angles
//         minCircleRadius += Random.Range(-4f, 5f);
//         orbitDirectionMultiplier = Random.value > 0.5f ? 1f : -1f; // 50/50 chance for clockwise or counter-clockwise
//     }

//     void Update()
//     {
//         switch (currentState)
//         {
//             case SharkState.Patrol:
//                 PatrolBehavior();
//                 break;
//             case SharkState.Circle:
//                 CircleBehavior();
//                 break;
//             case SharkState.Flee:
//                 FleeBehavior();
//                 break;
//         }
//     }

//     void PatrolBehavior()
//     {
//         float distanceToPlayer = Vector3.Distance(transform.position, playerCamera.position);

//         if (distanceToPlayer > maxWanderDistance)
//         {
//             Vector3 directionToPlayer = (playerCamera.position - transform.position).normalized;
//             transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(directionToPlayer), Time.deltaTime * (rotationSpeed * 0.2f));
//         }

//         transform.Translate(Vector3.forward * patrolSpeed * Time.deltaTime);

//         if (distanceToPlayer < detectionRadius)
//         {
//             currentState = SharkState.Circle;
//             currentOrbitRadius = distanceToPlayer;
//         }
//     }

//     void CircleBehavior()
//     {
//         Vector3 toPlayer = playerCamera.position - transform.position;
//         float distanceToPlayer = toPlayer.magnitude;

//         currentOrbitRadius = Mathf.Lerp(currentOrbitRadius, minCircleRadius, Time.deltaTime * 0.1f);
        
//         // Safe Cross Product: Project onto a flat plane so sharks coming from directly above don't glitch out
//         Vector3 toPlayerFlat = new Vector3(toPlayer.x, 0, toPlayer.z).normalized;
//         if (toPlayerFlat == Vector3.zero) toPlayerFlat = Vector3.forward;

//         Vector3 orbitDirection = Vector3.Cross(Vector3.up, toPlayerFlat) * orbitDirectionMultiplier;
        
//         // Add the Y difference back in so they smoothly dive down to your depth
//         Vector3 swimDirection = (orbitDirection + (toPlayer.normalized * 0.2f)).normalized;

//         transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(swimDirection), Time.deltaTime * rotationSpeed);
//         transform.Translate(Vector3.forward * circleSpeed * Time.deltaTime);

//         if (distanceToPlayer < shakeTriggerDistance)
//         {
//             float intensity = 1f - (distanceToPlayer / shakeTriggerDistance);
//             shaker.SetShakeIntensity(intensity);
            
//             CheckGazeDefense();
//         }
//         else
//         {
//             // The "shaker.SetShakeIntensity(0f);" has been completely removed from here!
//             currentStareTime = Mathf.Max(0, currentStareTime - (Time.deltaTime * 0.5f));
//         }
//     }

//     void CheckGazeDefense()
//     {
//         Vector3 targetCenter = transform.position + new Vector3(0, 0.5f, 0);
//         Vector3 toShark = (targetCenter - playerCamera.position).normalized;
        
//         float lookAccuracy = Vector3.Dot(playerCamera.forward, toShark);

//         if (lookAccuracy > gazePrecision)
//         {
//             currentStareTime += Time.deltaTime; 
            
//             if (currentStareTime >= requiredStareTime)
//             {
//                 currentState = SharkState.Flee;
//                 fleeTimer = 5f; 
//                 currentStareTime = 0f; 
//             }
//         }
//         else
//         {
//             currentStareTime = Mathf.Max(0, currentStareTime - (Time.deltaTime * 0.5f));
//         }
//     }

//     void FleeBehavior()
//     {
//         Vector3 awayFromPlayer = (transform.position - playerCamera.position).normalized;
        
//         transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(awayFromPlayer), Time.deltaTime * (rotationSpeed * 2f));
//         transform.Translate(Vector3.forward * fleeSpeed * Time.deltaTime);

//         fleeTimer -= Time.deltaTime;
//         if (fleeTimer <= 0)
//         {
//             currentState = SharkState.Patrol; 
//         }
//     }
// }




//With limits
using UnityEngine;

public class SharkStalker : MonoBehaviour
{
    public enum SharkState { Patrol, Circle, Flee }
    public SharkState currentState = SharkState.Patrol;

    [Header("Targets")]
    public Transform playerCamera;     
    public VRCameraShaker shaker;      

    [Header("Movement Settings")]
    public float patrolSpeed = 5f;
    public float circleSpeed = 8f;
    public float fleeSpeed = 15f;
    public float rotationSpeed = 2f;

    [Header("Circling Mechanics")]
    public float detectionRadius = 120f;   
    public float minCircleRadius = 15f;    
    public float shakeTriggerDistance = 35f; 
    public float maxWanderDistance = 150f; 
    
    private float currentOrbitRadius;
    private float orbitDirectionMultiplier; 

    [Header("Depth Limits (Water & Sand)")]
    public float maxDepthY = 120f; 
    public float minDepthY = 15f;   

    [Header("Defense Mechanics (Gaze)")]
    public float requiredStareTime = 1.5f; 
    [Range(0.8f, 1.0f)]
    public float gazePrecision = 0.85f;    
    
    private float currentStareTime = 0f;
    private float fleeTimer = 0f;

    // --- Wandering & Fleeing Variables ---
    private Vector3 randomWanderDirection;
    private float wanderTimer = 0f;
    
    // NEW: We store a specific randomized escape route so they scatter!
    private Vector3 currentFleeDirection; 

    void Start()
    {
        currentOrbitRadius = detectionRadius;
        
        patrolSpeed += Random.Range(-1.5f, 1.5f);
        circleSpeed += Random.Range(-1.5f, 1.5f);

        minCircleRadius += Random.Range(-4f, 5f);
        orbitDirectionMultiplier = Random.value > 0.5f ? 1f : -1f; 

        randomWanderDirection = transform.forward;
    }

    void Update()
    {
        switch (currentState)
        {
            case SharkState.Patrol:
                PatrolBehavior();
                break;
            case SharkState.Circle:
                CircleBehavior();
                break;
            case SharkState.Flee:
                FleeBehavior();
                break;
        }

        // The Hard Clamp
        Vector3 clampedPos = transform.position;
        if (clampedPos.y > maxDepthY) clampedPos.y = maxDepthY;
        if (clampedPos.y < minDepthY) clampedPos.y = minDepthY;
        transform.position = clampedPos;
    }

    void PatrolBehavior()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, playerCamera.position);

        wanderTimer -= Time.deltaTime;
        if (wanderTimer <= 0)
        {
            randomWanderDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-0.2f, 0.2f), Random.Range(-1f, 1f)).normalized;
            wanderTimer = Random.Range(4f, 8f); 
        }

        Vector3 desiredDirection = randomWanderDirection;

        if (distanceToPlayer > maxWanderDistance)
        {
            desiredDirection = (playerCamera.position - transform.position).normalized;
        }

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredDirection), Time.deltaTime * (rotationSpeed * 0.5f));

        EnforceDepthBounds(); 
        transform.Translate(Vector3.forward * patrolSpeed * Time.deltaTime);

        if (distanceToPlayer < detectionRadius)
        {
            currentState = SharkState.Circle;
            currentOrbitRadius = distanceToPlayer;
        }
    }

    void CircleBehavior()
    {
        Vector3 toPlayer = playerCamera.position - transform.position;
        float distanceToPlayer = toPlayer.magnitude;

        currentOrbitRadius = Mathf.Lerp(currentOrbitRadius, minCircleRadius, Time.deltaTime * 0.1f);
        
        Vector3 toPlayerFlat = new Vector3(toPlayer.x, 0, toPlayer.z).normalized;
        if (toPlayerFlat == Vector3.zero) toPlayerFlat = Vector3.forward;

        Vector3 orbitDirection = Vector3.Cross(Vector3.up, toPlayerFlat) * orbitDirectionMultiplier;
        Vector3 swimDirection = (orbitDirection + (toPlayer.normalized * 0.2f)).normalized;

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(swimDirection), Time.deltaTime * rotationSpeed);
        
        EnforceDepthBounds(); 
        transform.Translate(Vector3.forward * circleSpeed * Time.deltaTime);

        if (distanceToPlayer < shakeTriggerDistance)
        {
            float intensity = 1f - (distanceToPlayer / shakeTriggerDistance);
            shaker.SetShakeIntensity(intensity);
            
            CheckGazeDefense();
        }
        else
        {
            currentStareTime = Mathf.Max(0, currentStareTime - (Time.deltaTime * 0.5f));
        }
    }

    void CheckGazeDefense()
    {
        Vector3 targetCenter = transform.position + new Vector3(0, 0.5f, 0);
        Vector3 toShark = (targetCenter - playerCamera.position).normalized;
        
        float lookAccuracy = Vector3.Dot(playerCamera.forward, toShark);

        if (lookAccuracy > gazePrecision)
        {
            currentStareTime += Time.deltaTime; 
            
            if (currentStareTime >= requiredStareTime)
            {
                currentState = SharkState.Flee;
                fleeTimer = 5f; 
                currentStareTime = 0f; 

                // --- THE FIX: CALCULATE A RANDOM SCATTER ROUTE ---
                Vector3 awayFromPlayer = (transform.position - playerCamera.position).normalized;
                
                // Add a massive random kick (up, down, left, right)
                Vector3 randomKick = new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(-1.5f, 1.5f), Random.Range(-1.5f, 1.5f)).normalized;
                
                // Blend them so the shark goes generally away, but in a chaotic, unpredictable direction!
                currentFleeDirection = (awayFromPlayer + randomKick).normalized;
            }
        }
        else
        {
            currentStareTime = Mathf.Max(0, currentStareTime - (Time.deltaTime * 0.5f));
        }
    }

    void FleeBehavior()
    {
        float currentTurnSpeed = rotationSpeed * 2f; 
        
        // --- WE NOW USE OUR RANDOMIZED SCATTER ROUTE ---
        if (transform.position.y > maxDepthY - 10f && currentFleeDirection.y > 0)
        {
            currentFleeDirection.y = -0.5f; // Force them to dive if hitting surface
            currentTurnSpeed *= 5f; 
        }
        else if (transform.position.y < minDepthY + 10f && currentFleeDirection.y < 0)
        {
            currentFleeDirection.y = 0.5f; // Force them to climb if hitting sand
            currentTurnSpeed *= 5f; 
        }

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(currentFleeDirection), Time.deltaTime * currentTurnSpeed);
        
        EnforceDepthBounds(); 
        transform.Translate(Vector3.forward * fleeSpeed * Time.deltaTime);

        fleeTimer -= Time.deltaTime;
        if (fleeTimer <= 0)
        {
            wanderTimer = 0f; 
            currentState = SharkState.Patrol; 
        }
    }

    void EnforceDepthBounds()
    {
        if (transform.position.y > maxDepthY - 2f)
        {
            Vector3 gentlyDown = transform.forward;
            gentlyDown.y = -0.5f; 
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(gentlyDown), Time.deltaTime * rotationSpeed * 5f);
        }
        else if (transform.position.y < minDepthY + 2f)
        {
            Vector3 gentlyUp = transform.forward;
            gentlyUp.y = 0.5f; 
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(gentlyUp), Time.deltaTime * rotationSpeed * 5f);
        }
    }
}