using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureChest : MonoBehaviour
{
    private Animator animChest;
    private AudioSource[] audioSources;

    public GameObject chestItem;
    public bool opened = false;

    void Start()
    {
        animChest = GetComponent<Animator>();
        audioSources = GetComponents<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenChest()
    {
        animChest.SetBool("Open", true);
        audioSources[0].Play();
        StartCoroutine(ItemVisible(1.25f));
        opened = true;
    }

    public void LockedChest()
    {
        if (!audioSources[1].isPlaying)
        {
            audioSources[1].Play();
        }
    }

    private IEnumerator ItemVisible(float delay)
    {
        yield return new WaitForSeconds(delay);
        chestItem.SetActive(true);
    }
}
