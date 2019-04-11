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
    public Image midImage;
    public RectTransform doseSelectionStart;
    public RectTransform doseSelectionMid;
    public RectTransform doseSelectionEnd;
    public GameObject doseSelection;
    public Text doseSelectionSliderText;
    public Slider doseSelectionSlider;
    public Text doseSelectionTitle;
    public Text doseSelectionTypeText;
    public GameObject multiSelection;
    public Dropdown multiSelectionDropDown;
    public Text multiSelectionText;
    public GameObject btnNextScene;
    public Sprite handRelease;
    public Sprite handGrab;
    public Sprite handForbidden;
    public GameObject btnPre;
    public GameObject btnNext;
    

    [HideInInspector]
    public bool doseSelectionHasConfirmed = false;
    [HideInInspector]
    public float doseSelectionValue;
    [HideInInspector]
    public bool multiSelectionIsConfirmed = false;
    [HideInInspector]
    public string multiSelectionMyChoice;
    [HideInInspector]
    public bool isReLock = false;

    [HideInInspector]
    public float startSceneTime;

    private float lastUpdateTime;
    private float duration;
    private bool menuIsIn = true;
    private string doseSelectionDanWei;
    private AudioManager audioManager;

    private void Start()
    {
        if (SceneManager.GetActiveScene().name.Equals("Lab_1"))
        {
            btnPre.SetActive(false);
        }
        else if (SceneManager.GetActiveScene().name.Equals("Score"))
        {
            btnNext.SetActive(false);
        }
        btnNextScene.SetActive(false);
        audioManager = GetComponent<AudioManager>();
        doseSelection.transform.position = doseSelectionStart.position;
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

        if (Input.GetKeyDown(KeyCode.N))
        {
            if(!SceneManager.GetActiveScene().name.Equals("Lab_1"))
                OnBtnPreScene();
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (!SceneManager.GetActiveScene().name.Equals("Score"))
                OnBtnNextScene();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(firstPersonController.enabled)
            {
                Debug.Log("unlock");
                //UpdateInfo(new string[] { "已解除鼠标锁定", "按ESC键重新锁定" });
                audioManager.Read("已解除鼠标锁定");
                Invoke("f1", 3f);
                isReLock = false;
                firstPersonController.enabled = false;
                firstPersonController.m_MouseLook.lockCursor = false;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Debug.Log("lock");
                //UpdateInfo(new string[] { "已将鼠标锁定", "按鼠标左键继续操作" });
                audioManager.Read("已将鼠标锁定");
                Invoke("f2", 3f);
                isReLock = true;
                firstPersonController.enabled = true;
                firstPersonController.m_MouseLook.lockCursor = true;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }

    void f1()
    {
        audioManager.Read("按ESC键重新锁定");
    }

    void f2()
    {
        audioManager.Read("按鼠标左键继续操作");
    }

    public void UpdateInfo(string text, float duration = float.MaxValue)
    {
        audioManager.Read(text);
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
        else if (index == textArr.Length - 1)
            this.duration = float.MaxValue;
        audioManager.Read(textArr[index]);
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

    public void OnCorrectChoose()
    {
        audioManager.PlayAudioCorrect();
        midImage.color = Color.green;
        Invoke("ResetMidImageColor", 0.25f);
    }

    public void OnWrongChoose()
    {
        audioManager.PlayAudioWrong();
        midImage.color = Color.red;
        Invoke("ResetMidImageColor", 0.25f);
    }

    private void ResetMidImageColor()
    {
        midImage.color = Color.white;
    }

    public void OnHandGrab()
    {
        audioManager.PlayAudioCorrect();
        midImage.sprite = handGrab;
    }

    public void OnHandRelease()
    {
        audioManager.PlayAudioCorrect();
        midImage.sprite = handRelease;
    }

    public void OnHandForbidden()
    {
        midImage.sprite = handForbidden;
        audioManager.PlayAudioWrong();
        Invoke("ResetMidImageSprite", 0.8f);
    }

    private void ResetMidImageSprite()
    {
        midImage.sprite = handRelease;
    }

    public void OnDoseSelectionSliderChanged()
    {
        doseSelectionValue = doseSelectionSlider.value / 10f;
        doseSelectionSliderText.text = doseSelectionValue.ToString() + doseSelectionDanWei;
    }

    public void ShowDoseSelection(string doseSelectionDanWei, float minValue, float maxValue, float defaultValue, string type, string title = "请选择剂量")
    {
        this.doseSelectionDanWei = doseSelectionDanWei;
        doseSelectionHasConfirmed = false;
        doseSelection.transform.position = doseSelectionStart.position;
        doseSelectionSlider.minValue = (int)(minValue * 10);
        doseSelectionSlider.maxValue = (int)(maxValue * 10);
        doseSelectionSlider.value = (int)(defaultValue * 10);
        doseSelectionTypeText.text = type;
        doseSelectionTitle.text = title;

        firstPersonController.enabled = false;
        firstPersonController.m_MouseLook.lockCursor = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        iTween.MoveTo(doseSelection, doseSelectionMid.position, 1f);
    }

    public void HideDoseSelection()
    {
        firstPersonController.enabled = true;
        firstPersonController.m_MouseLook.lockCursor = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        iTween.MoveTo(doseSelection, doseSelectionEnd.position, 1f);

        doseSelectionValue = doseSelectionSlider.value / 10f;
        doseSelectionHasConfirmed = true;
    }

    public void StopUpdateTime()
    {
        CancelInvoke();
    }

    public void ShowMultiSelection(string[] options,string text)
    {
        multiSelectionIsConfirmed = false;

        firstPersonController.enabled = false;
        firstPersonController.m_MouseLook.lockCursor = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        multiSelection.transform.position = doseSelectionStart.transform.position;
        iTween.MoveTo(multiSelection, doseSelectionMid.position, 1f);

        multiSelectionText.text = text;
        multiSelectionDropDown.options.Clear();
        for (int i = 0; i < options.Length; i++) 
        {
            Dropdown.OptionData data = new Dropdown.OptionData();
            data.text = options[i];
            multiSelectionDropDown.options.Add(data);
        }
        multiSelectionDropDown.captionText.text = options[0];
        multiSelectionDropDown.value = 0;
    }

    public void OnMultiSelectionIsConfirmed()
    {
        firstPersonController.enabled = true;
        firstPersonController.m_MouseLook.lockCursor = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        iTween.MoveTo(multiSelection, doseSelectionEnd.position, 1f);
        multiSelectionIsConfirmed = true;

        int index = multiSelectionDropDown.value;
        List<Dropdown.OptionData> lt = multiSelectionDropDown.options;
        multiSelectionMyChoice = lt[index].text;
    }

    public void ShowBtnNextScene(string btnText)
    {
        firstPersonController.enabled = false;
        firstPersonController.m_MouseLook.lockCursor = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        btnNextScene.SetActive(true);
        Text tx = btnNextScene.GetComponentInChildren<Text>();
        tx.text = btnText;
        Button btn = btnNextScene.GetComponent<Button>();
        btn.onClick.AddListener(OnBtnNextScene);

    }

    public void OnBtnNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void OnBtnPreScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
}
