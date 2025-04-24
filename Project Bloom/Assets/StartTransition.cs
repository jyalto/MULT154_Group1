using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StartTransition : MonoBehaviour
{
    public float delay = 1.0f;
    public Image overlay;
    public Image bg;
    public Image text;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<AudioSource>().Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (delay > 0.0f)
        {
            delay -= Time.deltaTime;
        }
        else if (overlay.color.a > 0)
        {
            overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, overlay.color.a - Time.deltaTime / 2f);
        }
        else if (bg.color.a > 0)
        {
            bg.color = new Color(bg.color.r, bg.color.g, bg.color.b, bg.color.a - Time.deltaTime / 2f);
            text.color = new Color(text.color.r, text.color.g - Time.deltaTime, text.color.b - Time.deltaTime, text.color.a - Time.deltaTime / 2f);

            gameObject.GetComponent<AudioSource>().volume -= Time.deltaTime /  2f;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
