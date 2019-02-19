using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Lab_UIManager : MonoBehaviour
{
    public Text infoText;
    public Text timeText;
    public UnityStandardAssets.Characters.FirstPerson.FirstPersonController firstPersonController;
    public CharacterController characterController;
    public RectTransform menuInRT;
    public RectTransform menuOutRT;
    public GameObject menu;

    private float lastUpdateTime;
    private float duration;
    private bool menuIsIn = true;
    private float startSceneTime;

    private void Start()
    {
        startSceneTime = Time.time;
        InvokeRepeating("UpdateUIMenuTime", 0f, 1f);
    }

    private void UpdateUIMenuTime()
    {
        timeText.text = TimeFormatterUtil(Time.time - startSceneTime);
    }

    private void MenuMoveOut()
    {
        menuIsIn = false;
        iTween.MoveTo(menu, menuOutRT.position, 1f);
    }

    private void MenuMoveIn()
    {
        menuIsIn = true;
        iTween.MoveTo(menu, menuInRT.position, 1f);
    }

    private void Update()
    {
        if (Time.time - lastUpdateTime >= this.duration)
        {
            ClearInfo();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (menuIsIn)       //如果已经在里面，弹出去
            {
                MenuMoveOut();
            }
            else                //如果在外面，弹进来
            {
                MenuMoveIn();
            }
        }

        if (Input.GetKeyDown(KeyCode.R)) 
        {
            OnBtnRestart();
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            OnBtnBack();
        }
    }

    public void UpdateInfo(string text, float duration = 3f)
    {
        lastUpdateTime = Time.time;
        infoText.text = text;
        this.duration = duration;
        MenuMoveIn();
    }

    private int index = 0;
    private string[] textArr;
    public void UpdateInfo(string[] text, float duration = 3f)
    {
        MenuMoveIn();
        index = 0;
        textArr = text;
        this.duration = duration;
        _UpdateInfoArray();
    }

    private void _UpdateInfoArray()
    {
        if (index > textArr.Length - 1)
            return;
        infoText.text = textArr[index];
        index++;
        lastUpdateTime = Time.time;
        Invoke("_UpdateInfoArray", duration);
    }

    private void ClearInfo()
    {
        infoText.text = "";
    }

    public void OnBtnBack()
    {
        firstPersonController.m_MouseLook.lockCursor = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene("MainMenu");
    }

    public void OnBtnRestart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private string TimeFormatterUtil(double time)
    {
        int hour = (int)time / 3600;
        int tmp = (int)(time - hour * 3600);
        int minute = tmp / 60;
        int second = (int)(tmp - minute * 60);
        return string.Format("{0:D2}:{1:D2}:{2:D2}", hour, minute, second);
    }
}
