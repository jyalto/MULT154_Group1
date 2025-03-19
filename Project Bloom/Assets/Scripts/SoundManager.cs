using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public PlayerController player;

    public AudioSource[] weaponAudioSources;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();

        weaponAudioSources = GetComponents<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!player.flameActive)
        {
            if (weaponAudioSources[0].isPlaying)
            {
                weaponAudioSources[0].Stop();
            }
        }
        else
        {
            if (!weaponAudioSources[0].isPlaying)
            {
                weaponAudioSources[0].pitch = Random.Range(0.9f, 1.2f);
                weaponAudioSources[0].Play();
            }
        }
    }
}
