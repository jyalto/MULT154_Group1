using System.Collections;
using UnityEngine;

public class EnemySound : MonoBehaviour
{
    [SerializeField] 
    private AudioSource[] audioSources;

    [SerializeField]
    private AudioSource[] audioSourcesDeath;

    private Coroutine myCoroutine = null;

    void Start()
    {
        PlayRandomSound();
    }

    void Update()
    {
        if (myCoroutine == null)
        {
            float time = Random.Range(7f, 25f);
            myCoroutine = StartCoroutine(PlaySound(time));
        }
    }

    private void PlayRandomSound()
    {
        if (audioSources.Length == 0) return;

        AudioSource randomSource = audioSources[Random.Range(0, audioSources.Length)];

        // play in 3D at the parent transform position
        AudioClip clip = randomSource.clip;
        float pitch = Random.Range(0.85f, 1.15f);

        GameObject temp = new GameObject("TempAudio"); // temporary audio object
        temp.transform.position = transform.position;
        AudioSource tempSource = temp.AddComponent<AudioSource>();
        tempSource.clip = clip;
        tempSource.pitch = pitch;
        tempSource.spatialBlend = 1f;
        tempSource.minDistance = 1f;
        tempSource.maxDistance = 250f;
        tempSource.Play();

        Destroy(temp, clip.length / pitch);
    }

    public void PlayDeathSound()
    {
        if (audioSourcesDeath.Length == 0) return;

        AudioSource randomSource = audioSourcesDeath[Random.Range(0, audioSourcesDeath.Length)];

        // play in 3D at the parent transform position
        AudioClip clip = randomSource.clip;
        float pitch = Random.Range(0.85f, 1.15f);

        GameObject temp = new GameObject("TempAudio"); // temporary audio object
        temp.transform.position = transform.position;
        AudioSource tempSource = temp.AddComponent<AudioSource>();
        tempSource.clip = clip;
        tempSource.pitch = pitch;
        tempSource.spatialBlend = 1f;
        tempSource.minDistance = 1f;
        tempSource.maxDistance = 250f;
        tempSource.Play();

        Destroy(temp, clip.length / pitch);
    }

    private IEnumerator PlaySound(float time)
    {
        yield return new WaitForSeconds(time);
        PlayRandomSound();
        myCoroutine = null;
    }
}