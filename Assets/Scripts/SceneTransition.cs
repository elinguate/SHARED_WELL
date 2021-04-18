using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public StickyJumper attachedPlayer;

    public int direction;
    private Vector2[] directionTests = new Vector2[4]
    {
        new Vector2(0, 1),
        new Vector2(1, 0),
        new Vector2(0, -1),
        new Vector2(-1, 0)
    };

    void Reset()
    {
        OnValidate();
    }

    void OnValidate()
    {
        if (!attachedPlayer)
        {
            attachedPlayer = FindObjectOfType<StickyJumper>();
        }

        float[] rotations = new float[4]
        {
            0, -90, 180, 90
        };

        transform.localEulerAngles = new Vector3(0, 0, rotations[direction]);
    }

    void OnDrawGizmos()
    {
        float[] rotations = new float[4]
        {
            0, -90, 180, 90
        };

        Vector3[] arrowPoints = new Vector3[4]
        {
            new Vector3(0, 0, 0),
            new Vector3(0, 3, 0),
            new Vector3(0.25f, 2.75f, 0),
            new Vector3(-0.25f, 2.75f, 0)
        };

        for (int i = 0; i < arrowPoints.Length; i++)
        {
            arrowPoints[i] = Quaternion.Euler(0, 0, rotations[direction]) * arrowPoints[i];
            arrowPoints[i] += transform.position;
        }

        Gizmos.color = Color.yellow;
        
        Gizmos.DrawLine(arrowPoints[0], arrowPoints[1]);
        Gizmos.DrawLine(arrowPoints[1], arrowPoints[2]);
        Gizmos.DrawLine(arrowPoints[1], arrowPoints[3]);
    }

    public void TransitionScene(Vector2 _velocity)
    {
        if (Vector2.Dot(_velocity.normalized, directionTests[direction]) < 0)
        {
            return;
        }

        PlayerPrefs.SetInt("First_Load", 0);

        PlayerPrefs.SetFloat("Velocity_X", attachedPlayer.velocity.x);
        PlayerPrefs.SetFloat("Velocity_Y", attachedPlayer.velocity.y);

        PlayerPrefs.SetFloat("Position_X", attachedPlayer.transform.position.x);
        PlayerPrefs.SetFloat("Position_Y", attachedPlayer.transform.position.y);

        PlayerPrefs.SetInt("Direction", direction);

        PlayerPrefs.SetInt("Jumps_Left", attachedPlayer.jumpsLeft);

        string currentSceneName = SceneManager.GetActiveScene().name;
        int worldX = int.Parse(currentSceneName.Split('_')[0]);
        int worldY = int.Parse(currentSceneName.Split('_')[1]);

        //Debug.Log("current: " + worldX + "," + worldY);
        
        worldY += direction == 0 ? 1 : 0;
        worldX += direction == 1 ? 1 : 0;
        worldY += direction == 2 ? -1 : 0;
        worldX += direction == 3 ? -1 : 0;
        
        //Debug.Log("loading: " + worldX + "," + worldY);

        SceneManager.LoadScene(worldX + "_" + worldY);
    }
}
