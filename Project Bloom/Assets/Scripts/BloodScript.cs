using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class BloodScript : MonoBehaviour
{
    AudioSource source;

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();

        source.pitch = Random.Range(0.85f, 1.15f);
        source.Play();

        Destroy(gameObject, source.clip.length);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
