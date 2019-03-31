using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Lab_step_8 : MonoBehaviour
{
    public GameObject waterAnQuanPing;
    public GameObject fire;
    public GameObject rope_beng;
    public GameObject rope_water_to_eq;
    public GameObject rope_eq_to_water;
    
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
    private string key = "Lab_8";

    void Start()
    {
        ScoreManager.InitScore(key);

        waterAnQuanPing.SetActive(false);
        fire.SetActive(false);
        rope_beng.SetActive(false);
        rope_eq_to_water.SetActive(false);
        rope_water_to_eq.SetActive(false);

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
        UIManager.UpdateInfo(new string[] { "实验第八部分开始", "请将锥形瓶中常压蒸馏的产物移入蒸馏烧瓶" });
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
                //SceneManager.LoadScene("Lab_6");
                Debug.Log("finished");
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
                case 9:
                    if (myChoice.Equals("下进上出"))
                    {
                        isCorrect = true;
                    }
                    break;
            }
            if (isCorrect)
            {
                audioManager.PlayAudioCorrect();
                if (state == 9)
                {
                    rope_water_to_eq.SetActive(true);
                    rope_eq_to_water.SetActive(true);
                    Invoke("OnAnimationFinished", 1f);
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
        if (state == 9)
            UIManager.UpdateInfo(new string[] { "选择的通冷凝水的方法错误", "请重新选择" });

    }


    GameObject waterZhuiXingPing;
    GameObject waterZhenLiuShaoPing;

    void RaycastResultJudge(RaycastHit hitInfo, bool isMouseDown, bool isMouseUp)
    {
        if (hasSelected == false && isMouseDown)
        {
            switch (state)
            {
                case 0:
                    if (hitInfo.collider.name.Equals("zhuiXingPing"))
                    {
                        waterZhuiXingPing = hitInfo.collider.gameObject.transform.Find("waterZhuiXingPing").gameObject;
                        hasSelected = true;
                    }
                    break;
                case 1:
                    if (hitInfo.collider.name.Equals("keTou"))
                    {
                        hasSelected = true;
                    }
                    break;
                case 2:
                    if (hitInfo.collider.name.Equals("xiangJiaoGuan"))
                    {
                        hasSelected = true;
                    }
                    break;
                case 3:
                    if (hitInfo.collider.name.Equals("zhiShuiJia"))
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
                    if (hitInfo.collider.name.Equals("ShaoPing2"))
                    {
                        hasSelected = true;
                    }
                    break;
                case 8:
                    if (hitInfo.collider.name.Equals("anQuanPing"))
                    {
                        hasSelected = true;
                    }
                    break;
                case 9:
                    if (hitInfo.collider.name.Equals("waterTap") || hitInfo.collider.name.Equals("zhiXingLengNingGuan")) 
                    {
                        hasSelected = true;
                    }
                    break;
                case 10:
                    if (hitInfo.collider.name.Equals("anQuanPing") || hitInfo.collider.name.Equals("zhenKongBeng"))
                    {
                        hasSelected = true;
                    }
                    break;
                case 11:
                    if (hitInfo.collider.name.Equals("jiuJingPenDeng"))
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
                    if (hitInfo.collider.name.Equals("zhenLiuShaoPing"))
                    {
                        waterZhenLiuShaoPing = hitInfo.collider.gameObject.transform.Find("waterZhenLiuShaoPing").gameObject;
                        Invoke("state_0_delay_fun", 3f);
                        PlayAnimation();
                        hasSelected = false;
                    }
                    break;
                case 1:
                    if (hitInfo.collider.name.Equals("zhenLiuShaoPing"))
                    {
                        PlayAnimation();
                        hasSelected = false;
                    }
                    break;
                case 2:
                    if (hitInfo.collider.name.Equals("keTou"))
                    {
                        PlayAnimation();
                        hasSelected = false;
                    }
                    break;
                case 3:
                    if (hitInfo.collider.name.Equals("xiangJiaoGuan"))
                    {
                        PlayAnimation();
                        hasSelected = false;
                    }
                    break;
                case 4:
                    if (hitInfo.collider.name.Equals("keTou"))
                    {
                        PlayAnimation();
                        hasSelected = false;
                    }
                    break;
                case 5:
                    if (hitInfo.collider.name.Equals("keTou"))
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
                    if (hitInfo.collider.name.Equals("tableMat"))
                    {
                        PlayAnimation();
                        hasSelected = false;
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
                    if (hitInfo.collider.name.Equals("zhiXingLengNingGuan") || hitInfo.collider.name.Equals("waterTap")) 
                    {
                        multiSelectionIsOpened = true;
                        UIManager.ShowMultiSelection(new string[] { "请选择",
                                                                    "下进上出",
                                                                    "上进下出",
                                                                    "以上均可"}, "通冷凝水的方式");
                        hasSelected = false;

                        //rope_water_to_eq.SetActive(true);
                        //rope_eq_to_water.SetActive(true);
                        //Invoke("OnAnimationFinished", 1f);
                        //hasSelected = false;
                    }
                    break;
                case 10:
                    if (hitInfo.collider.name.Equals("anQuanPing") || hitInfo.collider.name.Equals("zhenKongBeng"))
                    {
                        rope_beng.SetActive(true);
                        Invoke("OnAnimationFinished", 1f);
                        hasSelected = false;
                    }
                    break;
                case 11:
                    if (hitInfo.collider.gameObject.name.Equals("jiuJingPenDeng"))
                    {
                        fire.SetActive(true);
                        Invoke("OnAnimationFinished", 1f);
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

    void state_0_delay_fun()
    {
        waterZhuiXingPing.SetActive(false);
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
                str = "请安装克氏蒸馏头";
                break;
            case 2:
                str = "请安装橡胶管";
                break;
            case 3:
                str = "请为橡胶管夹上止水夹";
                break;
            case 4:
                str = "请安装温度计";
                break;
            case 5:
                str = "请安装直形冷凝管";
                break;
            case 6:
                str = "请安装尾接管";
                break;
            case 7:
                str = "请取来烧瓶于尾接管下";
                break;
            case 8:
                str = "请安装安全瓶";
                break;
            case 9:
                str = "请为冷凝管通水";
                break;
            case 10:
                str = "请连接导管并开启真空泵";
                break;
            case 11:
                str = "请点燃酒精喷灯";
                break;
            case 12:
                str = "减压蒸馏一段时间后，得到最终产物乙酰乙酸乙酯";
                Invoke("state_12_delay_fun", 7f);
                break;
        }
        UIManager.UpdateInfo(str);
    }
    void state_12_delay_fun()
    {
        waterAnQuanPing.SetActive(true);
        state = -1;
        UIManager.StopUpdateTime();
        UIManager.UpdateInfo("实验完成");
        btnNextSceneOpened = true;
        UIManager.ShowBtnNextScene("查看实验成绩");
        ScoreManager.SaveScore(key);
    }

}

#if false

实验 第八部分 常压蒸馏 操作步骤

0	将锥形瓶中常压蒸馏的产物移入蒸馏烧瓶
拖动 锥形瓶 蒸馏烧瓶

1	安装克氏蒸馏头
拖动 克氏蒸馏头 蒸馏烧瓶

2	安装橡胶管
拖动 橡胶管 克氏蒸馏头

3	为橡胶管夹上止水夹
拖动 止水夹 橡胶管

4	安装温度计
拖动 温度计 克氏蒸馏头
//--------------------------------
5	安装直形冷凝管
拖动 直形冷凝管 克氏蒸馏头 

6	安装尾接管
拖动 尾接管 直形冷凝管

7	取来烧瓶于尾接管下
拖动 烧瓶 桌垫

8	安装安全瓶
拖动 安全瓶 尾接管
//--------------------------------
9	为冷凝管通水
拖动 水龙头 冷凝管

10	连接导管并开启真空泵
拖动 真空泵 吸滤瓶

11	点燃酒精喷灯
点击 酒精喷灯


#endif