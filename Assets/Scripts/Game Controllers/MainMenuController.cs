using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{

    public GameObject optionPanel, exitPanel;

    public Toggle musicToggle;
    // Start is called before the first frame update
    // Use this for initialization
    void Start()
    {
        if (GameController.instance.isMusicOn)
        {
            MusicController.instance.PlayBgMusic();
            musicToggle.isOn = true;
        }
        else
        {
            MusicController.instance.StopAllSound();
            musicToggle.isOn = false;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (exitPanel.activeInHierarchy)
            {
                exitPanel.SetActive(false);
            }
            else
            {
                exitPanel.SetActive(true);
            }

            if (optionPanel.activeInHierarchy)
            {
                optionPanel.SetActive(false);
            }
        }
    }

    public void StartButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void OptionButton()
    {
        optionPanel.SetActive(true);
    }

    public void CloseButton()
    {
        optionPanel.SetActive(false);
    }

    public void MusicButton()
    {
        if (musicToggle.isOn)
        {
            MusicController.instance.PlayBgMusic();
            GameController.instance.isMusicOn = true;
            GameController.instance.Save();
        }
        else
        {
            MusicController.instance.StopAllSound();
            GameController.instance.isMusicOn = false;
            GameController.instance.Save();
        }
    }

    public void YesButton()
    {
        Application.Quit();
    }

    public void NoButton()
    {
        exitPanel.SetActive(false);
    }
}