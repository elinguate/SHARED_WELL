using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreen : MonoBehaviour
{
    public float timer = 1.0f;

    void Awake()
    {
        PlayerPrefs.DeleteAll();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        timer -= Time.deltaTime;
        if (timer < 0 && Input.anyKeyDown)
        {
            SceneManager.LoadScene("-1_1");
        }
    }
}
