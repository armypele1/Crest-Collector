using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideDownSlopes : MonoBehaviour
{
    Vector3 hitNormal;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    void OnControllerColliderHit (ControllerColliderHit hit) {
        hitNormal = hit.normal;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
