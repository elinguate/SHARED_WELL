using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionSpawner : MonoBehaviour
{
    public StickyJumper attachedPlayer;

    public Vector3 startPosition;

    void OnValidate()
    {
        if (!attachedPlayer)
        {
            attachedPlayer = FindObjectOfType<StickyJumper>();
        }
    }

    void Start()
    {
        if (PlayerPrefs.GetInt("First_Load", 1) == 1)
        {
            attachedPlayer.jumpsLeft = attachedPlayer.startingJumpsLeft;
            return;
        }

        startPosition = attachedPlayer.transform.position;

        float velocityX = PlayerPrefs.GetFloat("Velocity_X", 0);
        float velocityY = PlayerPrefs.GetFloat("Velocity_Y", 0);

        float positionX = PlayerPrefs.GetFloat("Position_X", 0);
        float positionY = PlayerPrefs.GetFloat("Position_Y", 0);

        //Debug.Log("Vel: " + velocityX.ToString("F1") + ", " + velocityY.ToString("F1"));
        //Debug.Log("Pos: " + positionX.ToString("F1") + ", " + positionY.ToString("F1"));

        int direction = PlayerPrefs.GetInt("Direction", -1);
        positionX = (direction == 1 || direction == 3) ? -positionX - Mathf.Sign(positionX) * 1.5f : positionX;
        positionY = (direction == 0 || direction == 2) ? -positionY - Mathf.Sign(positionY) * 1.5f : positionY;

        attachedPlayer.jumpsLeft = PlayerPrefs.GetInt("Jumps_Left", -1);

        attachedPlayer.velocity = new Vector2(velocityX, velocityY);
        attachedPlayer.transform.position = new Vector3(positionX, positionY, attachedPlayer.transform.position.z);
    }

    void Update()
    {
        if (Application.isEditor && Input.GetKeyDown(KeyCode.R))
        {
            PlayerPrefs.DeleteAll(); 
            attachedPlayer.transform.position = startPosition;
            attachedPlayer.stuck = false;
            attachedPlayer.velocity = Vector2.zero;
        }
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("SPLASH");
        }
    }
}
