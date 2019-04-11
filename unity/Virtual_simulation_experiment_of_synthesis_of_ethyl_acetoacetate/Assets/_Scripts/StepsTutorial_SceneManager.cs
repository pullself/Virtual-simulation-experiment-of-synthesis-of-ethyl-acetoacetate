using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StepsTutorial_SceneManager : MonoBehaviour
{
    public RectTransform panelLeftPosRT;
    public RectTransform panelMidPosRT;
    public RectTransform panelRightPosRT;
    public GameObject btnPrevious;
    public GameObject btnNext;
    public GameObject btnBack;
    public GameObject[] panels;
    public float animationDuration = 1f;

    private Vector3 leftPos;
    private Vector3 midPos;
    private Vector3 rightPos;
    private int nowPanelIndex;

    void Start()
    {
        leftPos = panelLeftPosRT.position;
        midPos = panelMidPosRT.position;
        rightPos = panelRightPosRT.position;
        nowPanelIndex = panels.Length > 0 ? 0 : -1;
        //for (int i = 0; i < panels.Length; i++) 
        //{
        //    panels[i].SetActive(true);
        //    if (i == 0)
        //    {
        //        panels[i].GetComponent<RectTransform>().position = midPos;
        //        btnPrevious.SetActive(false);
        //    }
        //    else
        //    {
        //        panels[i].GetComponent<RectTransform>().position = rightPos;
        //    }
        //}
        //btnBack.SetActive(false);
        btnBack.SetActive(true);//
    }

    public void SwitchToNextPanel()
    {
        if (nowPanelIndex < 0 || nowPanelIndex >= panels.Length-1)
            return;
        nowPanelIndex++;
        if (nowPanelIndex > 0)
            btnPrevious.SetActive(true);
        if (nowPanelIndex == panels.Length - 1)
            btnNext.SetActive(false);
        btnBack.SetActive(nowPanelIndex == panels.Length - 1);
        iTween.MoveTo(panels[nowPanelIndex-1], leftPos, animationDuration);
        iTween.MoveTo(panels[nowPanelIndex], midPos, animationDuration);
    }
    public void SwitchToPreviousPanel()
    {
        if (nowPanelIndex <= 0 || nowPanelIndex > panels.Length - 1)
            return;
        nowPanelIndex--;
        if (nowPanelIndex == 0) 
            btnPrevious.SetActive(false);
        if (nowPanelIndex < panels.Length - 1)
            btnNext.SetActive(true);
        btnBack.SetActive(nowPanelIndex == panels.Length - 1);
        iTween.MoveTo(panels[nowPanelIndex + 1], rightPos, animationDuration);
        iTween.MoveTo(panels[nowPanelIndex], midPos, animationDuration);
    }

    public void Back()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
