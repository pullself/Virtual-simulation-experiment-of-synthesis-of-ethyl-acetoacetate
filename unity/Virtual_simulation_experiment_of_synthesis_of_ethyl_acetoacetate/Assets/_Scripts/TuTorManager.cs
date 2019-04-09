using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TuTorManager : MonoBehaviour
{
    public GameObject objClick;
    public float clickRate = 1f;
    public float clickDownDuration = 0.2f;

    public GameObject objDrag;
    public Material matRelease;
    public Material matGrab;

    private void Awake()
    {
        if(objClick)
            objClick.SetActive(false);
        if (objDrag)
            objDrag.SetActive(false);
    }

    void Start()
    {
        
    }

    public void ShowTutorClick()
    {
        SetMatClickRelease();
        objClick.SetActive(true);
        InvokeRepeating("DoTutorClick", clickRate, clickRate);
    }

    public void EndTutorClick()
    {
        objClick.SetActive(false);
        CancelInvoke("DoTutorClick");
    }

    private void DoTutorClick()
    {
        Invoke("SetMatClickGrab", 0f);
        Invoke("SetMatClickRelease", clickDownDuration);
    }

    private void SetMatClickRelease()
    {
        objClick.GetComponent<Renderer>().material = matRelease;
    }
    private void SetMatClickGrab()
    {
        objClick.GetComponent<Renderer>().material = matGrab;
    }
    //--------------------------------------
    public void ShowTutorDrag()
    {
        objDrag.SetActive(true);
        Animation anim = objDrag.GetComponent<Animation>();
        anim.Play();
        anim.playAutomatically = true;
        InvokeRepeating("DoTutorDrag", 0f, anim.clip.length);
    }

    public void EndTutorDrag()
    {
        objDrag.SetActive(false);
        CancelInvoke("ShowTutorDrag");
    }

    private void DoTutorDrag()
    {
        Invoke("SetMatDragGrab", 0.2f);
        Invoke("SetMatDragRelease", 1.8f);
    }

    private void SetMatDragRelease()
    {
        objDrag.GetComponent<Renderer>().material = matRelease;
    }
    private void SetMatDragGrab()
    {
        objDrag.GetComponent<Renderer>().material = matGrab;
    }
}
