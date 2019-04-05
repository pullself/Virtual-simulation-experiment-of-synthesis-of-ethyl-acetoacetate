using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Lab_step_6 : MonoBehaviour
{
    public GameObject jiaoTouDiGuanTou;
    public GameObject waterXiLvPing;
    public Material zhuiXingPingWaterMat;
    public GameObject rope;

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
    private int shakeNumber;
    private bool btnNextSceneOpened = false;
    private string key = "Lab_6";

    void Start()
    {
        ScoreManager.InitScore(key);

        rope.SetActive(false);

        Camera.main.fieldOfView = defaultFov;
        Camera camera = magnifierCamera.GetComponent<Camera>();
        camera.enabled = false;

        audioManager = GetComponent<AudioManager>();
        hasSelected = false;
        isPlayingAnim = false;
        state = 0;
        shakeNumber = 0;
        opNum = experimentOperation.Length;
        UIManager = GetComponent<Lab_UIManager>();
        animationUtils = GetComponent<AnimationUtils>();
        animationUtils.HideRoadPoints(experimentOperation);
        UIManager.UpdateInfo(new string[] { "实验第六部分开始", "请取来无水硫酸钠" });
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
                SceneManager.LoadScene("Lab_7");
                //Debug.Log("to scene step_7");
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

        //if (UIManager.multiSelectionIsConfirmed)
        //{
        //    multiSelectionIsOpened = false;
        //    UIManager.multiSelectionIsConfirmed = false;
        //    string myChoice = UIManager.multiSelectionMyChoice;
        //    bool isCorrect = false;
        //    switch (state)
        //    {
        //        case 3:
        //            if (myChoice.Equals("等量"))
        //            {
        //                isCorrect = true;
        //            }
        //            break;
        //    }
        //    if (isCorrect)
        //    {
        //        PlayAnimation();
        //        audioManager.PlayAudioCorrect();
        //    }
        //    else
        //    {
        //        Invoke("OnWrongTip", 1f);
        //        audioManager.PlayAudioWrong();
        //    }
        //}

        //if (UIManager.doseSelectionHasConfirmed == true)
        //{
        //    doseSelectionIsOpened = false;
        //    UIManager.doseSelectionHasConfirmed = false;
        //    float allowableDeviation = 0.1f;        //允许最大偏差10%
        //    float targetValue = float.MaxValue;
        //    switch (state)
        //    {
        //        case 4:                                                     //将2.5g钠加入圆底烧瓶
        //            targetValue = 2.5f;
        //            break;
        //        case 8:
        //            targetValue = 12.5f;                                    //将12.5ml二甲苯加入圆底烧瓶
        //            break;
        //        default:
        //            Debug.Log("error state");
        //            break;
        //    }
        //    if (Mathf.Abs(targetValue - UIManager.doseSelectionValue) / targetValue <= allowableDeviation)  //误差在10%以内
        //    {
        //        PlayAnimation();
        //        audioManager.PlayAudioCorrect();
        //        if (state == 8)
        //            Invoke("Case_8_Delay_Func", 2.5f);
        //    }
        //    else
        //    {
        //        audioManager.PlayAudioWrong();
        //        Invoke("OnWrongTip", 1f);
        //    }
        //}
    }

    //void OnWrongTip()
    //{
    //    if (state == 3)
    //        UIManager.UpdateInfo(new string[] { "所选溶液的量不符合要求", "请往分液漏斗中加入和反应液等体积的饱和氯化钠溶液" });

    //}

    GameObject fenMoSmall;
    GameObject waterZhuiXingPing;
    GameObject zhuiXingPing;
    GameObject buShiLouDou;
    GameObject stopperzongPing;
    GameObject zongPing;

    void RaycastResultJudge(RaycastHit hitInfo, bool isMouseDown, bool isMouseUp)
    {
        if (hasSelected == false && isMouseDown)
        {
            switch (state)
            {
                case 0:
                    if (hitInfo.collider.name.Equals("guangKouPingNa2SO4"))
                    {
                        hasSelected = true;
                    }
                    break;
                case 1:
                    if (hitInfo.collider.name.Equals("StopperNa2SO4"))
                    {
                        hasSelected = true;
                    }
                    break;
                case 2:
                    if (hitInfo.collider.name.Equals("guangKouPingNa2SO4"))
                    {
                        GameObject guangKouPingNa2SO4 = hitInfo.collider.gameObject;
                        fenMoSmall = guangKouPingNa2SO4.transform.Find("fenMoSmall").gameObject;
                        hasSelected = true;
                    }
                    break;
                case 3:
                    if (hitInfo.collider.name.Equals("StopperNa2SO4"))
                    {
                        fenMoSmall.transform.SetParent(zhuiXingPing.transform);
                        hasSelected = true;
                    }
                    break;
                case 4:
                    if (hitInfo.collider.name.Equals("XiLvPing"))
                    {
                        hasSelected = true;
                    }
                    break;
                case 5:
                    if (hitInfo.collider.name.Equals("buShiLouDou"))
                    {
                        hasSelected = true;
                    }
                    break;
                case 6:
                    if (hitInfo.collider.name.Equals("zhuiXingPing"))
                    {
                        hasSelected = true;
                    }
                    break;
                case 7:
                    if (hitInfo.collider.name.Equals("zhenKongBeng") || hitInfo.collider.gameObject.name.Equals("XiLvPing"))
                    {
                        hasSelected = true;
                    }
                    break;

                case 8:
                    if (hitInfo.collider.name.Equals("zongPing"))
                    {
                        hasSelected = true;
                    }
                    break;
                case 9:
                    if (hitInfo.collider.name.Equals("StopperzongPing"))
                    {
                        stopperzongPing = hitInfo.collider.gameObject;
                        hasSelected = true;
                    }
                    break;
                case 10:
                    if (hitInfo.collider.name.Equals("zongPing"))
                    {
                        zongPing = hitInfo.collider.gameObject;
                        stopperzongPing.transform.SetParent(null);
                        hasSelected = true;
                    }
                    break;
                case 11:
                    if (hitInfo.collider.name.Equals("StopperzongPing"))
                    {
                        stopperzongPing.transform.SetParent(zongPing.transform);
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
                    if (hitInfo.collider.name.Equals("tableMat"))
                    {
                        PlayAnimation();
                        hasSelected = false;
                    }
                    break;
                case 2:
                    if (hitInfo.collider.name.Equals("zhuiXingPing"))
                    {
                        zhuiXingPing = hitInfo.collider.gameObject;
                        waterZhuiXingPing = zhuiXingPing.transform.Find("waterZhuiXingPing").gameObject;
                        PlayAnimation();
                        hasSelected = false;
                    }
                    break;
                case 3:
                    if (hitInfo.collider.name.Equals("guangKouPingNa2SO4"))
                    {
                        PlayAnimation();
                        hasSelected = false;
                    }
                    break;
                case 4:
                    if (hitInfo.collider.name.Equals("tableMat"))
                    {
                        PlayAnimation();
                        hasSelected = false;
                    }
                    break;
                case 5:
                    if (hitInfo.collider.name.Equals("XiLvPing"))
                    {
                        PlayAnimation();
                        hasSelected = false;
                    }
                    break;
                case 6:
                    if (hitInfo.collider.name.Equals("buShiLouDou"))
                    {
                        buShiLouDou = hitInfo.collider.gameObject;
                        Invoke("state_6_delay_fun", 4f);
                        PlayAnimation();
                        hasSelected = false;
                    }
                    break;
                case 7:
                    if (hitInfo.collider.gameObject.name.Equals("XiLvPing") || hitInfo.collider.name.Equals("zhenKongBeng"))
                    {
                        //PlayAnimation();
                        rope.SetActive(true);
                        hasSelected = false;
                        Invoke("OnAnimationFinished", 2f);
                    }
                    break;
                case 8:
                    if (hitInfo.collider.name.Equals("tableMat"))
                    {
                        PlayAnimation();
                        hasSelected = false;
                    }
                    break;
                case 9:
                    if (hitInfo.collider.name.Equals("tableMat"))
                    {
                        PlayAnimation();
                        hasSelected = false;
                    }
                    break;
                case 10:
                    if (hitInfo.collider.name.Equals("buShiLouDou"))
                    {
                        PlayAnimation();

                        Invoke("state_10_ball_to_small", 0.5f * 2f);        //首次 放下前 挤压 胶头滴管 吸取
                        Invoke("state_10_ball_to_big", 0.5f * 3f + 0.1f);   //首次 放下后 放松 胶头滴管 吸取
                        Invoke("state_10_ball_to_small", 0.5f * 8f);        //首次 放下后 放松 胶头滴管 释放
                        Invoke("state_10_ball_to_big", 0.5f * 10f + 0.1f);  //首次 放下后 放松 胶头滴管 释放完成

                        Invoke("state_10_ball_to_small", 0.5f * 12f);       //第二次 放下前 挤压 胶头滴管 吸取
                        Invoke("state_10_ball_to_big", 0.5f * 13f + 0.1f);  //第二次 放下后 放松 胶头滴管 吸取
                        Invoke("state_10_ball_to_small", 0.5f * 18f);       //第二次 放下后 放松 胶头滴管 释放
                        Invoke("state_10_ball_to_big", 0.5f * 20f + 0.1f);  //第二次 放下后 放松 胶头滴管 释放完成

                        hasSelected = false;
                    }
                    break;
                case 11:
                    if (hitInfo.collider.name.Equals("zongPing"))
                    {
                        PlayAnimation();
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

    void state_6_delay_fun()
    {
        fenMoSmall.transform.SetParent(buShiLouDou.transform);

    }

    void state_10_ball_to_small()
    {
        iTween.ScaleTo(jiaoTouDiGuanTou, iTween.Hash(
                        "time", 0.5f,
                        "easetype", iTween.EaseType.linear,
                        "x", 0.015,
                        "z", 0.015));
    }

    void state_10_ball_to_big()
    {
        iTween.ScaleTo(jiaoTouDiGuanTou, iTween.Hash(
                        "time", 0.5f,
                        "easetype", iTween.EaseType.linear,
                        "x", 0.03,
                        "z", 0.03));
    }

    void OnAnimationStart() {}

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
                str = "请取下瓶塞";
                break;
            case 2:
                str = "请向产物中加入适量无水硫酸钠";
                break;
            case 3:
                Invoke("stete_2_delay_fun", 1f);
                str = "请将无水硫酸钠归位";
                break;
            case 4:
                str = "请取来吸滤瓶";
                break;
            case 5:
                str = "请安装布氏漏斗";
                break;
            case 6:
                str = "请将产物倒入布氏漏斗";
                break;
            case 7:
                str = "请连接并启用真空泵";
                break;
            case 8:
                Invoke("state_7_delay_fun", 1f);
                str = "请取来乙酸乙酯";
                break;
            case 9:
                str = "请取下瓶塞";
                break;
            case 10:
                str = "请用少量乙酸乙酯洗涤布氏漏斗里的硫酸钠";
                break;
            case 11:
                str = "请将乙酸乙酯归位";
                break;
            case 12:
                str = "实验第六部分完成";
                state = -1;
                UIManager.StopUpdateTime();
                btnNextSceneOpened = true;
                UIManager.ShowBtnNextScene("进入第七部分");
                ScoreManager.SaveScore(key);
                break;
        }
        UIManager.UpdateInfo(str);
    }

    void stete_2_delay_fun()
    {
        waterZhuiXingPing.SetActive(false);
        fenMoSmall.GetComponent<Renderer>().material = zhuiXingPingWaterMat;
    }

    void state_7_delay_fun()
    {
        waterXiLvPing.SetActive(true);
    }
}

#if false
实验 第六部分 操作步骤

0	取来无水硫酸钠
拖动 无水硫酸钠 桌垫

1	取下瓶塞
拖动 瓶塞桌垫

2	向产物中加入无水硫酸钠
拖动 无水硫酸钠->锥形瓶

3	将无水硫酸钠归位
拖动 瓶塞 无水硫酸钠广口瓶
//---------------------------------
4	取来吸滤瓶
拖动 吸滤瓶 桌垫

5	安装布氏漏斗
拖动 布氏漏斗 吸滤瓶

6	将产物倒入布氏漏斗
拖动 锥形瓶 布氏漏斗

7	连接并启用真空泵
拖动 真空泵 吸滤瓶
//---------------------------------
8	取来乙酸乙酯
拖动 乙酸乙酯 桌垫

9	取下瓶塞
拖动 瓶塞 桌垫

10	向布氏漏斗加入少量乙酸乙酯(用少量乙酸乙酯洗涤布氏漏斗里的硫酸钠)
拖动 乙酸乙酯 布氏漏斗

11	将乙酸乙酯归位
拖动 瓶塞 乙酸乙酯细口瓶


#endif