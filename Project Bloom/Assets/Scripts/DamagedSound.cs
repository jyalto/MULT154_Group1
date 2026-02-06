using System.Collections;
using UnityEngine;

public class DamagedSound : MonoBehaviour
{
    [SerializeField]
    private AudioSource[] audioSources;

    [SerializeField]
    private float pitchMinVal = 0f;
    [SerializeField]
    private float pitchMaxVal = 2f;

    public void PlayRandomSound()
    {
        if (audioSources.Length == 0) return;
        AudioSource randomSource = audioSources[Random.Range(0, audioSources.Length)];

        randomSource.pitch = Random.Range(pitchMinVal, pitchMaxVal);
        randomSource.Play();
    }
}
