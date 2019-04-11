using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Lab_step_7 : MonoBehaviour
{
    public GameObject ropeZhenKongBeng;
    public GameObject rope_water_to_eq;
    public GameObject rope_eq_to_water;
    public GameObject fire;
    public GameObject waterZhiXingLengNingGuan;

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
    private string key = "Lab_7";

    void Start()
    {
        ScoreManager.InitScore(key,15);

        rope_water_to_eq.SetActive(false);
        rope_eq_to_water.SetActive(false);
        fire.SetActive(false);
        waterZhiXingLengNingGuan.SetActive(false);

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
        UIManager.UpdateInfo(new string[] { "实验第七部分开始", "请关闭真空泵并拆卸导管" });
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
                SceneManager.LoadScene("Lab_8");
                //Debug.Log("to scene step_8");
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

        if (UIManager.multiSelectionIsConfirmed)
        {
            multiSelectionIsOpened = false;
            UIManager.multiSelectionIsConfirmed = false;
            string myChoice = UIManager.multiSelectionMyChoice;
            bool isCorrect = false;
            switch (state)
            {
                case 4:
                    if (myChoice.Equals("支管口处"))
                    {
                        isCorrect = true;
                    }
                    else
                    {
                        ScoreManager.AddScore(-10);
                    }
                    break;
                case 8:
                    if (myChoice.Equals("下进上出"))
                    {
                        isCorrect = true;
                    }
                    else
                    {
                        ScoreManager.AddScore(-5);
                    }
                    break;
            }
            if (isCorrect)
            {
                audioManager.PlayAudioCorrect();
                if (state == 8)
                {
                    rope_eq_to_water.SetActive(true);
                    rope_water_to_eq.SetActive(true);
                    waterZhiXingLengNingGuan.SetActive(true);
                    OnAnimationFinished();
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
            }
        }

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

    void OnWrongTip()
    {
        if (state==4)
            UIManager.UpdateInfo(new string[] { "温度计水银球位置选择错误", "请重新选择" });
        if (state == 8)
            UIManager.UpdateInfo(new string[] { "选择的通冷凝水的方法错误", "请重新选择" });
        
    }

    GameObject waterXiLvPing;
    GameObject waterZhenLiuShaoPing;
    GameObject waterZhuiXingPing;

    void RaycastResultJudge(RaycastHit hitInfo, bool isMouseDown, bool isMouseUp)
    {
        if (hasSelected == false && isMouseDown)
        {
            switch (state)
            {
                case 0:
                    if (hitInfo.collider.name.Equals("XiLvPing") || hitInfo.collider.name.Equals("zhenKongBeng"))
                    {
                        hasSelected = true;
                    }
                    break;
                case 1:
                    if (hitInfo.collider.name.Equals("buShiLouDou"))
                    {
                        hasSelected = true;
                    }
                    break;
                case 2:
                    if (hitInfo.collider.name.Equals("XiLvPing"))
                    {
                        GameObject xiLvPing = hitInfo.collider.gameObject;
                        waterXiLvPing = xiLvPing.transform.Find("waterXiLvPing").gameObject;
                        hasSelected = true;
                    }
                    break;
                case 3:
                    if (hitInfo.collider.name.Equals("zhenLiuZhiGuan"))
                    {
                        hasSelected = true;
                    }
                    break;
                case 4:
                    if (hitInfo.collider.name.Equals("wenDuJi"))
                    {
                        hasSelected = true;
                    }
                    break;
                case 5:
                    if (hitInfo.collider.name.Equals("zhiXingLengNingGuan"))
                    {
                        hasSelected = true;
                    }
                    break;
                case 6:
                    if (hitInfo.collider.name.Equals("weiJieGuan"))
                    {
                        hasSelected = true;
                    }
                    break;
                case 7:
                    if (hitInfo.collider.name.Equals("zhuiXingPing"))
                    {
                        waterZhuiXingPing = hitInfo.collider.gameObject.transform.Find("waterZhuiXingPing").gameObject;
                        hasSelected = true;
                    }
                    break;
                case 8:
                    if (hitInfo.collider.name.Equals("zhiXingLengNingGuan") || hitInfo.collider.name.Equals("waterTap"))
                    {
                        hasSelected = true;
                    }
                    break;
                case 9:
                    if (hitInfo.collider.name.Equals("酒精灯"))
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
                ScoreManager.AddScore(-1);
            }
        }
        else if (hasSelected == true && isMouseUp)
        {
            switch (state)
            {
                case 0:
                    if (hitInfo.collider.name.Equals("XiLvPing") || hitInfo.collider.name.Equals("zhenKongBeng"))
                    {
                        //PlayAnimation();
                        ropeZhenKongBeng.SetActive(false);
                        //OnAnimationFinished();
                        Invoke("Delay_OnAnimationFinished", 1f);
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
                    if (hitInfo.collider.name.Equals("zhenLiuShaoPing"))
                    {
                        GameObject zhenLiuShaoPing = hitInfo.collider.gameObject;
                        waterZhenLiuShaoPing = zhenLiuShaoPing.transform.Find("waterZhenLiuShaoPing").gameObject;
                        Invoke("state_2_delay_fun", 4f);
                        PlayAnimation();
                        hasSelected = false;
                    }
                    break;
                case 3:
                    if (hitInfo.collider.name.Equals("zhenLiuShaoPing"))
                    {
                        PlayAnimation();
                        hasSelected = false;
                    }
                    break;
                case 4:
                    if (hitInfo.collider.name.Equals("zhenLiuZhiGuan"))
                    {
                        //PlayAnimation();
                        multiSelectionIsOpened = true;
                        UIManager.ShowMultiSelection(new string[] { "请选择",
                                                                    "支管口处",
                                                                    "液体中",
                                                                    "液体表面"}, " 温度计水银球位置");
                        hasSelected = false;
                    }
                    break;
                case 5:
                    if (hitInfo.collider.name.Equals("zhenLiuZhiGuan"))
                    {
                        PlayAnimation();
                        hasSelected = false;
                    }
                    break;
                case 6:
                    if (hitInfo.collider.name.Equals("zhiXingLengNingGuan"))
                    {
                        PlayAnimation();
                        hasSelected = false;
                    }
                    break;
                case 7:
                    if (hitInfo.collider.name.Equals("tableMat") || hitInfo.collider.name.Equals("weiJieGuan"))
                    {
                        PlayAnimation();
                        hasSelected = false;
                    }
                    break;
                case 8:
                    if (hitInfo.collider.name.Equals("zhiXingLengNingGuan") || hitInfo.collider.name.Equals("waterTap"))
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
                case 9:
                    if (hitInfo.collider.name.Equals("酒精灯"))
                    {
                        //PlayAnimation();
                        fire.SetActive(true);
                        //OnAnimationFinished();
                        Invoke("Delay_OnAnimationFinished", 1f);
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
                ScoreManager.AddScore(-1);
            }
        }
    }

    void Delay_OnAnimationFinished()
    {
        OnAnimationFinished();
    }

    void state_2_delay_fun()
    {
        waterXiLvPing.SetActive(false);
        waterZhenLiuShaoPing.SetActive(true);
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
                str = "请拆卸布氏漏斗";
                break;
            case 2:
                str = "请将产物倒入蒸馏烧瓶";
                break;
            case 3:
                str = "请安装普通蒸馏头";
                break;
            case 4:
                str = "请安装温度计";
                break;
            case 5:
                str = "请安装冷凝管";
                break;
            case 6:
                str = "请安装尾接管";
                break;
            case 7:
                str = "请取来锥形瓶";
                break;
            case 8:
                str = "请为冷凝管通水";
                break;
            case 9:
                str = "请点燃酒精灯(点击)";
                break;
            case 10:
                str = "常压蒸馏一段时间后，把乙酸乙酯这样的低沸点液体蒸馏出来";
                Invoke("state_10_delay_fun", 7f);
                break;
        }
        UIManager.UpdateInfo(str);
    }

    void state_10_delay_fun()
    {
        waterZhuiXingPing.SetActive(true);
        state = -1;
        UIManager.StopUpdateTime();
        if (Time.time - UIManager.startSceneTime > 360f)    //超过6分钟
        {
            ScoreManager.AddScore(-999);                    //分数置0
        }
        UIManager.UpdateInfo("实验第七部分完成");
        btnNextSceneOpened = true;
        UIManager.ShowBtnNextScene("进入第八部分");
        ScoreManager.SaveScore(key);
    }

}

#if false

实验 第七部分 常压蒸馏 操作步骤

0	关闭真空泵并拆卸导管
任意点击 吸滤瓶、真空泵

1	拆卸布氏漏斗
拖动 漏斗 桌垫

2	将产物倒入蒸馏烧瓶
拖动 吸滤瓶 蒸馏烧瓶

//------------------------

3	安装蒸馏支管(?)普通蒸馏头
拖动 蒸馏支管 蒸馏烧瓶

4	安装温度计
拖动 温度计 蒸馏支管

5	安装直形冷凝管
拖动 直形冷凝管 蒸馏支管

6	安装尾接管
拖动 尾接管 直形冷凝管

7	取来锥形瓶
拖动 锥形瓶 桌垫

//------------------------

8	为冷凝管通水
拖动 水龙头 冷凝管

9	点燃酒精灯
点击 酒精灯

常压蒸馏一段时间后，把乙酸乙酯这样的低沸点液体蒸馏出来



#endif