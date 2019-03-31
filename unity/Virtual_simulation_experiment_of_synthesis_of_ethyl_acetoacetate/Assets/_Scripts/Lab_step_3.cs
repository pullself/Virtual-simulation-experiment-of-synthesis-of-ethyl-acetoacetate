using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Lab_step_3 : MonoBehaviour
{
    public GameObject rope_water_to_eq;
    public GameObject rope_eq_to_water;
    public GameObject waterTube;
    public Material matOrange;
    public DraughtCupboardDoor doorLeft;
    public DraughtCupboardDoor doorRight;
    public GameObject magnifierCamera;
    public ExperimentOperation[] experimentOperation;
    private int opNum;
    
    private Lab_UIManager UIManager;
    private AnimationUtils animationUtils;
    private int state;

    private float minFov = 5f;
    private float maxFov = 100f;
    private float defaultFov = 60f;
    private float sensitivity = 30f;
    private bool isPlayingAnim;
    private bool hasSelected;
    private bool isLocked = true;
    private AudioManager audioManager;
    private bool doseSelectionIsOpened = false;
    private bool multiSelectionIsOpened = false;
    private bool btnNextSceneOpened = false;
    private string key = "Lab_3";

    void Start()
    {
        ScoreManager.InitScore(key);

        waterTube.SetActive(false);
        rope_eq_to_water.SetActive(false);
        rope_water_to_eq.SetActive(false);

        Camera.main.fieldOfView = defaultFov;
        Camera camera = magnifierCamera.GetComponent<Camera>();
        camera.enabled = false;
        
        audioManager = GetComponent<AudioManager>();
        hasSelected = false;
        isPlayingAnim = false;
        state = 0;
        opNum = experimentOperation.Length;
        UIManager = GetComponent<Lab_UIManager>();
        animationUtils = GetComponent<AnimationUtils>();
        animationUtils.HideRoadPoints(experimentOperation);
        UIManager.UpdateInfo(new string[] { "实验第三部分开始" ,"请取下圆底烧瓶上的橡胶塞"});
    }

    private void Update()
    {
        bool isMouseDown = Input.GetMouseButtonDown(0);
        bool isMouseUp = Input.GetMouseButtonUp(0);
        if ((isMouseDown || isMouseUp) && !doseSelectionIsOpened && !multiSelectionIsOpened && !isPlayingAnim && !btnNextSceneOpened
            && UIManager.firstPersonController.enabled)
        {
            if (state == -1)
            {
                SceneManager.LoadScene("Lab_4");
                //Debug.Log("to scene step_4");
                return;
            }
            if (UIManager.isReLock)
            {
                UIManager.isReLock = false;
                return;
            }
            RaycastHit hitInfo;
            Vector2 screenMidPoint = new Vector2(Screen.width / 2, Screen.height / 2);
            LayerMask mask = 1 << (LayerMask.NameToLayer("Equipment"));//layer为枚举
            if (Physics.Raycast(Camera.main.ScreenPointToRay(screenMidPoint), out hitInfo, 64, mask.value))
            {
                string name = hitInfo.collider.name;
                if (name.Equals("doorLeft") || name.Equals("doorRight"))
                {
                    if (isMouseUp)
                    {
                        DraughtCupboardDoor door = hitInfo.collider.gameObject.GetComponent<DraughtCupboardDoor>();
                        door.OnMyMouseDown();
                    }
                }
                else
                {
                    Debug.Log(name);
                    RaycastResultJudge(hitInfo, isMouseDown, isMouseUp);
                }
            }
        }


        if (Input.GetMouseButtonDown(1))
        {
            Camera.main.fieldOfView = defaultFov;
            Camera camera = magnifierCamera.GetComponent<Camera>();
            camera.enabled = !camera.enabled;
        }

        float fov = Camera.main.fieldOfView;
        fov += Input.GetAxis("Mouse ScrollWheel") * -sensitivity;
        fov = Mathf.Clamp(fov, minFov, maxFov);
        Camera.main.fieldOfView = fov;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isLocked)
            {
                isLocked = false;
                UIManager.firstPersonController.m_MouseLook.lockCursor = false;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                isLocked = true;
                UIManager.firstPersonController.m_MouseLook.lockCursor = true;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        if (UIManager.doseSelectionHasConfirmed == true)
        {
            doseSelectionIsOpened = false;
            UIManager.doseSelectionHasConfirmed = false;
            float allowableDeviation = 0.1f;        //允许最大偏差10%
            float targetValue = float.MaxValue;
            switch (state)
            {
                case 4:
                    targetValue = 27.5f;
                    break;
                default:
                    Debug.Log("error state");
                    break;
            }
            if (Mathf.Abs(targetValue - UIManager.doseSelectionValue) / targetValue <= allowableDeviation)  //误差在10%以内
            {
                PlayAnimation();
                audioManager.PlayAudioCorrect();
                if(state==4)
                { 
                    Invoke("state_4_delay_func", 3.5f);
                    PlayAnimation();
                }
            }
            else
            {
                audioManager.PlayAudioWrong();
                Invoke("OnWrongTip", 1f);
                ScoreManager.AddScore(-20);
            }
        }

        if (UIManager.multiSelectionIsConfirmed)
        {
            multiSelectionIsOpened = false;
            UIManager.multiSelectionIsConfirmed = false;
            string myChoice = UIManager.multiSelectionMyChoice;
            bool isCorrect = false;
            switch (state)
            {
                case 8:
                    if (myChoice.Equals("下进上出"))
                    {
                        isCorrect = true;
                    }
                    break;
            }
            if (isCorrect)
            {
                audioManager.PlayAudioCorrect();
                if (state == 8)
                {
                    rope_water_to_eq.SetActive(true);
                    rope_eq_to_water.SetActive(true);
                    waterTube.SetActive(true);
                    iTween.ScaleFrom(waterTube, iTween.Hash(
                        "time", 4f,
                        "easetype", iTween.EaseType.linear,
                        "y", 0));
                    Invoke("OnAnimationFinished", 5f);
                }
                else
                {
                    PlayAnimation();
                }
            }
            else
            {
                Invoke("OnWrongTip", 1f);
                audioManager.PlayAudioWrong();
                ScoreManager.AddScore(-20);
            }
        }
    }

    void OnWrongTip()
    {
        if (state == 4)
        {
            UIManager.UpdateInfo("所选乙酸乙酯的量不符合要求");
            Invoke("state_4_delay_fun", 4f);
        }
        else if (state == 8)
        {
            UIManager.UpdateInfo(new string[] { "选择的通冷凝水的方法错误", "请重新选择" });
        }
    }

    void state_4_delay_fun()
    {
        UIManager.UpdateInfo("请将27.5ml乙酸乙酯加入圆底烧瓶");
    }

    GameObject StopperYuanDiShaoPing;
    GameObject waterYuanDiShaoPing;
    GameObject StopperzongPing;
    GameObject zongPing;
    GameObject yuanDiShaoPing;

    void RaycastResultJudge(RaycastHit hitInfo, bool isMouseDown, bool isMouseUp)
    {
        if (hasSelected == false && isMouseDown)
        {
            switch (state)
            {
                case 0:
                    if (hitInfo.collider.name.Equals("StopperYuanDiShaoPing"))          
                    {
                        StopperYuanDiShaoPing = hitInfo.collider.gameObject;
                        hasSelected = true;
                    }
                    break;    
                case 1:
                    if (hitInfo.collider.name.Equals("yuanDiShaoPing"))
                    {
                        StopperYuanDiShaoPing.transform.SetParent(null);
                        yuanDiShaoPing = hitInfo.collider.gameObject;
                        Transform[] trs = yuanDiShaoPing.GetComponentsInChildren<Transform>();
                        for (int i = 0; i < trs.Length; i++) 
                        {
                            if (trs[i].name.Equals("waterC8H10"))
                            {
                                waterYuanDiShaoPing = trs[i].gameObject;
                                break;
                            }
                        }
                        hasSelected = true;
                    }
                    break;
                case 2:
                    if (hitInfo.collider.name.Equals("zongPing"))
                    {
                        hasSelected = true;
                    }
                    break;
                case 3:
                    if (hitInfo.collider.name.Equals("StopperzongPing"))
                    {
                        StopperzongPing = hitInfo.collider.gameObject;
                        hasSelected = true;
                    }
                    break;
                case 4:
                    if (hitInfo.collider.name.Equals("zongPing"))
                    {
                        zongPing = hitInfo.collider.gameObject;
                        StopperzongPing.transform.SetParent(null);
                        hasSelected = true;
                    }
                    break;
                case 5:
                    if (hitInfo.collider.name.Equals("StopperzongPing"))
                    {
                        StopperzongPing.transform.SetParent(zongPing.transform);
                        hasSelected = true;
                    }
                    break;
                case 6:
                    if (hitInfo.collider.name.Equals("qiuXingLengNingGuan"))
                    {
                        hasSelected = true;
                    }
                    break;
                case 7:
                    if (hitInfo.collider.name.Equals("ganZaoGuan"))
                    {
                        hasSelected = true;
                    }
                    break;
                case 8:
                    if (hitInfo.collider.name.Equals("qiuXingLengNingGuan") || hitInfo.collider.name.Equals("waterTap"))
                    {
                        hasSelected = true;
                    }
                    break;
            }
            if (hasSelected)
            {
                //UIManager.OnCorrectChoose();
                UIManager.OnHandGrab();
            }
            else
            {
                //UIManager.OnWrongChoose();
                UIManager.OnHandForbidden();
                ScoreManager.AddScore(-5);
            }
        }
        else if (hasSelected == true && isMouseUp)
        {
            switch (state)
            {
                case 0:
                    if (hitInfo.collider.name.Equals("tableMat"))
                    {
                        PlayAnimation();
                        hasSelected = false;
                    }
                    break;
                case 1:
                    if (hitInfo.collider.name.Equals("guanZi"))
                    {
                        Invoke("state_1_delay_func",5.5f);
                        PlayAnimation();
                        hasSelected = false;
                    }
                    break;
                case 2:
                    if (hitInfo.collider.name.Equals("tableMat"))
                    {
                        PlayAnimation();
                        hasSelected = false;
                    }
                    break;
                case 3:
                    if (hitInfo.collider.name.Equals("tableMat"))
                    {
                        PlayAnimation();
                        hasSelected = false;
                    }
                    break;
                case 4:
                    if (hitInfo.collider.name.Equals("yuanDiShaoPing"))
                    {
                        doseSelectionIsOpened = true;
                        UIManager.ShowDoseSelection("ml", 0.1f, 50f, Random.Range(0.1f, 50f), "种类:乙酸乙酯", "请选择剂量");
                        hasSelected = false;
                    }
                    break;
                case 5:
                    if (hitInfo.collider.name.Equals("zongPing"))
                    {
                        PlayAnimation();
                        hasSelected = false;
                    }
                    break;
                case 6:
                    if (hitInfo.collider.name.Equals("yuanDiShaoPing"))
                    {
                        PlayAnimation();
                        hasSelected = false;
                    }
                    break;
                case 7:
                    if (hitInfo.collider.name.Equals("qiuXingLengNingGuan"))
                    {
                        PlayAnimation();
                        hasSelected = false;
                    }
                    break;
                case 8:
                    if (hitInfo.collider.name.Equals("qiuXingLengNingGuan") || hitInfo.collider.name.Equals("waterTap"))
                    {
                        //PlayAnimation();
                        multiSelectionIsOpened = true;
                        UIManager.ShowMultiSelection(new string[] { "请选择",
                                                                    "下进上出",
                                                                    "上进下出",
                                                                    "以上均可"}, "通冷凝水的方式");
                        hasSelected = false;
                    }
                    break;
            }
            if (hasSelected == false)
            {
                //UIManager.OnCorrectChoose();
                UIManager.OnHandRelease();
            }
            else
            {
                hasSelected = false;
                //UIManager.OnWrongChoose();
                UIManager.OnHandForbidden();
                ScoreManager.AddScore(-5);
            }
        }
    }

    void state_1_delay_func()
    {
        waterYuanDiShaoPing.SetActive(false);
    }

    void state_4_delay_func()
    {
        waterYuanDiShaoPing.SetActive(true);
    }

    void OnAnimationStart()
    {

    }

    void PlayAnimation()
    {
        if (!isPlayingAnim)
        {
            isPlayingAnim = true;
            OnAnimationStart();                                             //播放动画前的操作
            animationUtils.PlayAnimations(experimentOperation, state);      //播放动画

            float timeSum = 0f;
            for (int i = 0; i < experimentOperation[state].animationItem.Length; i++) 
            {
                timeSum += experimentOperation[state].animationItem[i].animDuration;
            }
            Invoke("OnAnimationFinished", timeSum);     //播放动画后的操作
        }
    }

    void OnAnimationFinished()
    {
        isPlayingAnim = false;
        state++;
        string str = string.Empty;
        switch (state)
        {
            case 1:
                str = "请把圆底烧瓶里的二甲苯倒掉";
                break;
            case 2:
                str = "请取来乙酸乙酯";
                break;
            case 3:
                str = "请打开棕色试剂瓶瓶塞";
                break;
            case 4:
                str = "请将适量乙酸乙酯加入圆底烧瓶";
                break;
            case 5:
                str = "请将装有乙酸乙酯的棕色试剂瓶归位";
                break;
            case 6:
                str = "请将球形冷凝管安装到圆底烧瓶上";
                break;
            case 7:
                str = "请将干燥管安装到球形冷凝管上";
                break;
            case 8:
                str = "请为球形冷凝管通水";
                break;
            case 9:
                str = "加热1.5小时后，生成了乙酰乙酸乙酯的钠盐";
                Invoke("state_8_delay_func", 8f);
                iTween.ColorTo(waterYuanDiShaoPing, iTween.Hash(
                    "color", matOrange.color,
                    "time", 8f,
                    "easetype", iTween.EaseType.linear));
                break;
        }
        UIManager.UpdateInfo(str);
    }

    void state_8_delay_func()
    {
        yuanDiShaoPing.transform.Find("NaBalls").gameObject.SetActive(false);

        UIManager.UpdateInfo("实验第三部分完成");
        state = -1;
        UIManager.StopUpdateTime();
        btnNextSceneOpened = true;
        UIManager.ShowBtnNextScene("进入第四部分");
        ScoreManager.SaveScore(key);
    }

}

#if false
实验 第三部分 操作步骤

0	取下瓶塞
拖动 瓶塞->桌垫

1	把圆底烧瓶里的二甲苯倒掉
拖动 圆底烧瓶->废物皿

2	取来棕色试剂瓶
拖动 棕色试剂瓶->桌垫

3	打开棕色试剂瓶瓶塞
拖动 瓶塞->桌垫

4	将27.5ml乙酸乙酯加入圆底烧瓶
拖动 棕色试剂瓶->圆底烧瓶 （选择剂量）

5	将棕色试剂瓶归位
拖动 瓶塞->棕色试剂瓶（打开门，飞回）

6	安装冷凝管
拖动 冷凝管->圆底烧瓶

7	安装干燥管
拖动 干燥管->冷凝管

8  为冷凝管通水
拖动/点击 水龙头/冷凝管

#endif