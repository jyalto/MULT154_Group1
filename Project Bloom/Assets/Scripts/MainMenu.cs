using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Image transition;
    public float fadeDuration = 2.0f;
    public bool isActive = false;
    public AudioClip startSound;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Color currentColor = transition.color;
        if (isActive && currentColor.a < 1)
        {
            transition.enabled = true;
            transition.color = new Color(currentColor.r, currentColor.g, currentColor.b, currentColor.a + Time.deltaTime / fadeDuration);
        }
        if (!isActive && currentColor.a > 0)
        {
            transition.color = new Color(currentColor.r, currentColor.g, currentColor.b, currentColor.a - Time.deltaTime / fadeDuration);
        }

        if (!isActive && currentColor.a < 0.001)
        {
            transition.enabled = false;
        }
    }

    public void OnPlayPressed()
    {
        isActive = true;
        GetComponent<AudioSource>().Stop();
        GetComponent<AudioSource>().loop = false;
        GetComponent<AudioSource>().PlayOneShot(startSound);
        Invoke("StartGame", 5);
    }

    void StartGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void OnQuitPressed()
    {
        Application.Quit();
    }
}
