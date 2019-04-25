using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Block : MonoBehaviour
{

    public bool canRotate;
    public bool minRotate;

    public AudioClip moveSound;
    public AudioClip rotateSound;
    public AudioClip landSound;

    private AudioSource audioSource;

    private float fallSpeed;
    private float fall;
    private float verticalMoveSpeed = 0.02f;
    private float horizontalMoveSpeed = 0.03f;
    private float holdDownDelay = 0.2f;
    private float verticalDelay = 0;
    private float horizontalDelay = 0;
    private float holdDownTimerHorizontal = 0;
    private float holdDownTimerVertical = 0;

    private bool moveHorizontal;
    private bool moveVertical;
    private bool moved = false;
    private bool inPositionBlock;

    private int touchSensitivityHorizontal = 4;
    private int touchSensitivityVertical = 4;

    private Vector2 prevousUnitPosition = Vector2.zero;
    private Vector2 direction = Vector2.zero;
    // Start is called before the first frame update
    void Awake()
    {
        tag = "CurrentBlock";
        audioSource = GetComponent<AudioSource>();
        fallSpeed = GameplayController.instance.fallSpeed;
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!GameplayController.instance.gameOver && GameplayController.instance.gameInprogress)
        {
            if (!inPositionBlock)
            {
                PlayerController();
                TouchScreen();
            }
            else
            {
                DestroyBlocks();
            }
        }


    }

    void DestroyBlocks()
    {
        if (transform.childCount <= 0)
        {
            Destroy(gameObject);
        }
    }

    void TouchScreen()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                prevousUnitPosition = new Vector2(touch.position.x, touch.position.y);
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                Vector2 touchDeltaPosition = touch.deltaPosition;
                direction = touchDeltaPosition.normalized;

                if (Mathf.Abs(touch.position.x - prevousUnitPosition.x) >= touchSensitivityHorizontal && direction.x < 0 && touchDeltaPosition.y > -10 && touchDeltaPosition.y < 10)
                {
                    MoveLeft();
                    prevousUnitPosition = touch.position;
                    moved = true;
                }
                else if (Mathf.Abs(touch.position.x - prevousUnitPosition.x) >= touchSensitivityHorizontal && direction.x > 0 && touch.deltaPosition.y > -10 && touch.deltaPosition.y < 10)
                {
                    MoveRight();
                    prevousUnitPosition = touch.position;
                    moved = true;
                }
                else if (Mathf.Abs(touch.position.y - prevousUnitPosition.y) >= touchSensitivityVertical && direction.y < 0 && touchDeltaPosition.x > -10 && touchDeltaPosition.x < 10)
                {
                    MoveDown();
                    prevousUnitPosition = touch.position;
                    moved = true;
                }

            }
            else if (touch.phase == TouchPhase.Ended)
            {
                if (!moved && touch.position.x > Screen.width / 4)
                {
                    Rotate();
                }

                moved = false;
            }
        }

        if (Time.time - fall >= fallSpeed)
        {
            MoveDown();
        }
    }

    void PlayerController()
    {
        if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.LeftArrow))
        {
            moveHorizontal = false;
            horizontalDelay = 0;
            holdDownTimerHorizontal = 0;
        }

        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            moveVertical = false;
            horizontalDelay = 0;
            holdDownTimerVertical = 0;
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            MoveRight();
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            MoveLeft();
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Rotate();
        }

        if (Input.GetKey(KeyCode.DownArrow) || Time.time - fall >= fallSpeed)
        {
            MoveDown();
        }
    }

    private bool CheckCorrectPosition()
    {
        foreach (Transform block in transform)
        {

            Vector2 position = GameplayController.instance.Estimate(block.position);

            if (GameplayController.instance.CheckBlockInside(position) == false)
            {
                return false;
            }

            if (GameplayController.instance.GetGridPosition(position) != null && GameplayController.instance.GetGridPosition(position).parent != transform)
            {
                return false;
            }
        }

        return true;
    }

    void MoveLeft()
    {
        if (moveHorizontal)
        {
            if (holdDownTimerHorizontal < holdDownDelay)
            {
                holdDownTimerHorizontal += Time.deltaTime;
                return;
            }

            if (horizontalDelay < horizontalMoveSpeed)
            {
                horizontalDelay += Time.deltaTime;
                return;
            }

        }

        if (!moveHorizontal)
        {
            moveHorizontal = true;
        }

        horizontalDelay = 0;


        transform.position += new Vector3(-1, 0, 0);

        if (CheckCorrectPosition())
        {
            GameplayController.instance.UpdateGrid(this);
            if (GameController.instance.isMusicOn)
            {
                audioSource.PlayOneShot(moveSound);
            }
        }
        else
        {
            transform.position += new Vector3(1, 0, 0);
        }


    }

    void MoveRight()
    {
        if (moveHorizontal)
        {
            if (holdDownTimerHorizontal < holdDownDelay)
            {
                holdDownTimerHorizontal += Time.deltaTime;
                return;
            }

            if (horizontalDelay < horizontalMoveSpeed)
            {
                horizontalDelay += Time.deltaTime;
                return;
            }

        }

        if (!moveHorizontal)
        {
            moveHorizontal = true;
        }

        horizontalDelay = 0;

        transform.position += new Vector3(1, 0, 0);

        if (CheckCorrectPosition())
        {
            GameplayController.instance.UpdateGrid(this);
            if (GameController.instance.isMusicOn)
            {
                audioSource.PlayOneShot(moveSound);
            }
        }
        else
        {
            transform.position += new Vector3(-1, 0, 0);
        }

    }

    void MoveDown()
    {
        if (moveVertical)
        {

            if (holdDownTimerVertical < holdDownDelay)
            {
                holdDownTimerVertical += Time.deltaTime;
                return;
            }


            if (verticalDelay < verticalMoveSpeed)
            {
                verticalDelay += Time.deltaTime;
                return;
            }

        }

        if (!moveVertical)
        {
            moveVertical = true;
        }

        verticalDelay = 0;


        transform.position += new Vector3(0, -1, 0);

        if (CheckCorrectPosition())
        {
            GameplayController.instance.UpdateGrid(this);

        }
        else
        {

            transform.position += new Vector3(0, 1, 0);

            GameplayController.instance.DeleteRow();

            if (GameplayController.instance.CheckOutsideGrid(this))
            {
                GameplayController.instance.GameOver();
            }

            if (!GameplayController.instance.gameOver)
            {
                GameController.instance.currentScore += Random.Range(1, 15);
            }
            tag = "InPositionBlock";

            inPositionBlock = true;

            if (GameController.instance.isMusicOn)
            {
                audioSource.PlayOneShot(landSound);
            }

            GameplayController.instance.SpawnNextBlock();


        }
        fall = Time.time;
    }

    void Rotate()
    {
        if (canRotate)
        {
            if (minRotate)
            {

                if (transform.rotation.eulerAngles.z >= 90)
                {
                    transform.Rotate(0, 0, -90);

                }
                else
                {
                    transform.Rotate(0, 0, 90);
                }

            }
            else
            {

                transform.Rotate(0, 0, 90);
            }


            if (CheckCorrectPosition())
            {
                GameplayController.instance.UpdateGrid(this);
                if (GameController.instance.isMusicOn)
                {
                    audioSource.PlayOneShot(rotateSound);
                }
            }
            else
            {

                if (minRotate)
                {
                    if (transform.rotation.eulerAngles.z >= 90)
                    {
                        transform.Rotate(0, 0, -90);

                    }
                    else
                    {
                        transform.Rotate(0, 0, 90);
                    }
                }
                else
                {
                    transform.Rotate(0, 0, -90);
                }


            }



        }
    }
}
