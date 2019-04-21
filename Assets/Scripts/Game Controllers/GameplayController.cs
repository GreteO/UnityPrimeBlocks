using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameplayController : MonoBehaviour
{

    public static GameplayController instance;

    public static int gridWidth = 10;
    public static int gridHeight = 22;

    public static Transform[,] grid = new Transform[gridWidth, gridHeight];

    public int singleLineScore = 40;
    public int doubleLineScore = 100;
    public int tripleLineScore = 300;
    public int quadrupleLineScore = 1200;
    public int currentLevel = 0;

    public GameObject[] previewBlock;

    public GameObject pausePanel, gameOverPanel;

    public float fallSpeed;

    public Text scoreText, lineText, levelText, gameOverScoreText, highScoreText;

    public AudioClip[] scoreSounds;

    public bool gameOver, gameInprogress, gameInPause;

    private AudioSource audioSource;

    private string blockName;

    private int rowsNumber = 0;
    private int linesCleared = 0;

    private float delay;

    private GameObject nextBlock, ghostBlock;

    private bool gameStarted;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        CreateInstance();
    }


    // Use this for initialization
    void Start()
    {
        if (GameController.instance.isMusicOn)
        {
            MusicController.instance.GameplaySound();
        }
        else
        {
            MusicController.instance.StopAllSound();
        }

        InitializeGameVariables();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseButton();
        }
        UpdateGameplay();
    }

    void InitializeGameVariables()
    {
        gameInprogress = true;

        GameController.instance.currentScore = 0;
        currentLevel = GameController.instance.currentLevel;
        SpawnNextBlock();
    }

    void UpdateGameplay()
    {
        if (rowsNumber > 0)
        {
            if (rowsNumber == 1)
            {
                GameController.instance.currentScore += singleLineScore * (currentLevel + 1);
                if (GameController.instance.isMusicOn)
                {
                    audioSource.PlayOneShot(scoreSounds[0]);
                }
                linesCleared++;
            }
            else if (rowsNumber == 2)
            {
                GameController.instance.currentScore += doubleLineScore * (currentLevel + 1);
                if (GameController.instance.isMusicOn)
                {
                    audioSource.PlayOneShot(scoreSounds[1]);
                }
                linesCleared += 2;
            }
            else if (rowsNumber == 3)
            {
                GameController.instance.currentScore += tripleLineScore * (currentLevel + 1);
                if (GameController.instance.isMusicOn)
                {
                    audioSource.PlayOneShot(scoreSounds[2]);
                }
                linesCleared += 3;
            }
            else if (rowsNumber == 4)
            {
                GameController.instance.currentScore += quadrupleLineScore * (currentLevel + 1);
                if (GameController.instance.isMusicOn)
                {
                    audioSource.PlayOneShot(scoreSounds[3]);
                }
                linesCleared += 4;
            }

            rowsNumber = 0;
        }


        if (currentLevel <= linesCleared / 10)
        {
            currentLevel = linesCleared / 10;
        }


        fallSpeed = FallSpeedPerLevel();


        scoreText.text = GameController.instance.currentScore.ToString("N0");
        levelText.text = currentLevel.ToString();
        lineText.text = linesCleared.ToString();


    }

    public bool CheckOutsideGrid(Block block)
    {
        for (int x = 0; x < gridWidth; x++)
        {
            foreach (Transform newBlock in block.transform)
            {
                Vector2 position = Estimate(newBlock.position);
                if (position.y >= gridHeight)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public bool CheckFullRow(int y)
    {
        for (int x = 0; x < gridWidth; x++)
        {
            if (grid[x, y] == null)
            {
                return false;
            }
        }

        rowsNumber++;

        return true;
    }

    public void DeleteBlock(int y)
    {
        for (int x = 0; x < gridWidth; x++)
        {
            Destroy(grid[x, y].gameObject);
            grid[x, y] = null;
        }
    }

    public void MoveDown(int y)
    {
        for (int x = 0; x < gridWidth; x++)
        {
            if (grid[x, y] != null)
            {
                grid[x, y - 1] = grid[x, y];
                grid[x, y] = null;
                grid[x, y - 1].position += new Vector3(0, -1, 0);
            }
        }
    }

    public void MoveAllDown(int y)
    {
        for (int i = y; i < gridHeight; i++)
        {
            MoveDown(i);
        }
    }

    public void DeleteRow()
    {
        for (int y = 0; y < gridHeight; y++)
        {
            if (CheckFullRow(y))
            {
                DeleteBlock(y);
                MoveAllDown(y + 1);
                y--;
            }
        }
    }

    public void UpdateGrid(Block block)
    {
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                if (grid[x, y] != null)
                {
                    if (grid[x, y].parent == block.transform)
                    {
                        grid[x, y] = null;
                    }
                }
            }
        }

        foreach (Transform newBlock in block.transform)
        {
            Vector2 position = Estimate(newBlock.position);

            if (position.y < gridHeight)
            {
                grid[(int)position.x, (int)position.y] = newBlock;
            }
        }
    }


    public Transform GetGridPosition(Vector2 position)
    {
        if (position.y >= gridHeight)
        {
            return null;
        }
        else
        {
            return grid[(int)position.x, (int)position.y];
        }
    }

    void CreateInstance()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void SpawnNextBlock()
    {
        if (!gameStarted)
        {
            gameStarted = true;
            nextBlock = Instantiate(Resources.Load(GetRandomBlock(), typeof(GameObject)), new Vector2(5.0f, 22.0f), Quaternion.identity) as GameObject;
            blockName = GetPreviewBlock();
            SpawnGhostBlock();
        }
        else
        {
            nextBlock = Instantiate(Resources.Load(blockName, typeof(GameObject)), new Vector2(5.0f, 22.0f), Quaternion.identity) as GameObject;
            blockName = GetPreviewBlock();
            SpawnGhostBlock();
        }
    }


    public bool CheckBlockInside(Vector2 position)
    {
        return ((int)position.x >= 0 && (int)position.x < gridWidth && (int)position.y >= 0);
    }

    public Vector2 Estimate(Vector2 position)
    {
        return new Vector2(Mathf.Round(position.x), Mathf.Round(position.y));
    }

    private string GetPreviewBlock()
    {

        string prevBlockPath = "";

        switch (GetRandomBlock())
        {

            case "Prefabs/Blocks/Block J":
                for (int i = 0; i < previewBlock.Length; i++)
                {
                    if (i == 1)
                    {
                        previewBlock[i].SetActive(true);
                    }
                    else
                    {
                        previewBlock[i].SetActive(false);
                    }
                }

                prevBlockPath = "Prefabs/Blocks/Block J";

                break;

            case "Prefabs/Blocks/Block L":
                for (int i = 0; i < previewBlock.Length; i++)
                {
                    if (i == 2)
                    {
                        previewBlock[i].SetActive(true);
                    }
                    else
                    {
                        previewBlock[i].SetActive(false);
                    }
                }

                prevBlockPath = "Prefabs/Blocks/Block L";

                break;

            case "Prefabs/Blocks/Block T":
                for (int i = 0; i < previewBlock.Length; i++)
                {
                    if (i == 3)
                    {
                        previewBlock[i].SetActive(true);
                    }
                    else
                    {
                        previewBlock[i].SetActive(false);
                    }
                }

                prevBlockPath = "Prefabs/Blocks/Block T";

                break;

            case "Prefabs/Blocks/Block S":
                for (int i = 0; i < previewBlock.Length; i++)
                {
                    if (i == 5)
                    {
                        previewBlock[i].SetActive(true);
                    }
                    else
                    {
                        previewBlock[i].SetActive(false);
                    }
                }

                prevBlockPath = "Prefabs/Blocks/Block S";

                break;

            case "Prefabs/Blocks/Block Z":
                for (int i = 0; i < previewBlock.Length; i++)
                {
                    if (i == 4)
                    {
                        previewBlock[i].SetActive(true);
                    }
                    else
                    {
                        previewBlock[i].SetActive(false);
                    }
                }

                prevBlockPath = "Prefabs/Blocks/Block Z";

                break;

            case "Prefabs/Blocks/Block I":
                for (int i = 0; i < previewBlock.Length; i++)
                {
                    if (i == 0)
                    {
                        previewBlock[i].SetActive(true);
                    }
                    else
                    {
                        previewBlock[i].SetActive(false);
                    }
                }

                prevBlockPath = "Prefabs/Blocks/Block I";

                break;

            case "Prefabs/Blocks/Block O":
                for (int i = 0; i < previewBlock.Length; i++)
                {
                    if (i == 6)
                    {
                        previewBlock[i].SetActive(true);
                    }
                    else
                    {
                        previewBlock[i].SetActive(false);
                    }
                }

                prevBlockPath = "Prefabs/Blocks/Block O";

                break;
        }

        return prevBlockPath;
    }

    private string GetRandomBlock()
    {

        int randomBlock = Random.Range(1, 8);

        string blockPathName = "";


        switch (randomBlock)
        {
            case 1:
                blockPathName = "Prefabs/Blocks/Block J";
                break;

            case 2:
                blockPathName = "Prefabs/Blocks/Block L";
                break;

            case 3:
                blockPathName = "Prefabs/Blocks/Block O";
                break;

            case 4:
                blockPathName = "Prefabs/Blocks/Block I";
                break;

            case 5:
                blockPathName = "Prefabs/Blocks/Block S";
                break;

            case 6:
                blockPathName = "Prefabs/Blocks/Block Z";
                break;

            case 7:
                blockPathName = "Prefabs/Blocks/Block T";
                break;
        }


        return blockPathName;
    }

    private float FallSpeedPerLevel()
    {
        float speed = 0;

        switch (currentLevel)
        {

            case 0:
                speed = 0.8f;
                break;

            case 1:
                speed = 0.72f;
                break;

            case 2:
                speed = 0.63f;
                break;

            case 3:
                speed = 0.55f;
                break;

            case 4:
                speed = 0.47f;
                break;

            case 5:
                speed = 0.38f;
                break;

            case 6:
                speed = 0.3f;
                break;

            case 7:
                speed = 0.22f;
                break;

            case 8:
                speed = 0.13f;
                break;

            case 9:
                speed = 0.1f;
                break;

            case 10:
                speed = 0.08f;
                break;
            case 13:
                speed = 0.07f;
                break;

            case 16:
                speed = 0.05f;
                break;

            case 19:
                speed = 0.03f;
                break;

            case 29:
                speed = 0.02f;
                break;
        }

        return speed;
    }


    public void GameOver()
    {
        gameOver = true;
        gameOverPanel.SetActive(true);
        gameOverScoreText.text = GameController.instance.currentScore.ToString("N0");
        if (GameController.instance.isMusicOn)
        {
            if (MusicController.instance.audioSource.isPlaying)
            {
                MusicController.instance.StopAllSound();
                AudioSource.PlayClipAtPoint(MusicController.instance.audioClips[2], Camera.main.transform.position);
            }
        }
        if (GameController.instance.currentScore >= GameController.instance.highScore)
        {
            highScoreText.gameObject.SetActive(true);
            GameController.instance.highScore = GameController.instance.currentScore;
            GameController.instance.Save();
        }
    }

    public void SpawnGhostBlock()
    {
        if (GameObject.FindGameObjectWithTag("CloneBlock") != null)
        {
            Destroy(GameObject.FindGameObjectWithTag("CloneBlock"));
        }

        ghostBlock = Instantiate(nextBlock, nextBlock.transform.position, Quaternion.identity);
        Destroy(ghostBlock.GetComponent<Block>());
        ghostBlock.AddComponent<GhostBlock>();




    }

    public void PauseButton()
    {
        if (gameInprogress)
        {
            if (!gameInPause)
            {
                gameInprogress = false;
                gameInPause = true;
                pausePanel.SetActive(true);
            }
        }
    }

    public void ResumeButton()
    {
        if (!gameInprogress)
        {
            if (gameInPause)
            {
                gameInprogress = true;
                gameInPause = false;
                pausePanel.SetActive(false);
            }
        }
    }

    public void QuitButton()
    {
        if (MusicController.instance.audioSource.isPlaying)
        {
            MusicController.instance.StopAllSound();
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void RestartButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
