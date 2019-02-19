using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Welcome_SceneManager : MonoBehaviour
{
    private string nextSceneName = "MainMenu";
    private bool hasJumped = false;

    void Start()
    {
        Invoke("JumpToNextScene", 5f);
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
