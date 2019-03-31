using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Lab_step_2 : MonoBehaviour
{
    public GameObject waterTube;
    public GameObject rope_water_to_eq;
    public GameObject rope_eq_to_water;
    public GameObject rope2_eq_to_water;
    public GameObject NaBalls;
    public Animation rotatePointAnim;
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
    private int shakeNumber;
    private bool btnNextSceneOpened = false;
    private string key = "Lab_2";

    void Start()
    {
        ScoreManager.InitScore(key);

        rope2_eq_to_water.SetActive(false);
        Camera.main.fieldOfView = defaultFov;
        Camera camera = magnifierCamera.GetComponent<Camera>();
        camera.enabled = false;

        NaBalls.SetActive(false);
        shakeNumber = 0;
        audioManager = GetComponent<AudioManager>();
        hasSelected = false;
        isPlayingAnim = false;
        state = -1;
        opNum = experimentOperation.Length;
        UIManager = GetComponent<Lab_UIManager>();
        animationUtils = GetComponent<AnimationUtils>();
        animationUtils.HideRoadPoints(experimentOperation);
        UIManager.UpdateInfo(new string[] { "实验第二部分开始" , "请停止通冷凝水" });
    }
    
    private void Update()
    {
        bool isMouseDown = Input.GetMouseButtonDown(0);
        bool isMouseUp = Input.GetMouseButtonUp(0);
        if ((isMouseDown || isMouseUp) && !doseSelectionIsOpened && !isPlayingAnim && !btnNextSceneOpened
            && UIManager.firstPersonController.enabled)
        {
            if (state == -2)
            {
                SceneManager.LoadScene("Lab_3");
                //Debug.Log("to scene step_3");
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
                    if(isMouseUp)
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
        
    }

    GameObject yuanDiShaoPing;
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
                    if (hitInfo.collider.name.Equals("ganZaoGuan"))             //选择干燥管
                    {
                        hasSelected = true;
                    }
                    break;
                case 1:
                    if (hitInfo.collider.name.Equals("qiuXingLengNingGuan"))    //选择球形冷凝管
                    {
                        hasSelected = true;
                    }
                    break;
                case 2:
                    if (hitInfo.collider.name.Equals("StopperYuanDiShaoPing"))  //选择瓶塞
                    {
                        hasSelected = true;
                    }
                    break;
                case 3:
                    if (hitInfo.collider.name.Equals("yuanDiShaoPing"))
                    {
                        if (shakeNumber == 0)
                        {
                            UIManager.OnHandGrab();
                            yuanDiShaoPing = hitInfo.collider.gameObject;
                            yuanDiShaoPing.transform.SetParent(rotatePointAnim.gameObject.transform);
                        }
                        shakeNumber++;
                        isPlayingAnim = true;
                        rotatePointAnim.PlayQueued(rotatePointAnim.clip.name);
                        Invoke("state_3_delay_fun", rotatePointAnim.clip.length);
                        if (shakeNumber == 9) 
                        {
                            hasSelected = true;
                            NaBalls.SetActive(true);
                        }
                        //UIManager.OnCorrectChoose();
                        audioManager.PlayAudioCorrect();
                        return;
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
                    if (hitInfo.collider.name.Equals("tableMat"))           //将干燥管移动到桌垫上
                    {
                        PlayAnimation();
                        hasSelected = false;
                    }
                    break;
                case 1:
                    if (hitInfo.collider.name.Equals("tableMat"))          //将球形冷凝管移到桌垫上
                    {
                        PlayAnimation();
                        hasSelected = false;
                    }
                    break;
                case 2:
                    if (hitInfo.collider.name.Equals("yuanDiShaoPing"))           //给圆底烧瓶塞上瓶塞，并拿起圆底烧瓶
                    {
                        PlayAnimation();
                        hasSelected = false;
                    }
                    break;
                case 3:
                    PlayAnimation();
                    hasSelected = false;
                    UIManager.OnHandRelease();
                    return;
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

    void state_3_delay_fun()
    {
        isPlayingAnim = false;
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
                str = "请拆卸干燥管至桌面上";
                break;
            case 1:
                str = "请拆卸球形冷凝管至桌面上";
                break;
            case 2:
                str = "请用橡皮塞塞紧圆底烧瓶";
                break;
            case 3:
                str = "请快速摇动圆底烧瓶(快速点击)";
                break;
            case 4:
                str = "实验第二部分完成";
                state = -2;
                UIManager.StopUpdateTime();
                yuanDiShaoPing.transform.SetParent(null);
                btnNextSceneOpened = true;
                UIManager.ShowBtnNextScene("进入第三部分");
                ScoreManager.SaveScore(key);
                break;
        }
        UIManager.UpdateInfo(str);
    }
}

#if false
实验 第二部分 操作步骤

-1   停止通冷凝水
点击/拖动   冷凝管/水龙头

0	拆卸干燥管
拖动 干燥管->桌垫

1	拆卸球形冷凝管
拖动 球形冷凝管->桌垫

2	用橡皮塞塞紧圆底烧瓶（并拿起圆底烧瓶）
拖动 橡皮塞->圆底烧瓶

3	摇钠珠
快速点击圆底烧瓶


#endif