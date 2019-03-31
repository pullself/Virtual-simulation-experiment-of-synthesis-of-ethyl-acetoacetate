using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lab_wallText : MonoBehaviour
{
    string url = "http://182.254.187.40:8080/home.html";
    
    private void OnMouseDown()
    {
        Debug.Log(url);
        Application.OpenURL(url);
    }

}
