using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class _testForScoreManager : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnGUI()
    {
        if (GUILayout.Button("init"))
            ScoreManager.InitScore("key",100);
        if (GUILayout.Button("add"))
            ScoreManager.AddScore(-5);
        if (GUILayout.Button("get"))
            Debug.Log(ScoreManager.GetScore());
        if (GUILayout.Button("save"))
            ScoreManager.SaveScore("key");
        if (GUILayout.Button("read"))
            Debug.Log(ScoreManager.ReadScore("key"));

        //if (GUI.Button(new Rect(0, 0, 200, 80), "init"))
        //    ScoreManager.InitScore("key");
        //if (GUI.Button(new Rect(0, 100, 200, 80), "add"))
        //    ScoreManager.AddScore(-5);
        //if (GUI.Button(new Rect(0, 200, 200, 80), "get"))
        //    Debug.Log(ScoreManager.GetScore());
        //if (GUI.Button(new Rect(0, 300, 200, 80), "save"))
        //    ScoreManager.SaveScore("key");
        //if (GUI.Button(new Rect(0, 400, 200, 80), "read"))
        //    Debug.Log(ScoreManager.ReadScore("key"));
    }
}
