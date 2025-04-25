using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bat : MonoBehaviour
{
    private AudioSource audioSource;
    private PlayerController playerController;
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        player = GameObject.Find("Player");
        playerController = player.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        AnimatorStateInfo stateInfo = playerController.playerAnim.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("Attack") && other.CompareTag("Enemy"))
        {
            audioSource.Play();
        }
    }
}
