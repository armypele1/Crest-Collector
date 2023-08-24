using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockedDoor : MonoBehaviour
{
    [SerializeField] int scoreToOpen;
    [SerializeField] GameObject requiredScoreText;
    private bool locked;
    private Rigidbody doorBody;
    // Start is called before the first frame update
    void Start()
    {
        locked = true;
        doorBody = gameObject.GetComponent<Rigidbody>();
        doorBody.isKinematic = true;
        requiredScoreText.GetComponent<TMPro.TextMeshPro>().text = "<sprite=0> " + scoreToOpen;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        int currentScore = ScoringSystem.theScore;
        if (currentScore >= scoreToOpen && locked == true){
            print("A door has unlocked!");
            doorBody.isKinematic = false;
            locked = false;
        }
    }
}
