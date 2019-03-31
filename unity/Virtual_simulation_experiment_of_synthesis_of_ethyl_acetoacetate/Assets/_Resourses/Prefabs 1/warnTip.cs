using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class warnTip : MonoBehaviour
{
    public float destroyDelay = 2f;
    public AudioClip warnTipAudio;
    private void Start()
    {
        Destroy(gameObject, destroyDelay);
    }

    private void OnDestroy()
    {
        if(warnTipAudio!=null)
        { 
            AudioSource audioSourse = Camera.main.GetComponent<AudioSource>();
            audioSourse.clip = warnTipAudio;
            audioSourse.Play();
        }
        Destroy(gameObject);
    }
}
