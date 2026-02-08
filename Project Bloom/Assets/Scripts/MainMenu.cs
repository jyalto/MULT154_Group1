using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    public Image transition;
    public float fadeDuration = 2.0f;
    public bool isActive = false;
    public AudioClip startSound;

    public Button playButton;
    public Button quitButton;

    // Start is called before the first frame update
    void Start()
    {
        EventSystem.current.SetSelectedGameObject(playButton.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the cursor is over a button using RectTransformUtility
        Vector2 mousePos = Input.mousePosition;

        if (RectTransformUtility.RectangleContainsScreenPoint(playButton.GetComponent<RectTransform>(), mousePos))
        {
            EventSystem.current.SetSelectedGameObject(playButton.gameObject);
        }
        else if (RectTransformUtility.RectangleContainsScreenPoint(quitButton.GetComponent<RectTransform>(), mousePos))
        {
            EventSystem.current.SetSelectedGameObject(quitButton.gameObject);
        }

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
