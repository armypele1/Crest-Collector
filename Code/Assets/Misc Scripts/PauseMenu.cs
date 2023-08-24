using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public InputManager inputManager;
    public GameObject pauseMenu;
    public bool isPaused;

    // Start is called before the first frame update
    void Start()
    {
        pauseMenu.SetActive(false);
        
    }

    void Awake(){
        GameObject.Find("SensSlider").GetComponent<Slider>().value = SettingsMenu.sensitivity;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PauseGame(){
        inputManager.DisableMovement();
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        Cursor.visible = true;
        isPaused = true;
    }

    public void ResumeGame(){
        inputManager.EnableMovement();
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        Cursor.visible = false;
        isPaused = false;
    }
    
    public void GoToMainMenu(){
        ScoringSystem.theScore = 0;
        Time.timeScale = 1f;
        isPaused = false;
        SceneManager.LoadScene(0);
    }

    public void SetSensitivity(){
        float sensChoice = GameObject.Find("SensSlider").GetComponent<Slider>().value;
        SettingsMenu.sensitivity = sensChoice;
    }

    public void OnPausePressed(PlayerControls.GroundMovementActions groundMovement){
        if (isPaused){
            //groundMovement.Enable();
            ResumeGame();
        }
        else{
            //groundMovement.Disable();
            PauseGame();
        }
    }
}
