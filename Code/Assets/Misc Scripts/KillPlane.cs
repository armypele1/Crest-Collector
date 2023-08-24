using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillPlane : MonoBehaviour
{
    [SerializeField] Vector3 resetCoords;
    [SerializeField] Quaternion resetRotation;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter (Collider other) 
     {
         if (other.gameObject.tag == "Player"){
            other.transform.position = resetCoords;
            other.transform.rotation = resetRotation;
         }
     }
}
