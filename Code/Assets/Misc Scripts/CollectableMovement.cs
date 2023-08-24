using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableMovement : MonoBehaviour
{
    float initialYPos = 0;
    float timer = 0;
    [SerializeField] float bobAmount = 1f;
    [SerializeField] float bobSpeed = 1f;
    [SerializeField] float rotationAmount = 1f;

    // Start is called before the first frame update
    void Start()
    {
        initialYPos = gameObject.transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        // bobbing
        timer += Time.deltaTime * bobSpeed;
        gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x, initialYPos + Mathf.Sin(timer) * bobAmount, gameObject.transform.localPosition.z);

        // rotation    
        gameObject.transform.Rotate(0, Time.deltaTime * rotationAmount, 0, Space.Self);
    }
}
