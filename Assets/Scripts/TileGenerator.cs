#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[ExecuteInEditMode]
public class TileGenerator : MonoBehaviour
{
    public bool regenerate = false;
    
    public GameObject tilePrefab;

    public Vector2Int generationSize = new Vector2Int(100, 100);
    public float tileSize = 1.0f;

    [MenuItem("SOL/Regnerate Tiles %g")]
    static void Regenerate()
    {
        TileGenerator gen = FindObjectOfType<TileGenerator>();
        gen.ForceRegenerate();
    }

    public void ForceRegenerate()
    {
        regenerate = true;
        Update();
    }

    void Update()
    {
        if (!regenerate)
        {
            return;
        }

        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
        
        for (int x = 0; x < generationSize.x; x++)
        {
            for (int y = 0; y < generationSize.y; y++)
            {
                Vector3 offset = new Vector3(x * tileSize, y * tileSize, 0);
                GameObject tile = Instantiate(tilePrefab, transform.position + offset, transform.rotation);
                tile.name = "Tile [" + x + ", " + y + "]"; 
                tile.transform.parent = transform;
                tile.GetComponent<TileRandomizer>().Initialize();
            }
        }

        regenerate = false;

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
    }
}
#endif