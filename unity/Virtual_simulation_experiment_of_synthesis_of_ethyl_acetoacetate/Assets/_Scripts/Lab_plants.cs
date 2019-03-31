using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lab_plants : MonoBehaviour
{
    private Quaternion defaultRotation;
    private Vector3 defaultPosition;

    void Start()
    {
        defaultRotation = transform.rotation;
        defaultPosition = transform.position;
    }

    
    private void OnMouseUpAsButton()
    {
        transform.rotation = defaultRotation;
        transform.position = defaultPosition + Vector3.up;
    }
}
