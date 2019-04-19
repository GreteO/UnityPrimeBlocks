using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    public bool isMusicOn;
    public bool isGameStartedFirstTime;

    public int highScore;
    public int currentScore;
    public int currentLevel;

    private GameData data;

    void Awake()
    {
        CreateInstance();
        InitializeGameVariables();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void CreateInstance()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void InitializeGameVariables()
    {
        Load();

        if (data != null)
        {
            isGameStartedFirstTime = data.GetIsGameStartedFirstTime();
        }
        else
        {
            isGameStartedFirstTime = true;
        }

        if (isGameStartedFirstTime)
        {
            isMusicOn = true;
            isGameStartedFirstTime = false;
            highScore = 0;

            data = new GameData();
            data.SetIsMusicOn(isMusicOn);
            data.SetIsGameStartedFirstTime(isGameStartedFirstTime);
            data.SetHighScore(highScore);

            Save();

            Load();
        }
        else
        {
            highScore = data.GetHighScore();
            isGameStartedFirstTime = data.GetIsGameStartedFirstTime();
            isMusicOn = data.GetIsMusicOn();
        }

    }


    public void Save()
    {
        FileStream file = null;

        try
        {
            BinaryFormatter bf = new BinaryFormatter();
            file = File.Create(Application.persistentDataPath + "/save.dat");

            if (data != null)
            {
                data.SetHighScore(highScore);
                data.SetIsGameStartedFirstTime(isGameStartedFirstTime);
                data.SetIsMusicOn(isMusicOn);
                bf.Serialize(file, data);
            }

        }
        catch (Exception e)
        {
            Debug.LogException(e, this);

        }
        finally
        {
            if (file != null)
            {
                file.Close();
            }
        }
    }

    public void Load()
    {
        FileStream file = null;

        try
        {
            BinaryFormatter bf = new BinaryFormatter();
            file = File.Open(Application.persistentDataPath + "/save.dat", FileMode.Open);
            data = bf.Deserialize(file) as GameData;
        }
        catch (Exception e)
        {
            Debug.LogException(e, this);
        }
        finally
        {
            if (file != null)
            {
                file.Close();
            }
        }
    }


    [Serializable]
    class GameData
    {
        private bool isGameStartedFirstTime;
        private bool isMusicOn;

        private int highScore;

        public void SetIsGameStartedFirstTime(bool isGameStartedFirstTime)
        {
            this.isGameStartedFirstTime = isGameStartedFirstTime;
        }

        public bool GetIsGameStartedFirstTime()
        {
            return this.isGameStartedFirstTime;
        }

        public void SetIsMusicOn(bool isMusicOn)
        {
            this.isMusicOn = isMusicOn;
        }

        public bool GetIsMusicOn()
        {
            return this.isMusicOn;
        }

        public void SetHighScore(int highScore)
        {
            this.highScore = highScore;
        }

        public int GetHighScore()
        {
            return this.highScore;
        }
    }


    // Update is called once per frame
//    void Update()
  //  {
        
    //}
}
