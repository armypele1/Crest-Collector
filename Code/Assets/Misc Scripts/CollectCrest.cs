using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectCrest : MonoBehaviour
{
    public AudioSource collectSound;
    public GameObject player;
    Collider playerCollider;
    Health playerHealth;

    void Start()
    {
        playerHealth = player.GetComponent<Health>();
    }
    void OnTriggerEnter(Collider playerCollider){
        if (playerCollider.CompareTag("Player")){
            collectSound.Play();
            ScoringSystem.theScore += 1;
            playerHealth.FullHeal();
            Destroy(gameObject);
        }
        
    }
}
