using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StudentAI : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsGround, whatIsPlayer;
    public Transform personHead;
    private float headRotateSpeed = 200;
    public bool stationary;

    // Patrolling
    public Vector3 walkPoint;
    bool walkPointSet = false;
    public float walkPointRange;
    public float idleTime;
    private float waitTime;

    // Returning to Spot
    private Vector3 spawnPos;

    // States
    public float sightRange;
    public bool playerInSightRange;

    // Custom features
    Color color1 = new Color(0.83f,0.64f,0.48f);
    Color color2 = new Color(.35f,.16f,.11f);
    Color color3 = new Color(.82f,.46f,.21f);
     
    //skinTones.Add(color1);

    // Start is called before the first frame update
    void Awake()
    {
        // Give NPC random skin tone from set of colours
        Color[] skinTones = { color1, color2, color3};
        int index = Random.Range(0,skinTones.Length);
        personHead.GetComponent<MeshRenderer>().material.color = skinTones[index];

        walkPoint = transform.position;
        waitTime = -1;
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        spawnPos = transform.position;
        
    }
    void Patrolling()
    {
        personHead.rotation = Quaternion.RotateTowards(personHead.rotation, transform.rotation, Time.deltaTime * headRotateSpeed);
        if (!walkPointSet && waitTime < 0) {
            SearchWalkPoint();
            waitTime = idleTime;
        }
        else agent.SetDestination(walkPoint);

        // Check if walkpoint reached
        Vector3 distanceToWalkPoint = transform.position - walkPoint;
        if (distanceToWalkPoint.magnitude < 1f){ 
            walkPointSet = false;
            waitTime -= Time.deltaTime;
        }
    }

    void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z +randomZ);

        if (NavMesh.SamplePosition(walkPoint, out NavMeshHit hit, 2.76f, NavMesh.AllAreas)) walkPointSet = true;
        else{
            print("invalid");
        }
    }
    
    void ChasePlayer()
    {
        //make enemy look at player
        LookAtPlayer();     
        agent.SetDestination(transform.position);
        

        agent.SetDestination(player.position);
        waitTime = idleTime;
    }

     void LookAtPlayer(){
        Vector3 playerHead = new Vector3(player.position.x, player.position.y+1.26f, player.position.z);
        Vector3 targetRotation = playerHead - transform.position;
        personHead.rotation = Quaternion.RotateTowards(personHead.rotation, Quaternion.LookRotation(targetRotation), Time.deltaTime * headRotateSpeed);
     }

    // Update is called once per frame
    void Update()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);

        if (stationary){
            if (transform.position.x != spawnPos.x || transform.position.z != spawnPos.z){
                agent.SetDestination(spawnPos);
                personHead.rotation = Quaternion.RotateTowards(personHead.rotation, transform.rotation, Time.deltaTime * headRotateSpeed);
            } 
            else if (playerInSightRange) LookAtPlayer();
            else personHead.rotation = Quaternion.RotateTowards(personHead.rotation, transform.rotation, Time.deltaTime * headRotateSpeed);
            
        }
        else{
            if (!playerInSightRange) Patrolling();
            else if (playerInSightRange) LookAtPlayer();
        }
    }

    private void OnDrawGizmosSelected(){
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
