using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class AudioManager:MonoBehaviour
{
    public AudioClip audioCorrect;
    public AudioClip audioWrong;
    public AudioClip audioComplete;

    public AudioClip[] audioClips;
    private AudioSource audioSourse = null;

    public void Start()
    {
        GetAudioSourceComponent();
    }

    public void Read(string audioClipName)
    {
        for (int i = 0; i < audioClips.Length; i++) 
        {
            if (audioClipName.Equals(audioClips[i].name))
            {
                PlayAudio(audioClips[i]);
                return;
            }
        }
        Debug.Log("no match audio : " + audioClipName);
    }

    public void Read(int audioIndex)
    {
        if (audioIndex < audioClips.Length)
        {
            PlayAudio(audioClips[audioIndex]);
        }
        else
        {
            Debug.Log("no match audio : " + "index=" + audioIndex);
        }
    }

    void PlayAudio(AudioClip audioClip)
    {
        GetAudioSourceComponent();
        audioSourse.clip = audioClip;
        audioSourse.Play();
    }

    void GetAudioSourceComponent()
    {
        if (audioSourse != null)
            return;
        audioSourse = Camera.main.GetComponent<AudioSource>();
        if (audioSourse == null)
        {
            Camera.main.gameObject.AddComponent<AudioSource>();
            audioSourse = Camera.main.GetComponent<AudioSource>();
        }
    }

    public void PlayAudioCorrect()
    {
        PlayAudio(audioCorrect);
    }

    public void PlayAudioWrong()
    {
        PlayAudio(audioWrong);
    }

    public void PlayAudioComplete()
    {
        PlayAudio(audioComplete);
    }
}


#if false

//using SpeechLib;

public class StringReader
{
    

    //Thread thread;
    //string readText;
    //SpVoice v;

    public StringReader()   //构造
    {
        //v = new SpVoice();
        //v.Voice = v.GetVoices(string.Empty, string.Empty).Item(0);
    }

    public void Read(string str)
    {
        //readText = str;
        //thread = new Thread(new ThreadStart(DoRead));
        //thread.Start();
    }

    private void DoRead()
    {
        //try
        //{
        //    v.Speak(readText);
        //}
        //catch (Exception e)
        //{
        //    Debug.Log(e.ToString());
        //}
    }
}

#endif