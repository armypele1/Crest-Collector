using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    [SerializeField] enum Shooter {
        Player,
        Enemy
    }
    [SerializeField] Shooter shooter;
    float timePassed;
    
    PhysicMaterial physicMat;
    public int lifetime;
    public GameObject explosion;
    public bool endOnCollision;

    [Range(0f,1f)]
    public float bounciness;
    void Setup()
    {
        physicMat = new PhysicMaterial();
        physicMat.bounciness = bounciness;
        physicMat.frictionCombine = PhysicMaterialCombine.Minimum; 
        physicMat.bounceCombine = PhysicMaterialCombine.Maximum;
        GetComponent<SphereCollider>().material = physicMat;
    }
    void Explode(Collision collision)
    {
        GameObject hitTarget = collision.collider.gameObject;
        // if player fired shot we need to destroy the enemy
        if (hitTarget.CompareTag("Enemy")){
            EnemyAI EnemyScript = hitTarget.GetComponent<EnemyAI>();
            EnemyScript.Death();
        }
        // if enemy fired shot we need to damage the player
        else if (hitTarget.CompareTag("Player")){
            Health playerHealth = hitTarget.GetComponent<Health>();
            if (playerHealth != null){
                playerHealth.takeDamage(1);
            }
        }   
        if (explosion != null) Instantiate(explosion, hitTarget.transform.position, Quaternion.identity);
        Invoke("Delay", 0.05f); 
    }
    void Start()
    {
        Setup();
    }
    void Awake()
    {
        timePassed = 0;
    }

    // Update is called once per frame
    void Update()
    {   
        timePassed += Time.deltaTime;
        if (timePassed >= lifetime){
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        //if (shooter == Shooter.Enemy){
        if (collision.collider.CompareTag("Player")){
            Explode(collision);    
        } 
        //}

        //else if (shooter == Shooter.Player){
        else if (collision.collider.CompareTag("Enemy")){
            Explode(collision);    
        } 
        //}     
        // destroy bullet with small delay to prevent bug
        else if (endOnCollision == true){
            Invoke("Delay", 0.05f); 
        }

    }

    private void Delay(){
        Destroy(gameObject);
    }
}
