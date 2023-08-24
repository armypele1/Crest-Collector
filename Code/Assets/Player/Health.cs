using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
{
    int maxHealth;
    int currentHealth;
    public GameObject healthText;
    public GameObject healthSlider;
    public GameObject playerCamera;
    public AudioSource hitSound;
    public AudioSource deathSound;

    Shake cameraShake;
    
    // Start is called before the first frame update
    void Start()
    {
        if (SettingsMenu.difficulty == 1) maxHealth = 5;
        else if (SettingsMenu.difficulty == 2) maxHealth = 3;
        else if (SettingsMenu.difficulty == 3) maxHealth = 1;
        currentHealth = maxHealth;
        cameraShake = playerCamera.GetComponent<Shake>();
        if (healthSlider != null){
            healthSlider.GetComponent<Slider>().maxValue = maxHealth;
        }
    }

    public void FullHeal(){
        currentHealth = maxHealth;
    }

    public void takeDamage(int damage){
        currentHealth -= damage;
        hitSound.Play();

        if (cameraShake != null){
            cameraShake.start = true;
        }
        if (currentHealth <= 0){
            Invoke("Death", 0.1f);
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (healthText != null){
            healthText.GetComponent<TMPro.TextMeshProUGUI>().text = currentHealth.ToString();
        }
        if (healthSlider != null){
            healthSlider.GetComponent<Slider>().value = currentHealth;
        }
    }

    void Death(){
        Cursor.visible = true;
        ScoringSystem.theScore = 0;
        SceneManager.LoadScene(2);
    }
}
