using UnityEngine;
using System.Collections;

public class ApplicationManager : MonoBehaviour {

    /*
    public void StartExperiment()
    {
        Debug.Log("开始试验");

    }


    public void ShowSteps()
    {
        Debug.Log("实验流程");

    }

    public void ShowVedio()
    {
        Debug.Log("视频演示");

    }*/

    public void QuitExperiment()
    {
        //Debug.Log("退出实验");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
    }

}
