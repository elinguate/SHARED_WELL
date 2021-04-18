using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurtainReveal : MonoBehaviour
{
    public Vector2 start;
    public Vector2 end;

    public float blend;
    public AnimationCurve blendCurve = AnimationCurve.EaseInOut(0, 0, 1, 1); 

    public float speed = 2.0f;

    void Start()
    {
        if (PlayerPrefs.GetInt("Died", -1) == 0)
        {
            blend = 1;
        }
        else
        {
            PlayerPrefs.SetInt("Died", 0);
        }
    }

    void Update()
    {
        blend += Time.deltaTime * speed;
        if (blend > 1)
        {
            Destroy(gameObject);
        }

        Vector3 cachePos = transform.position;
        cachePos.x = Mathf.Lerp(start.x, end.x, blendCurve.Evaluate(blend));
        cachePos.y = Mathf.Lerp(start.y, end.y, blendCurve.Evaluate(blend));
        transform.position = cachePos;
    }
}
