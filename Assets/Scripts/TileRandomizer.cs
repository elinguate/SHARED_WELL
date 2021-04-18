using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileRandomizer : MonoBehaviour
{
    public LayerMask environmentLayerMask;

    public float tileSize = 1.0f;
    
    public bool regenerate = false;

    public int randomSeed;

    public SpriteRenderer attachedRenderer;

    public Sprite[] open4Sprites;
    public Sprite[] open3Sprites;
    public Sprite[] open2ASprites;
    public Sprite[] open2OSprites;
    public Sprite[] open1Sprites;
    public Sprite[] open0Sprites;

    public void Initialize()
    {
        Reset();
        regenerate = true;
        OnValidate();
    }

    void Reset()
    {
        randomSeed = Random.Range(0, int.MaxValue);
    }

    void OnValidate()
    {
        if (!regenerate)
        {
            return;
        }

        bool s = Physics2D.OverlapPoint(transform.position, environmentLayerMask);
        if (!s)
        {
            DestroyImmediate(gameObject);
            return;
        }

        bool u, r, d, l;
        int c = 0;
        u = Physics2D.OverlapPoint(transform.position + Vector3.up * tileSize, environmentLayerMask);
        c += u ? 1 : 0;
        r = Physics2D.OverlapPoint(transform.position + Vector3.right * tileSize, environmentLayerMask);
        c += r ? 1 : 0;
        d = Physics2D.OverlapPoint(transform.position - Vector3.up * tileSize, environmentLayerMask);
        c += d ? 1 : 0;
        l = Physics2D.OverlapPoint(transform.position - Vector3.right * tileSize, environmentLayerMask);
        c += l ? 1 : 0;

        float rot = 0;
        Sprite chosenSprite = null;
        if (c == 4)
        {
            rot = (randomSeed % 4) * 90;
            
            chosenSprite = open0Sprites[randomSeed % open0Sprites.Length];
        }
        else if (c == 3)
        {
            rot = !u ? -90 : rot;
            rot = !r ? 180 : rot;
            rot = !d ? 90 : rot;
            rot = !l ? 0 : rot;

            chosenSprite = open1Sprites[randomSeed % open1Sprites.Length];
        }
        else if (c == 2)
        {
            rot = u && d ? 0 : rot;
            rot = l && r ? (randomSeed % 2 == 1 ? 90 : -90) : rot;
            rot = u && r ? 90 : rot;
            rot = r && d ? 0 : rot;
            rot = d && l ? -90 : rot;
            rot = l && u ? 180 : rot;

            Sprite[] tileVersion = (u && d || l && r) ? open2OSprites : open2ASprites;
            chosenSprite = tileVersion[randomSeed % tileVersion.Length];
        }
        else if (c == 1)
        {
            rot = u ? 180 : rot;
            rot = r ? 90 : rot;
            rot = d ? 0 : rot;
            rot = l ? -90 : rot;

            chosenSprite = open3Sprites[randomSeed % open3Sprites.Length];
        }
        else
        {
            rot = (randomSeed % 4) * 90;

            chosenSprite = open4Sprites[randomSeed % open4Sprites.Length];
        }

        transform.localEulerAngles = new Vector3(0, 0, rot);
        attachedRenderer.sprite = chosenSprite;

        regenerate = false;
    }
}
