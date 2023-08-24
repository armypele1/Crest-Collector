using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    Vector3 startPos;
    [SerializeField] Vector3 endPos;
    [SerializeField] float speed = 1f;
    [SerializeField] bool repeatMovement = true;
    [SerializeField] bool autoStart = false;
    [SerializeField] GameObject player;

    private float startTime;
    private float journeyDistance;
    private bool continueMovement = true;
    private Vector3 newPosition;
    private Vector3 deltaMove;

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        startTime = Time.time;
        journeyDistance = Vector3.Distance(startPos, endPos);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!autoStart){
            startTime = Time.time;
            return;
        }
        // Move platform a suitable fraction of the total distance
        float distCovered = (Time.time - startTime) * speed;
        float fractionOfJourney = distCovered / journeyDistance;
        newPosition = Vector3.Lerp(startPos, endPos, fractionOfJourney);
        deltaMove = newPosition - transform.position;
        transform.position += deltaMove;

        if (repeatMovement && transform.position == endPos){
            Vector3 temp = startPos;
            startPos = endPos;
            endPos = temp;
            startTime = Time.time;
        }
    }
    void OnTriggerStay (Collider other) 
     {
        autoStart = true;
         if (other.gameObject.tag == "Player"){
            //print("test");
            other.transform.position += deltaMove;
         }
     }

    void OnTriggerExit (Collider other) 
        {   
            if (other.gameObject.tag == "Player"){
            }
            
        }

}
