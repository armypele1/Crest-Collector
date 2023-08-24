using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsGround, whatIsPlayer;
    public Transform enemyHead;
    private float headRotateSpeed = 200;
    public Material red;
    public Material purple;
    public float respawnTime;
    public GameObject respawnEffect;
    private Vector3 startPos;
    public AudioClip deathSound;
    public AudioSource voice1;
    public AudioSource voice2;
    public AudioSource voice3;

    // Patrolling
    public Vector3 walkPoint;
    bool walkPointSet = false;
    public float walkPointRange;
    public float idleTime;
    private float waitTime;
    // Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    public float shootForce;
    public GameObject bullet;
    public LayerMask ignoreSelf;
    Vector3 targetPoint;
    public Transform attackPoint;

    // States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange, canSpeak;


    // Start is called before the first frame update
    void Awake()
    {
        startPos = transform.position;
        walkPoint = startPos;
        waitTime = -1;
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        canSpeak = true;
        Invoke("Speak", Random.Range(3,10));
    }
    void Patrolling()
    {
        enemyHead.rotation = Quaternion.RotateTowards(enemyHead.rotation, transform.rotation, Time.deltaTime * headRotateSpeed);
        if (!walkPointSet && waitTime < 0) {
            SearchWalkPoint();
            waitTime = Random.Range(0.1f, idleTime);
        }
        else agent.SetDestination(walkPoint);

        // Check if walkpoint reached
        Vector3 distanceToWalkPoint = transform.position - walkPoint;
        if (distanceToWalkPoint.magnitude < 1f){ 
            walkPointSet = false;
            waitTime -= Time.deltaTime;
        }

        SwitchEyeColor("purple");
    }

    void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z +randomZ);

        if (NavMesh.SamplePosition(walkPoint, out NavMeshHit hit, 3f, NavMesh.AllAreas)) walkPointSet = true;
    }
    
    void ChasePlayer()
    {
        //make enemy look at player
        LookAtPlayer();     
        agent.SetDestination(transform.position);
        

        agent.SetDestination(player.position);
        waitTime = idleTime;

        SwitchEyeColor("red");
    }

    void AttackPlayer()
    {
        //make enemy look at player
        LookAtPlayer();
        agent.SetDestination(transform.position);
        //Vector3 playerHead = new Vector3(player.position.x, player.position.y+1.46f, player.position.z);
        //Vector3 targetRotation = playerHead - transform.position;
        //enemyHead.rotation = Quaternion.RotateTowards(enemyHead.rotation, Quaternion.LookRotation(targetRotation), Time.deltaTime * headRotateSpeed);

        //enemyHead.rotation = Quaternion.Slerp(transform.rotation, targetRotation, headRotateSpeed * Time.deltaTime);
        //enemyHead.transform.LookAt(playerHead);

        if (!alreadyAttacked){
            // attack code goes here
            Shoot();
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }

        SwitchEyeColor("red");
    }
    void Shoot()
    {
        // Calculate direction from attackPoint
        Vector3 playerCenter = new Vector3(player.position.x, player.position.y+1.3f, player.position.z);
        Vector3 direction = playerCenter - attackPoint.position;

        // Instantiate bullet
        GameObject currentBullet = Instantiate(bullet, attackPoint.position, Quaternion.identity);
        attackPoint.forward = attackPoint.parent.forward;
        currentBullet.transform.forward = direction.normalized;

        // Add forces to bullet
        currentBullet.GetComponent<Rigidbody>().AddForce(direction.normalized * shootForce, ForceMode.Impulse);

    }
     void ResetAttack(){
        alreadyAttacked = false;
     }

     void LookAtPlayer(){
        Vector3 playerHead = new Vector3(player.position.x, player.position.y+1.46f, player.position.z);
        Vector3 targetRotation = playerHead - transform.position;
        enemyHead.rotation = Quaternion.RotateTowards(enemyHead.rotation, Quaternion.LookRotation(targetRotation), Time.deltaTime * headRotateSpeed);
     }

     void SwitchEyeColor(string color){
        MeshRenderer eye1 = enemyHead.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>();
        MeshRenderer eye2 = enemyHead.transform.GetChild(1).gameObject.GetComponent<MeshRenderer>();

        if (color == "red"){
            eye1.material = red;
            eye2.material = red;
        }
        else if (color == "purple"){
            eye1.material = purple;
            eye2.material = purple;
        }
        else{
            print("invalid color");
        }
     }

     public void Death(){
        AudioSource.PlayClipAtPoint(deathSound, transform.position);
        gameObject.SetActive(false);
        Invoke("Respawn", respawnTime);
     }

     void respawnPoof(){
        if (respawnEffect != null) Instantiate(respawnEffect, gameObject.transform.position, Quaternion.identity);
     }

     void Respawn(){
        if (gameObject.activeSelf == false){      
            gameObject.transform.position = startPos;
            gameObject.SetActive(true);
            Invoke("respawnPoof",0.01f);
            Speak();
            voice1.Play();
        }    
     }

     void Speak(){
        if (gameObject.activeSelf){
            float chanceToTalk = Random.Range(0, 3);
            if (chanceToTalk < 1) voice1.Play();
            else if (chanceToTalk < 2) voice2.Play();
            else if (chanceToTalk < 3) voice3.Play();
            Invoke("Speak", Random.Range(3,10));
        }
     }

    // Update is called once per frame
    void Update()
    {
        //check for sight/attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange) Patrolling();
        else if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        else if (playerInAttackRange && playerInSightRange) AttackPlayer(); 
    }

    private void OnDrawGizmosSelected(){
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
