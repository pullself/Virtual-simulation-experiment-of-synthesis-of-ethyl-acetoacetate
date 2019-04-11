using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Lab_step_4 : MonoBehaviour
{
    public Material waterTransparentMat;
    public GameObject waterTube;
    public GameObject rope_water_to_eq;
    public GameObject rope_eq_to_water;
    public GameObject rope2_eq_to_water;

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
    private string key = "Lab_4";

    void Start()
    {
        ScoreManager.InitScore(key,10);

        rope2_eq_to_water.SetActive(false);
        Camera.main.fieldOfView = defaultFov;
        Camera camera = magnifierCamera.GetComponent<Camera>();
        camera.enabled = false;

        audioManager = GetComponent<AudioManager>();
        hasSelected = false;
        isPlayingAnim = false;
        state = -1;
        opNum = experimentOperation.Length;
        UIManager = GetComponent<Lab_UIManager>();
        animationUtils = GetComponent<AnimationUtils>();
        animationUtils.HideRoadPoints(experimentOperation);
        UIManager.UpdateInfo(new string[] { "实验第四部分开始", "等待反应产物冷却到室温", "请停止通冷凝水" });
    }

    private void Update()
    {
        bool isMouseDown = Input.GetMouseButtonDown(0);
        bool isMouseUp = Input.GetMouseButtonUp(0);
        if ((isMouseDown || isMouseUp) && !doseSelectionIsOpened && !multiSelectionIsOpened && !isPlayingAnim && !btnNextSceneOpened
            && UIManager.firstPersonController.enabled)
        {
            if (state == -2)
            {
                SceneManager.LoadScene("Lab_5");
                //Debug.Log("to scene step_5");
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
                    if (myChoice.Equals("弱酸性"))
                    {
                        isCorrect = true;
                        Invoke("state_4_delay_fun", 2.5f);
                    }
                    break;
            }
            if (isCorrect)
            {
                PlayAnimation();
                audioManager.PlayAudioCorrect();
            }
            else
            {
                audioManager.PlayAudioWrong();
                Invoke("OnWrongTip", 1f);
                ScoreManager.AddScore(-10);
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
        if (state == 4)
            UIManager.UpdateInfo(new string[] { "所滴加的醋酸的量不符合要求", "请向圆底烧瓶中加入适量醋酸使溶液变成弱酸性" });

    }

    void state_4_delay_fun()
    {
        waterYuanDiShaoPing.GetComponent<Renderer>().material = waterTransparentMat;
    }

    GameObject xiKouPingCuSuan;
    GameObject stopperCuSuan;
    GameObject waterYuanDiShaoPing;

    void RaycastResultJudge(RaycastHit hitInfo, bool isMouseDown, bool isMouseUp)
    {
        if (hasSelected == false && isMouseDown)
        {
            switch (state)
            {
                case -1:
                    if (hitInfo.collider.name.Equals("qiuXingLengNingGuan") || hitInfo.collider.name.Equals("waterTap"))
                    {
                        hasSelected = true;
                    }
                    break;
                case 0:
                    if (hitInfo.collider.name.Equals("ganZaoGuan"))
                    {
                        hasSelected = true;
                    }
                    break;
                case 1:
                    if (hitInfo.collider.name.Equals("qiuXingLengNingGuan"))
                    {
                        hasSelected = true;
                    }
                    break;
                case 2:
                    if (hitInfo.collider.name.Equals("xiKouPingCuSuan"))
                    {
                        xiKouPingCuSuan = hitInfo.collider.gameObject;
                        hasSelected = true;
                    }
                    break;
                case 3:
                    if (hitInfo.collider.name.Equals("StopperCuSuan"))
                    {
                        stopperCuSuan = hitInfo.collider.gameObject;
                        hasSelected = true;
                    }
                    break;
                case 4:
                    if (hitInfo.collider.name.Equals("xiKouPingCuSuan"))
                    {
                        stopperCuSuan.transform.SetParent(null);
                        hasSelected = true;
                    }
                    break;
                case 5:
                    if (hitInfo.collider.name.Equals("StopperCuSuan"))
                    {
                        stopperCuSuan.transform.SetParent(xiKouPingCuSuan.transform);
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
                case -1:
                    if (hitInfo.collider.name.Equals("qiuXingLengNingGuan") || hitInfo.collider.name.Equals("waterTap"))
                    {
                        isPlayingAnim = true;
                        rope_eq_to_water.SetActive(false);
                        rope_water_to_eq.SetActive(false);
                        rope2_eq_to_water.SetActive(true);
                        iTween.ScaleTo(waterTube, iTween.Hash(
                            "time", 4f,
                            "easetype", iTween.EaseType.linear,
                            "y", 0.001));
                        Invoke("state_neg1_delay_fun", 5f);
                        hasSelected = false;
                    }
                    break;
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
                        waterYuanDiShaoPing = hitInfo.collider.transform.Find("waterC8H10").gameObject;
                        multiSelectionIsOpened = true;
                        UIManager.ShowMultiSelection(new string[] { "请选择",
                                                                    "强酸性",
                                                                    "弱酸性",
                                                                    "中性",
                                                                    "弱碱性",
                                                                    "强碱性"},"加入适量醋酸使溶液变成：");
                        hasSelected = false;
                    }
                    break;
                case 5:
                    if (hitInfo.collider.name.Equals("xiKouPingCuSuan"))
                    {
                        doorLeft.OpenDoor();
                        doorRight.OpenDoor();

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
                ScoreManager.AddScore(-1);
            }
        }
    }

    void state_neg1_delay_fun()
    {
        waterTube.SetActive(false);
        rope2_eq_to_water.SetActive(false);
        OnAnimationFinished();
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
            case 0:
                str = "请拆下干燥管";
                break;
            case 1:
                str = "请拆下冷凝管";
                break;
            case 2:
                str = "请取来50%醋酸";
                break;
            case 3:
                str = "请取下瓶塞";
                break;
            case 4:
                str = "请向圆底烧瓶中加入适量醋酸";
                break;
            case 5:
                str = "请将醋酸归位";
                break;
            case 6:
                str = "实验第四部分完成";
                state = -2;
                UIManager.StopUpdateTime();
                if (Time.time - UIManager.startSceneTime > 360f)    //超过6分钟
                {
                    ScoreManager.AddScore(-999);                    //分数置0
                }
                btnNextSceneOpened = true;
                UIManager.ShowBtnNextScene("进入第五部分");
                ScoreManager.SaveScore(key);
                break;
        }
        UIManager.UpdateInfo(str);
    }

}

#if false

实验 第四部分 操作步骤

等待反应产物冷却到室温

-1   停止通冷凝水
点击/拖动   冷凝管/水龙头

0	拆下干燥管
干燥管	桌垫

1	拆下冷凝管
冷凝管	桌垫

2	取来50%醋酸
醋酸	桌垫

3	取下瓶塞
瓶塞	桌垫

4	向圆底烧瓶中加入适量醋酸
醋酸	圆底烧瓶

5	醋酸归位
瓶塞	醋酸


#endif