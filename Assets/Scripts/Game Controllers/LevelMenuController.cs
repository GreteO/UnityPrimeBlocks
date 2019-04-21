using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class LevelMenuController : MonoBehaviour
{

    public Slider levelMeter;

    public Text levelText;
    public Text highScore;

    private int level;
    void Start()
    {
        if (GameController.instance.isMusicOn)
        {
            MusicController.instance.PlayBgMusic();
        }
        else
        {
            MusicController.instance.StopAllSound();
        }

        InitializeLevelController();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        }

        UpdateLevelMenu();
    }

    void InitializeLevelController()
    {
        highScore.text = GameController.instance.highScore.ToString("N0");
    }

    void UpdateLevelMenu()
    {
        level = (int)levelMeter.value;
        GameController.instance.currentLevel = level;
        levelText.text = level.ToString();
    }

    public void PlayButton()
    {
        MusicController.instance.audioSource.Stop();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
