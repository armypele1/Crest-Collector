using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public static int difficulty = 1;
    public static float sensitivity = 0.1f;

    public void SetDifficulty(){
        float difficultyChoice = GameObject.Find("DifficultySlider").GetComponent<Slider>().value;
        difficulty = ((int)difficultyChoice);
    }

    public void SetSensitivity(){
        float sensChoice = GameObject.Find("SensSlider").GetComponent<Slider>().value;
        sensitivity = sensChoice;
    }
    
}
