using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Welcome_SceneManager : MonoBehaviour
{
    private AudioManager audioManager;
    private string nextSceneName = "MainMenu";
    private bool hasJumped = false;

    void Start()
    {
        audioManager = GetComponent<AudioManager>();
        audioManager.Read("欢迎使用乙酰乙酸乙酯制备模拟实验系统");
        Invoke("JumpToNextScene", 5f);
        ScoreManager.ClearAll();
    }

    private void FixedUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            JumpToNextScene();
        }
    }

    void JumpToNextScene()
    {
        if(!hasJumped)
        { 
            hasJumped = true;
            SceneManager.LoadScene(nextSceneName);
        }
    }

    
}
