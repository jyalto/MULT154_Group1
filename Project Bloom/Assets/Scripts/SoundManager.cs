using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public PlayerController player;

    private AudioSource[] audioSources;

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
                audioSources[0].Play();
            }
        }
    }
}
