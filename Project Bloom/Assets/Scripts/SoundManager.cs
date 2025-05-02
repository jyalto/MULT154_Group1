using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public PlayerController player;

    public AudioSource[] audioSources;

    private bool death = false;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();

        audioSources = GetComponents<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!player.flameActive)
        {
            if (audioSources[0].isPlaying)
            {
                audioSources[0].Stop();
            }
        }
        else
        {
            if (!audioSources[0].isPlaying)
            {
                audioSources[0].pitch = Random.Range(0.9f, 1.2f);
                audioSources[0].Play();
            }
        }
        if (!player.lureActive)
        {
            if (audioSources[1].isPlaying)
            {
                audioSources[1].Stop();
            }
        }
        else
        {
            if (!audioSources[1].isPlaying)
            {
                audioSources[1].pitch = Random.Range(0.8f, 1.2f);
                audioSources[1].Play();
            }
        }
    }

    public void DeathCry()
    {
        if (!death)
        {
            audioSources[2].Play();
            death = true;
        }
    }
}
