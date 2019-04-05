using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Lab_step_5 : MonoBehaviour
{
    public Animation fenYeLouDouAnim;
    public Material orangeMat;
    public Material fenYeLouDouDownMat;
    public Material fenYeLouDouUpMat;

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
    private string key = "Lab_5";

    void Start()
    {
        ScoreManager.InitScore(key);

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
        UIManager.UpdateInfo(new string[] { "实验第五部分开始", "请将溶液移入分液漏斗中"});
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
                SceneManager.LoadScene("Lab_6");
                //Debug.Log("to scene step_6");
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
                case 3:
                    if (myChoice.Equals("和反应液等体积"))
                    {
                        isCorrect = true;
                        Invoke("state_3_delay_fun", 2.5f);
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
                ScoreManager.AddScore(-20);
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

    void state_3_delay_fun()
    {
        water_big_up.SetActive(true);
    }

    void OnWrongTip()
    {
        if (state == 3)
            UIManager.UpdateInfo(new string[] { "所选溶液的量不符合要求", "请往分液漏斗中加入和反应液等体积的饱和氯化钠溶液" });

    }

    GameObject waterC8H10;
    GameObject water_big_up;
    GameObject water_big_down;
    GameObject water_small;
    GameObject xiKouPingNaCl;
    GameObject stopperNaCl;
    GameObject stopperFenYeLouDou;
    GameObject fenYeLouDou;
    GameObject waterZhuiXingPing;
    GameObject huoSai = null;
    int rotateRound = 0;

    void RaycastResultJudge(RaycastHit hitInfo, bool isMouseDown, bool isMouseUp)
    {
        if (hasSelected == false && isMouseDown)
        {
            switch (state)
            {
                case 0:
                    if (hitInfo.collider.name.Equals("yuanDiShaoPing"))
                    {
                        GameObject yuanDiShaoPing = hitInfo.collider.gameObject;
                        waterC8H10 = yuanDiShaoPing.transform.Find("waterC8H10").gameObject;

                        hasSelected = true;
                    }
                    break;
                case 1:
                    if (hitInfo.collider.name.Equals("xiKouPingNaCl"))
                    {
                        xiKouPingNaCl = hitInfo.collider.gameObject;
                        hasSelected = true;
                    }
                    break;
                case 2:
                    if (hitInfo.collider.name.Equals("StopperNaCl"))
                    {
                        stopperNaCl = hitInfo.collider.gameObject;
                        hasSelected = true;
                    }
                    break;
                case 3:
                    if (hitInfo.collider.name.Equals("xiKouPingNaCl"))
                    {
                        stopperNaCl.transform.SetParent(null);
                        hasSelected = true;
                    }
                    break;
                case 4:
                    if (hitInfo.collider.name.Equals("StopperNaCl"))
                    {
                        stopperNaCl.transform.SetParent(xiKouPingNaCl.transform);
                        hasSelected = true;
                    }
                    break;
                case 5:
                    if (hitInfo.collider.name.Equals("StopperFenYeLouDou"))
                    {
                        stopperFenYeLouDou = hitInfo.collider.gameObject;
                        hasSelected = true;
                    }
                    break;
                case 6:
                    if (hitInfo.collider.gameObject.name.Equals("fenYeLouDou"))
                    {
                        if (shakeNumber == 0)
                        {
                            UIManager.OnHandGrab();
                            if (huoSai == null)
                                huoSai = hitInfo.collider.transform.Find("huoSai").gameObject;
                        }
                        shakeNumber++;
                        isPlayingAnim = true;
                        fenYeLouDouAnim.PlayQueued(fenYeLouDouAnim.clip.name);

                        if (shakeNumber == 9)
                        {
                            isPlayingAnim = true;
                            fenYeLouDouAnim["fenYeLouDouTilt"].time = 0;
                            fenYeLouDouAnim["fenYeLouDouTilt"].speed = 1;
                            fenYeLouDouAnim.PlayQueued("fenYeLouDouTilt");
                            Invoke("state_6_huoSaiRotate", fenYeLouDouAnim["fenYeLouDouTilt"].length);
                            rotateRound++;
                            if (rotateRound == 2)
                                hasSelected = true;
                            else
                                shakeNumber = 0;
                        }
                        else
                        {
                            Invoke("state_6_delay_fun", fenYeLouDouAnim.clip.length);
                        }
                        //UIManager.OnCorrectChoose();
                        audioManager.PlayAudioCorrect();
                        return;
                    }
                    break;
                case 7:
                    if (hitInfo.collider.name.Equals("StopperFenYeLouDou"))
                    {
                        hasSelected = true;
                    }
                    break;
                case 8:
                    if (hitInfo.collider.name.Equals("fenYeLouDou"))
                    {
                        hasSelected = true;
                    }
                    break;
                case 9:
                    if (hitInfo.collider.name.Equals("fenYeLouDou"))
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
        else if (hasSelected == true && (isMouseUp || state == 6)) 
        {
            switch (state)
            {
                case 0:
                    if (hitInfo.collider.name.Equals("fenYeLouDou"))
                    {
                        GameObject fenYeLouDou = hitInfo.collider.gameObject;
                        water_big_up = fenYeLouDou.transform.Find("water_big_up").gameObject;
                        water_big_down = fenYeLouDou.transform.Find("water_big_down").gameObject;
                        water_small = fenYeLouDou.transform.Find("water_small").gameObject;

                        Invoke("state_0_delay_func", 6f);
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
                    if (hitInfo.collider.name.Equals("fenYeLouDou"))
                    {
                        multiSelectionIsOpened = true;
                        UIManager.ShowMultiSelection(new string[] { "请选择",
                                                                    "少量",
                                                                    "大量",
                                                                    "和反应液等体积"},"加入适量饱和氯化钠溶液");
                        //PlayAnimation();
                        hasSelected = false;
                    }
                    break;
                case 4:
                    if (hitInfo.collider.name.Equals("xiKouPingNaCl"))
                    {
                        doorLeft.OpenDoor();
                        doorRight.OpenDoor();
                        
                        PlayAnimation();
                        hasSelected = false;
                    }
                    break;
                case 5:
                    if (hitInfo.collider.gameObject.name.Equals("fenYeLouDou"))
                    {
                        fenYeLouDou = hitInfo.collider.gameObject;
                        PlayAnimation();
                        hasSelected = false;
                    }
                    break;
                case 6:
                    if (hitInfo.collider.gameObject.name.Equals("fenYeLouDou"))
                    {
                        PlayAnimation();
                        hasSelected = false;
                        UIManager.OnHandRelease();
                        return;
                    }
                    break;
                case 7:
                    if (hitInfo.collider.gameObject.name.Equals("tableMat"))
                    {
                        PlayAnimation();
                        hasSelected = false;
                    }
                    break;
                case 8:
                    if (hitInfo.collider.gameObject.name.Equals("guanZiFeiWuMing"))
                    {
                        Invoke("state_8_delay_fun", 6f);
                        PlayAnimation();
                        hasSelected = false;
                    }
                    break;
                case 9:
                    if (hitInfo.collider.gameObject.name.Equals("zhuiXingPing"))
                    {
                        waterZhuiXingPing = hitInfo.collider.gameObject.transform.Find("waterZhuiXingPing").gameObject;

                        Invoke("state_9_delay_fun", 5f);
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
        isPlayingAnim = false;
    }

    void state_6_huoSaiRotate()
    {
        Animation anim = huoSai.GetComponent<Animation>();
        anim.PlayQueued(anim.clip.name);
        Invoke("state_6_fenYeLouDou_recover", anim.clip.length);
    }

    void state_6_fenYeLouDou_recover()
    {
        fenYeLouDouAnim["fenYeLouDouTilt"].time = fenYeLouDouAnim["fenYeLouDouTilt"].length;
        fenYeLouDouAnim["fenYeLouDouTilt"].speed = -1;
        fenYeLouDouAnim.Play("fenYeLouDouTilt");
        Invoke("state_6_delay_fun", fenYeLouDouAnim["fenYeLouDouTilt"].length);
    }

    void state_0_delay_func()
    {
        waterC8H10.SetActive(false);
        water_small.SetActive(false);
        water_big_up.SetActive(false);
        water_big_down.SetActive(true);
        water_big_up.GetComponent<Renderer>().material = orangeMat;
        water_big_down.GetComponent<Renderer>().material = orangeMat;

    }

    void state_8_delay_fun()
    {
        //water_big_down.SetActive(false);
        //water_big_up.SetActive(false);
        //water_small.SetActive(true);
        //water_small.GetComponent<Renderer>().material = fenYeLouDouUpMat;

        water_big_up.SetActive(false);
        water_big_down.GetComponent<Renderer>().material = fenYeLouDouUpMat;
    }

    void state_9_delay_fun()
    {
        water_big_down.SetActive(false);
        //water_small.SetActive(false);
        waterZhuiXingPing.SetActive(true);
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
                str = "请取来饱和氯化钠溶液";
                break;
            case 2:
                str = "请取下瓶塞";
                break;
            case 3:
                str = "请往分液漏斗中加入适量饱和氯化钠溶液";
                break;
            case 4:
                str = "请将饱和氯化钠溶液归位";
                break;
            case 5:
                str = "请为分液漏斗塞上瓶塞";
                break;
            case 6:
                stopperFenYeLouDou.transform.SetParent(fenYeLouDou.transform);
                iTween.RotateTo(fenYeLouDou, iTween.Hash(
                    "rotation", new Vector3(0, -90f, 0),
                    "easetype", iTween.EaseType.linear,
                    "time", 1.5f
                    ));
                //iTween.RotateTo(fenYeLouDou, new Vector3(0f, -90f, 0f), 1.5f);// default not linear
                str = "请摇动分液漏斗（点击）";
                break;
            case 7:
                str = "静置，等待液体分层";
                iTween.ColorTo(water_big_up, iTween.Hash(
                    "color", fenYeLouDouUpMat.color,
                    "time", 6f,
                    "easetype", iTween.EaseType.linear));
                iTween.ColorTo(water_big_down, iTween.Hash(
                    "color", fenYeLouDouDownMat.color,
                    "time", 6f,
                    "easetype", iTween.EaseType.linear));
                Invoke("state_7_delay_fun", 6f);
                break;
            case 8:
                str = "请将下层液体通过活塞旋钮从下口放掉，弃去";
                stopperFenYeLouDou.transform.SetParent(null);
                break;
            case 9:
                str = "将上层液体倒入锥形瓶中";
                break;
            case 10:
                str = "实验第五部分完成";
                state = -1;
                UIManager.StopUpdateTime();
                btnNextSceneOpened = true;
                UIManager.ShowBtnNextScene("进入第六部分");
                ScoreManager.SaveScore(key);
                break;
        }
        UIManager.UpdateInfo(str);
    }

    void state_7_delay_fun()
    {
        //water_big_up.GetComponent<Renderer>().material = fenYeLouDouUpMat;
        //water_big_down.GetComponent<Renderer>().material = fenYeLouDouDownMat;
        UIManager.UpdateInfo("请将分液漏斗橡胶塞拔出");
    }
    
}

#if false

实验 第五部分 操作步骤

0	将溶液移入分液漏斗中
拖动 圆底烧瓶中的溶液->分液漏斗

1	取来饱和氯化钠溶液
拖动 饱和氯化钠溶液->桌垫

2	取下瓶塞
拖动 瓶塞->桌垫

3	往分液漏斗中加入和反应液等体积的饱和氯化钠溶液
拖动 氯化钠溶液->分液漏斗

4	饱和氯化钠溶液归位
拖动 瓶塞->瓶身

5	为分液漏斗塞上瓶塞
拖动 瓶塞->分液漏斗

6	用力摇分液漏斗（动作有要求，视频中有）
快速点击 分液漏斗
//				静置分层
//				等待 （分层后产物集中在上层（上层颜色更浓））

7	将分液漏斗橡胶塞拔出
拖动 橡胶塞->桌垫

8 	请将下层液体通过活塞旋钮从下口放掉，弃去	（可考虑倒入水池扣分）	
拖动 分液漏斗->废物皿

9	将上层液体倒入锥形瓶中
拖动  分液漏斗->锥形瓶


#endif