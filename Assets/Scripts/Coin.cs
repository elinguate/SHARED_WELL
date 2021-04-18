using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public SpriteRenderer display;
    public Sprite[] coinSprites;
    public float animationSpeed = 1;
    public float animationTimer = 0;

    void Update()
    {
        animationTimer += Time.deltaTime * animationSpeed;
        display.sprite = coinSprites[Mathf.FloorToInt(animationTimer) % coinSprites.Length];
    }
}
