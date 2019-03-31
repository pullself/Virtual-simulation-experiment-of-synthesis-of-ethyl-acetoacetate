using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraughtCupboardDoor : MonoBehaviour
{
    public float animationLength = 2.5f;
    public Transform doorClosed;
    public AudioClip audioOpenDoor;

    private bool doorIsOpened;
    private Vector3 doorOpenedRotation;
    private Vector3 doorClosedRotation;
    private AudioSource audioSourse;

    void Start()
    {
        doorOpenedRotation = transform.rotation.eulerAngles;
        doorClosedRotation = doorClosed.transform.rotation.eulerAngles;

        transform.rotation = doorClosed.rotation;
        doorIsOpened = false;
        //OpenDoor();
    }

    public void OnMyMouseDown()
    {
        if (doorIsOpened == true)
        {
            CloseDoor();
        }
        else
        {
            OpenDoor();
        }
    }

    public void OpenDoor()
    {
        if (doorIsOpened == false)
        {
            doorIsOpened = true;
            iTween.RotateTo(gameObject, doorOpenedRotation, animationLength);
            AudioSource audiosourse = GetAudioSourceComponent();
            audiosourse.clip = audioOpenDoor;
            audiosourse.Play();
        }
    }

    public void CloseDoor()
    {
        if (doorIsOpened == true)
        {
            doorIsOpened = false;
            iTween.RotateTo(gameObject, doorClosedRotation, animationLength);
            AudioSource audiosourse = GetAudioSourceComponent();
            audiosourse.clip = audioOpenDoor;
            audiosourse.Play();
        }
    }

    AudioSource GetAudioSourceComponent()
    {
        if (audioSourse != null)
            return audioSourse;
        audioSourse = Camera.main.GetComponent<AudioSource>();
        if (audioSourse == null)
        {
            Camera.main.gameObject.AddComponent<AudioSource>();
            audioSourse = Camera.main.GetComponent<AudioSource>();
        }
        return audioSourse;
    }
}
