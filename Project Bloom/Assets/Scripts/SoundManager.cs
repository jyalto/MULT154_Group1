using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SoundManager : MonoBehaviour
{
    public PlayerController player;

    public AudioSource[] audioSources;

    public bool wave4SoundPlayed = false;

    private bool death = false;
    public bool disturbedPlayed = false;
    private bool disturbedDelay = false;
    private bool helicopterSpawned = false;

    private Coroutine myCoroutine = null;
    private GameManager gameManager;

    public GameObject helicopter;
    public GameObject alertRadio;
    public AudioClip wave4;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();

        audioSources = GetComponents<AudioSource>();

        GameObject gameManagerObject = GameObject.Find("Game Manager");

        if (gameManagerObject != null)
        {
            gameManager = gameManagerObject.GetComponent<GameManager>();
        }
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

        if (gameManager.wave == 4 && !disturbedDelay && myCoroutine == null)
        {
            myCoroutine = StartCoroutine(DisturbedDelayTime());
        }

        /*if (disturbedPlayed)
        {
            audioSources[7].Stop();
        }*/
    }

    public void DeathCry()
    {
        if (!death)
        {
            if (audioSources[3].isPlaying)
            {
                audioSources[3].Stop();
            }
            if (audioSources[7].isPlaying)
            {
                audioSources[7].Stop();
            }
            audioSources[5].Stop();
            audioSources[2].Play();
            death = true;
        }
    }

    public void Wave4Start()
    {
        if (!audioSources[4].isPlaying && !wave4SoundPlayed)
        {
            audioSources[4].Play();
            wave4SoundPlayed = true;
        }
    }

    public void RayGunTroll()
    {
        if (!audioSources[6].isPlaying)
        {
            audioSources[6].Play();
        }
    }

    public void Disturbed()
    {
        if (disturbedDelay)
        {
            if (!audioSources[7].isPlaying && !disturbedPlayed)
            {
                audioSources[3].Stop();
                audioSources[7].Play();
                disturbedPlayed = true;
            }
            else if (!audioSources[7].isPlaying && disturbedPlayed)
            {
                if (!audioSources[3].isPlaying && !helicopterSpawned)
                {
                    audioSources[3].Play();
                    alertRadio.GetComponent<AudioSource>().PlayOneShot(wave4, 4);
                    myCoroutine = StartCoroutine(HelicopterDelayTime());
                }
            }
        }
    }

    public void BearTrap()
    {
        audioSources[8].Play();
    }

    public void BearTrapBreak()
    {
        audioSources[9].Play();
    }

    private IEnumerator DisturbedDelayTime()
    {
        yield return new WaitForSeconds(3f);
        disturbedDelay = true;
        myCoroutine = null;
    }

    private IEnumerator HelicopterDelayTime()
    {
        yield return new WaitForSeconds(35f);
        helicopter.SetActive(true);
        helicopterSpawned = true;
        myCoroutine = null;
    }
}
