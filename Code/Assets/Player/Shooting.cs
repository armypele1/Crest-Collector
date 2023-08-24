using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shooting : MonoBehaviour
{
    public GameObject bullet;
    public float shootForce, upwardForce;
    public float timeBetweenShots, spread, reloadTime;
    public LayerMask player;
    Vector3 targetPoint;
    public GameObject rechargeMeter;
    
    bool readyToShoot, shooting; 

    public Camera playerCamera;
    public Transform attackPoint;

    public bool allowInvoke = true;
    private float time;
    // Start is called before the first frame update
    void Awake()
    {
        readyToShoot = true;
        if (rechargeMeter != null){
            rechargeMeter.GetComponent<Slider>().maxValue = timeBetweenShots;
        }
        time = timeBetweenShots;
    }

    // Update is called once per frame
    void Update()
    {
        if (readyToShoot && shooting){
            Shoot();
        }
        if (rechargeMeter != null){
            time = (time + Time.deltaTime);
            rechargeMeter.GetComponent<Slider>().value = time;
            if (rechargeMeter.GetComponent<Slider>().value >= timeBetweenShots){
                rechargeMeter.GetComponent<Slider>().value = timeBetweenShots;
                rechargeMeter.SetActive(false);
            }
            else{
                rechargeMeter.SetActive(true);
            }
        }
    }
    void Shoot()
    {
        time = 0;
        readyToShoot = false;
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(.5f,.5f,0));
        RaycastHit hit;

        //check if ray hit
        if (Physics.Raycast(ray, out hit, 1000f, ~player)){
            targetPoint = hit.point;
        }
        else{
            targetPoint = ray.GetPoint(75);
        }

        // Calculate direction from attackPoint
        Vector3 direction = targetPoint - attackPoint.position;

        // Instantiate bullet
        GameObject currentBullet = Instantiate(bullet, attackPoint.position, Quaternion.identity);
        attackPoint.forward = attackPoint.parent.forward;
        currentBullet.transform.forward = direction.normalized;

        // Add forces to bullet
        currentBullet.GetComponent<Rigidbody>().AddForce(direction.normalized * shootForce, ForceMode.Impulse);
        currentBullet.GetComponent<Rigidbody>().AddForce(playerCamera.transform.up * upwardForce, ForceMode.Impulse);

        //Invoke resetShot
        if (allowInvoke){
            Invoke("ResetShot", timeBetweenShots);
            // record time for the recharge meter
            allowInvoke = false;
        }
    }

    void ResetShot(){
        readyToShoot = true;
        allowInvoke = true;
        shooting = false;
    }
    public void OnShootPressed()
    {
        shooting = true;
    }
}
