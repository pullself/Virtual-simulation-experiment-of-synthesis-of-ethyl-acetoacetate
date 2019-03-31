using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Lab_step_1 : MonoBehaviour
{
    public GameObject waterTube;
    public GameObject rope_water_to_eq;
    public GameObject rope_eq_to_water;
    public DraughtCupboardDoor doorLeft;
    public DraughtCupboardDoor doorRight;
    public GameObject magnifierCamera;
    public GameObject waterC8H10;
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
    private string key = "Lab_1";
    
    void Start()
    {
        ScoreManager.InitScore(key);

        waterTube.SetActive(false);
        rope_eq_to_water.SetActive(false);
        rope_water_to_eq.SetActive(false);

        Camera.main.fieldOfView = defaultFov;
        Camera camera = magnifierCamera.GetComponent<Camera>();
        camera.enabled = false;

        waterC8H10.SetActive(false);
        audioManager = GetComponent<AudioManager>();
        hasSelected = false;
        isPlayingAnim = false;
        state = 0;
        opNum = experimentOperation.Length;
        UIManager = GetComponent<Lab_UIManager>();
        animationUtils = GetComponent<AnimationUtils>();
        animationUtils.HideRoadPoints(experimentOperation);
        UIManager.UpdateInfo(new string[] { "实验第一部分开始" ,"请取来电热套"});
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
                SceneManager.LoadScene("Lab_2");
                //Debug.Log("to scene step_2");
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
                case 4:                                                     //将2.5g钠加入圆底烧瓶
                    targetValue = 2.5f;
                    break;
                case 8:
                    targetValue = 12.5f;                                    //将12.5ml二甲苯加入圆底烧瓶
                    break;
                default:
                    Debug.Log("error state");
                    break;
            }
            if (Mathf.Abs(targetValue - UIManager.doseSelectionValue) / targetValue <= allowableDeviation)  //误差在10%以内
            {
                PlayAnimation();
                audioManager.PlayAudioCorrect();
                if(state==8)
                    Invoke("Case_8_Delay_Func", 2.5f);
            }
            else
            {
                ScoreManager.AddScore(-20);
                audioManager.PlayAudioWrong();
                Invoke("OnWrongTip", 1f);
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
                case 12:
                    if (myChoice.Equals("下进上出"))
                    {
                        isCorrect = true;
                    }
                    break;
            }
            if (isCorrect)
            {
                audioManager.PlayAudioCorrect();
                if (state == 12)
                {
                    rope_water_to_eq.SetActive(true);
                    rope_eq_to_water.SetActive(true);
                    waterTube.SetActive(true);
                    iTween.ScaleFrom(waterTube, iTween.Hash(
                        "time", 4f,
                        "easetype", iTween.EaseType.linear,
                        "y", 0));
                    Invoke("OnAnimationFinished", 4f);
                }
                else
                {
                    PlayAnimation();
                }
            }
            else
            {
                ScoreManager.AddScore(-20);
                Invoke("OnWrongTip", 1f);
                audioManager.PlayAudioWrong();
            }
        }
    }

    

    void OnWrongTip()
    {
        if (state == 4)
            UIManager.UpdateInfo(new string[] { "所选钠的量不符合要求", "请将2.5g钠加入圆底烧瓶" });
        else if (state == 8)
            UIManager.UpdateInfo(new string[] { "所选二甲苯的量不符合要求", "请将12.5ml二甲苯加入圆底烧瓶" });
        else if(state==12)
            UIManager.UpdateInfo(new string[] { "选择的通冷凝水的方法错误", "请重新选择" });
    }

    void Case_8_Delay_Func()
    {
        waterC8H10.SetActive(true);
    }

    GameObject yuanDiShaoPing;
    GameObject guangKouPingNa;
    GameObject naSmallCube;
    GameObject xiKouPingC8H10;
    GameObject stopperC8H10;

    void RaycastResultJudge(RaycastHit hitInfo, bool isMouseDown, bool isMouseUp)
    {
        if (hasSelected == false && isMouseDown)
        {
            switch (state)
            {
                case 0:
                    if (hitInfo.collider.name.Equals("dianReTao"))          //选择电热套
                    {
                        hasSelected = true;
                    }
                    break;
                case 1:
                    if (hitInfo.collider.name.Equals("yuanDiShaoPing"))     //选择圆底烧瓶
                    {
                        yuanDiShaoPing = hitInfo.collider.gameObject;
                        hasSelected = true;
                    }
                    break;
                case 2:
                    if (hitInfo.collider.name.Equals("guangKouPingNa"))     //选择装有钠的广口瓶
                    {
                        guangKouPingNa = hitInfo.collider.gameObject;
                        hasSelected = true;
                    }
                    break;
                case 3:
                    if (hitInfo.collider.name.Equals("StopperNa"))          //选择广口瓶瓶塞
                    {
                        hasSelected = true;
                    }
                    break;
                case 4:
                    if (hitInfo.collider.name.Equals("guangKouPingNa"))     //选择装有钠的广口瓶
                    {
                        hasSelected = true;
                    }
                    break;
                case 5:
                    if (hitInfo.collider.name.Equals("StopperNa"))          //选择广口瓶瓶塞
                    {
                        hasSelected = true;
                        //在进行广口瓶归位操作前，先将小钠块的父物体设置为圆底烧瓶，
                        //避免小钠块跟随广口瓶移动
                        Transform[] tr = guangKouPingNa.GetComponentsInChildren<Transform>();
                        for (int i = 0; i < tr.Length; i++)
                        {
                            if (tr[i].name.Equals("NaSmallCube"))
                            {
                                naSmallCube = tr[i].gameObject;
                                tr[i].SetParent(yuanDiShaoPing.transform);
                                break;
                            }
                        }
                    }
                    break;
                case 6:
                    if (hitInfo.collider.name.Equals("xiKouPingC8H10"))     //选择装有二甲苯的细口瓶
                    {
                        hasSelected = true;
                    }
                    break;
                case 7:
                    if (hitInfo.collider.name.Equals("StopperC8H10"))       //选择细口瓶瓶塞
                    {
                        stopperC8H10 = hitInfo.collider.gameObject;
                        hasSelected = true;
                    }
                    break;
                case 8:
                    if (hitInfo.collider.name.Equals("xiKouPingC8H10"))     //选择细口瓶
                    {
                        stopperC8H10.transform.SetParent(null);             //移动细口瓶前将细口瓶瓶塞的父物体设置为空，避免跟随移动
                        xiKouPingC8H10 = hitInfo.collider.gameObject;       
                        hasSelected = true;
                    }
                    break;
                case 9:
                    if (hitInfo.collider.name.Equals("StopperC8H10"))       //选择细口瓶瓶塞
                    {
                        //细口瓶移动完成，重新将瓶塞的父物体设置为细口瓶。
                        stopperC8H10.transform.SetParent(xiKouPingC8H10.transform);

                        hasSelected = true;
                    }
                    break;
                case 10:
                    if (hitInfo.collider.name.Equals("qiuXingLengNingGuan"))//选择冷凝管
                    {
                        hasSelected = true;
                    }
                    break;
                case 11:
                    if (hitInfo.collider.name.Equals("ganZaoGuan"))         //选择干燥管
                    {
                        hasSelected = true;
                    }
                    break;
                case 12:
                    if (hitInfo.collider.name.Equals("waterTap") || hitInfo.collider.name.Equals("qiuXingLengNingGuan"))
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
        else if(hasSelected == true && isMouseUp)
        {
            switch (state)
            {
                case 0:
                    if (hitInfo.collider.name.Equals("tableMat"))           //将电热套移动到桌垫上
                    {
                        PlayAnimation();
                        hasSelected = false;
                    }
                    break;
                case 1:
                    if (hitInfo.collider.name.Equals("dianReTao"))          //将圆底烧瓶放到电热套上
                    {
                        PlayAnimation();
                        hasSelected = false;
                    }
                    break;
                case 2:
                    if (hitInfo.collider.name.Equals("tableMat"))           //取来装有钠的广口瓶
                    {
                        PlayAnimation();
                        hasSelected = false;
                    }
                    break;
                case 3:
                    if (hitInfo.collider.name.Equals("tableMat"))           //取下广口瓶瓶塞
                    {
                        PlayAnimation();
                        hasSelected = false;
                    }
                    break;
                case 4:
                    if (hitInfo.collider.name.Equals("yuanDiShaoPing"))     //将2.5g钠加入圆底烧瓶
                    {
                        doseSelectionIsOpened = true;
                        //PlayAnimation();  //不直接播放动画，而是先让用户选择剂量再播放。
                        UIManager.ShowDoseSelection("g", 0.1f, 5f, Random.Range(0.1f, 5f), "种类:钠", "请选择剂量");
                        hasSelected = false;
                    }
                    break;
                case 5:
                    if (hitInfo.collider.name.Equals("guangKouPingNa"))           //广口瓶归位
                    {
                        doorLeft.OpenDoor();
                        doorRight.OpenDoor();

                        PlayAnimation();
                        hasSelected = false;
                    }
                    break;
                case 6:
                    if (hitInfo.collider.name.Equals("tableMat"))                 //取来装有二甲苯的细口瓶
                    {
                        PlayAnimation();
                        hasSelected = false;
                    }
                    break;
                case 7:
                    if (hitInfo.collider.name.Equals("tableMat"))                 //取下细口瓶瓶塞
                    {
                        PlayAnimation();
                        hasSelected = false;
                    }
                    break;
                case 8:
                    if (hitInfo.collider.name.Equals("yuanDiShaoPing"))           //将12.5ml二甲苯加入圆底烧瓶
                    {
                        //PlayAnimation();
                        doseSelectionIsOpened = true;
                        UIManager.ShowDoseSelection("ml", 0.1f, 25f, Random.Range(0.1f, 25f), "种类:二甲苯", "请选择剂量");
                        hasSelected = false;
                    }
                    break;
                case 9:
                    if (hitInfo.collider.name.Equals("xiKouPingC8H10"))           //细口瓶归位
                    {
                        doorLeft.OpenDoor();
                        doorRight.OpenDoor();

                        PlayAnimation();
                        hasSelected = false;
                    }
                    break;
                case 10:
                    if (hitInfo.collider.name.Equals("yuanDiShaoPing"))           //安装冷凝管
                    {
                        PlayAnimation();
                        hasSelected = false;
                    }
                    break;
                case 11:
                    if (hitInfo.collider.name.Equals("qiuXingLengNingGuan"))      //安装干燥管
                    {
                        PlayAnimation();
                        hasSelected = false;
                    }
                    break;
                case 12:
                    if (hitInfo.collider.name.Equals("waterTap") || hitInfo.collider.name.Equals("qiuXingLengNingGuan"))
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
           
            //Invoke("OnAnimationFinished", animationUtils.currAnimDuration);     //播放动画后的操作
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
                str = "请将圆底烧瓶放到电热套上";
                //state = -1;
                //UIManager.StopUpdateTime();
                //UIManager.ShowBtnNextScene("进入第二步实验");
                break;
            case 2:
                str = "请取来装有钠的广口瓶";
                break;
            case 3:
                str = "请取下广口瓶瓶塞";
                break;
            case 4:
                str = "请将适量钠加入圆底烧瓶";
                break;
            case 5:
                str = "请将广口瓶归位";
                break;
            case 6:
                str = "请取来装有二甲苯的细口瓶";
                break;
            case 7:
                str = "请取下细口瓶瓶塞";
                break;
            case 8:
                str = "请将适量二甲苯加入圆底烧瓶";
                break;
            case 9:
                str = "请将细口瓶归位";
                break;
            case 10:
                str = "请将球形冷凝管安装到圆底烧瓶上";
                break;
            case 11:
                str = "请将干燥管安装到球形冷凝管上";
                break;
            case 12:
                str = "请为球形冷凝管通水";
                break;
            case 13:
                str = "用电热套小心加热一段时间使钠融化";
                Invoke("state_13_delay_func", 4.5f);
                break;
        }
        UIManager.UpdateInfo(str);
    }

    void state_13_delay_func()
    {
        naSmallCube.SetActive(false);
        UIManager.UpdateInfo("实验第一部分完成");
        state = -1;
        UIManager.StopUpdateTime();
        btnNextSceneOpened = true;
        UIManager.ShowBtnNextScene("进入第二部分");
        ScoreManager.SaveScore(key);
    }

}

#if false

实验 第一部分 操作步骤

0	取来电热套
拖动 电热套->桌垫

1	将圆底烧瓶放到电热套上
拖动 圆底烧瓶->电热套

2	取来装有钠的广口瓶
拖动 装有钠的广口瓶->桌垫

3	取下广口瓶瓶塞
拖动 广口瓶瓶塞->桌垫

4	将2.5g钠加入圆底烧瓶
拖动 装有钠的广口瓶->圆底烧瓶

5	广口瓶归位
拖动 瓶塞->瓶身

6	取来装有二甲苯的细口瓶
拖动 装有二甲苯的细口瓶->桌垫

7	取下细口瓶瓶塞
拖动 细口瓶瓶塞->桌垫

8	将12.5ml二甲苯加入圆底烧瓶
拖动 装有二甲苯的细口瓶->圆底烧瓶

9	细口瓶归位
拖动 瓶塞->瓶身

10	安装冷凝管
拖动 冷凝管->圆底烧瓶

11	安装干燥管
拖动 干燥管->冷凝管

12  为冷凝管通水
拖动/点击 水龙头/冷凝管

#endif