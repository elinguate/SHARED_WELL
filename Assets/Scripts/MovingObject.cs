using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObject : MonoBehaviour
{
    public Vector2 start;
    public Vector2 end;

    public float frequency = 1.0f;

    public float overshoot = 1.5f;
    public float startOffset = 0;
    public float blend = 0;

    public float sceneLoadTime = 0;

    void Awake()
    {
        //sceneLoadTime = Time.time;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        Gizmos.DrawLine(start, end);
    }

    void OnValidate()
    {
        blend = -overshoot * Mathf.Cos(startOffset * Mathf.PI) * 0.5f + 0.5f;
        blend = Mathf.Clamp(blend, 0, 1);

        Vector3 cachePos = transform.position;
        cachePos.x = Mathf.Lerp(start.x, end.x, blend);
        cachePos.y = Mathf.Lerp(start.y, end.y, blend);
        transform.position = cachePos;
    }

    void Update()
    {
        blend = -overshoot * Mathf.Cos(((Time.time - sceneLoadTime) * frequency + startOffset) * Mathf.PI) * 0.5f + 0.5f;
        blend = Mathf.Clamp(blend, 0, 1);

        Vector3 cachePos = transform.position;
        cachePos.x = Mathf.Lerp(start.x, end.x, blend);
        cachePos.y = Mathf.Lerp(start.y, end.y, blend);
        transform.position = cachePos;
    }
}
