using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tipController : MonoBehaviour
{
    public float destroyDelay = 2f;
    public AudioClip tipAudio;
    private void Start()
    {
        if (tipAudio != null)
        {
            AudioSource audioSourse = Camera.main.GetComponent<AudioSource>();
            audioSourse.clip = tipAudio;
            audioSourse.Play();
        }
        Destroy(gameObject, destroyDelay);
    }
    
}
