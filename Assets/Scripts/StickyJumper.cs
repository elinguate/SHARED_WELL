using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StickyJumper : MonoBehaviour
{
    public int startingJumpsLeft = 10;
    public int jumpsLeft = 0;
    public SpriteRenderer jumpsLeftDisplay;
    public Sprite[] jumpsLeftText;

    public int airJumps = 0;

    [Header("Jumping")]
    public bool stuck = false;
    public Transform stuckParent;
    public GameObject stuckEmpty;

    public float jumpConsumeTimer = 0;
    public float jumpConsumeLength = 0.1f;
    public bool awaitingJumpConsume = false;

    public Vector2 velocity;

    public float jumpDistanceCap = 4.0f;
    public float jumpSpeed = 16.0f;
    public float gravity = 10.0f;

    public float skinWidth = 0.01f;

    public SpriteRenderer playerDisplay;
    public Sprite defaultSprite;
    public Sprite squashSprite;

    [Header("Cursor")]
    public Camera mainCamera;

    public Vector2 mouseDirection;

    public SpriteRenderer trueMouseCursor;

    public SpriteRenderer mouseCursor;
    public bool cursorLocked = true;
    public Sprite enabledCursorSprite;
    public Sprite disabledCursorSprite;

    public Color enabledCursorColor = Color.white;
    public Color disabledCursorColor = Color.white;

    public BoxCollider2D attachedCollider;

    public LayerMask hittableLayerMask;
    public LayerMask transitionLayerMask;
    public LayerMask collectibleLayerMask;
    public LayerMask dangerLayerMask;
    
    void OnValidate()
    {
        if (Application.isPlaying)
        {
            return;
        }
        
        if (!mainCamera)
        {
            mainCamera = FindObjectOfType<Camera>();
        }
        if (!mouseCursor)
        {
            mouseCursor = GameObject.Find("Cursor").GetComponent<SpriteRenderer>();
        }
        if (!trueMouseCursor)
        {
            trueMouseCursor = GameObject.Find("True Cursor").GetComponent<SpriteRenderer>();
        }
    }

    void Awake()
    {
        LockCursor();
    }

    void Start()
    {
        airJumps = PlayerPrefs.GetInt("Air_Jumps", 0);
    }

    void LockCursor()
    {
        //Cursor.lockState = cursorLocked ? CursorLockMode.Confined : CursorLockMode.None;
        Cursor.visible = !cursorLocked;
    }

    void Update()
    {
        Vector2 scale = transform.localScale * attachedCollider.size;

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            cursorLocked = !cursorLocked;
            LockCursor();
        }

        if (cursorLocked)
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos = mainCamera.ScreenToWorldPoint(mousePos);
            mousePos.z = trueMouseCursor.transform.position.z;
            trueMouseCursor.transform.position = mousePos;

            mouseDirection = mousePos - transform.position;
            if (mouseDirection.magnitude > jumpDistanceCap)
            {
                mouseDirection = mouseDirection.normalized * jumpDistanceCap;
            }

            mouseCursor.transform.position = new Vector3(transform.position.x + mouseDirection.x, transform.position.y + mouseDirection.y, mouseCursor.transform.position.z);

            mouseCursor.sprite = stuck ? enabledCursorSprite : disabledCursorSprite;
            mouseCursor.color = stuck ? enabledCursorColor : disabledCursorColor;
        }

        if (!stuck)
        {
            velocity.y -= gravity * Time.deltaTime;
            
            jumpConsumeTimer -= Time.deltaTime;
            if (awaitingJumpConsume && jumpConsumeTimer < 0)
            {
                jumpsLeft -= 1;
                awaitingJumpConsume = false;
            }

            /*
            if (airJumps > 0 && Input.GetMouseButtonDown(0) && jumpsLeft > 0)
            {
                velocity = mouseDirection * jumpSpeed;

                jumpsLeft -= 1;
                airJumps -= 1;
            }*/

            Vector2 move = velocity * Time.deltaTime;
            Vector2 safeMove = move;
            if (safeMove.magnitude < skinWidth)
            {
                safeMove = safeMove.normalized * skinWidth;
            }

            RaycastHit2D hit = Physics2D.BoxCast(transform.position, scale, transform.localEulerAngles.z, safeMove.normalized, safeMove.magnitude, hittableLayerMask);
            Vector2 offset = new Vector2(hit.point.x - transform.position.x, hit.point.y - transform.position.y);
            float dot = Vector2.Dot(offset.normalized, velocity.normalized);
            if (hit && dot > 0.1f)
            {
                move = move.normalized * Mathf.Max(0, hit.distance - 0.01f);               
                stuck = true; 
            }
            transform.position += new Vector3(move.x, move.y, 0);

            if (stuck)
            {
                stuckEmpty = new GameObject("Stuck Empty Point");
                stuckEmpty.transform.position = transform.position;
                stuckEmpty.transform.parent = hit.transform;
                
                float rot = 0;
                if (Mathf.Abs(offset.x) > Mathf.Abs(offset.y))
                {
                    rot = -90;
                    rot += offset.x < 0 ? 0 : 180;
                }
                else
                {
                    rot += offset.y < 0 ? 0 : 180;
                }
                transform.localEulerAngles = new Vector3(0, 0, rot);
                playerDisplay.sprite = squashSprite;

                airJumps = PlayerPrefs.GetInt("Air_Jumps", 0);
            }
        }
        else
        {
            transform.position = stuckEmpty.transform.position;
            if (Input.GetMouseButtonDown(0) && jumpsLeft > 0)
            {
                velocity = mouseDirection * jumpSpeed;
                Destroy(stuckEmpty);

                jumpConsumeTimer = jumpConsumeLength;
                awaitingJumpConsume = true;

                playerDisplay.sprite = defaultSprite;
                stuck = false;
            }
        }

        Collider2D transitionHit = Physics2D.OverlapBox(transform.position, scale, transform.localEulerAngles.z, transitionLayerMask);
        if (transitionHit)
        {
            transitionHit.GetComponent<SceneTransition>().TransitionScene(velocity);
        }

        Collider2D collectibleHit = Physics2D.OverlapBox(transform.position, scale, transform.localEulerAngles.z, collectibleLayerMask);
        if (collectibleHit)
        {
            jumpsLeft = 9;
        }

        Collider2D dangerHit = Physics2D.OverlapBox(transform.position, scale, transform.localEulerAngles.z, dangerLayerMask);
        if (dangerHit || (jumpsLeft <= 0 && stuck))
        {
            PlayerPrefs.SetInt("Died", 1);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        jumpsLeftDisplay.sprite = jumpsLeftText[Mathf.Clamp(jumpsLeft, 0, 9)];
    }
}
