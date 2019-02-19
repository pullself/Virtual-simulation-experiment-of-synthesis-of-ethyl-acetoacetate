using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu_SceneManager : MonoBehaviour
{


    public void StartExperiment()
    {
        SceneManager.LoadScene("Lab");

    }

    public void ShowSteps()
    {
        SceneManager.LoadScene("StepsTutorial");
    }

    public void ShowVedio()
    {
        SceneManager.LoadScene("VideoTutorial");
    }

    public void QuitExperiment()
    {
        Debug.Log("退出实验");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
    }

}
