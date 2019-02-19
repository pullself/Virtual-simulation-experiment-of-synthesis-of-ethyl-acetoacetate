using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lab_step_1 : MonoBehaviour
{
    public Animation[] state_1_obj;
    public Animation[] state_2_obj;
    public Animation[] state_3_obj;
    public Animation[] state_4_obj;
    public Animation[] state_5_obj;
    public Animation[] state_6_obj;
    public Animation state_7_obj;
    public GameObject state_7_waterC8H10;
    public Animation[] state_8_obj;
    public Animation[] state_9_obj;
    public Animation[] state_10_obj;
    public Animation state_11_dianReTao;

    private Lab_UIManager UIManager;
    private int state = 1;
    

    void Start()
    {
        UIManager = GetComponent<Lab_UIManager>();
        UIManager.UpdateInfo(new string[]{ "实验开始，请开始你的操作", "按E可显示/隐藏本菜单" });
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))    //点击鼠标左键时，利用射线检测Equipment层的物体
        {
            RaycastHit hitInfo;
            Vector2 screenMidPoint = new Vector2(Screen.width / 2, Screen.height / 2);
            LayerMask mask = 1 << (LayerMask.NameToLayer("Equipment"));//layer为枚举
            if (Physics.Raycast(Camera.main.ScreenPointToRay(screenMidPoint), out hitInfo, 32, mask.value))
            {
                Debug.Log(hitInfo.collider.name);
                //UIManager.UpdateInfo(hitInfo.collider.name);
                ChangeState(hitInfo);
            }
        }
    }


    GameObject yuanDiShaoPingObj = null;
    GameObject stoperTmp = null;
    GameObject xikoupingObj = null;
    GameObject qiuXingLengNingGuan = null;
    GameObject ganZaoGuan = null;
    void ChangeState(RaycastHit hitInfo)
    {
        switch (state)
        {
            case 1:
                if (hitInfo.collider.name.Equals("yuanDiShaoPing"))
                {
                    yuanDiShaoPingObj = hitInfo.collider.gameObject;
                    PlayAnimation(state_1_obj);
                    UIManager.UpdateInfo("移动圆底烧瓶");
                    state++; //state = 5;
                }
                break;
            case 2:
                if (hitInfo.collider.name.Equals("guangKouPingNa"))
                {
                    PlayAnimation(state_2_obj);
                    UIManager.UpdateInfo("移动装有Na的广口瓶");
                    state++;
                }
                break;
            case 3:
                if (hitInfo.collider.name.Equals("Stopper"))
                {
                    PlayAnimation(state_3_obj);
                    UIManager.UpdateInfo(new string[] { "打开瓶塞", "将Na放入圆底烧瓶" });
                    state++;
                }
                break;
            case 4:
                foreach (var childObj in state_3_obj)
                {
                    if (childObj.gameObject.name.Equals("NaSmallCube"))
                    {
                        childObj.gameObject.transform.SetParent(yuanDiShaoPingObj.transform);
                        break;
                    }
                }
                if (hitInfo.collider.name.Equals("Stopper"))
                {
                    
                    //PlayAnimationRevert(state_4_obj);
                    PlayAnimationRevertQueued(state_4_obj);
                    UIManager.UpdateInfo(new string[] { "塞上瓶塞", "将广口瓶放回原处" });
                    state++;
                }
                break;
            case 5:
                if (hitInfo.collider.name.Equals("xiKouPingC8H10"))
                {
                    xikoupingObj = hitInfo.collider.gameObject;
                    PlayAnimation(state_5_obj);
                    UIManager.UpdateInfo("移动 装有二甲苯的细口瓶");
                    state++;
                }
                break;
            case 6:
                if (hitInfo.collider.name.Equals("StopperC8H10"))
                {
                    stoperTmp = hitInfo.collider.gameObject;
                    PlayAnimation(state_6_obj);
                    UIManager.UpdateInfo("打开细口瓶瓶塞");
                    state++;
                }
                break;
            case 7:
                if (hitInfo.collider.name.Equals("xiKouPingC8H10"))
                {
                    stoperTmp.transform.SetParent(null);
                    PlayAnimation(state_7_obj, "7xiKouPingC8H10");
                    Invoke("state_7_delay_fun", state_7_obj["7xiKouPingC8H10"].length + 0.2f);
                    UIManager.UpdateInfo("倒入二甲苯");
                    state++;
                }
                break;
            case 8:
                if (hitInfo.collider.name.Equals("StopperC8H10"))
                {
                    stoperTmp.transform.SetParent(xikoupingObj.transform);
                    PlayAnimationRevertQueued(state_8_obj);
                    UIManager.UpdateInfo(new string[] { "塞上瓶塞", "将细口瓶放回原处" });
                    state++;
                }
                break;
            case 9:
                if (hitInfo.collider.name.Equals("qiuXingLengNingGuan"))
                {
                    qiuXingLengNingGuan = hitInfo.collider.gameObject;
                    PlayAnimation(state_9_obj);
                    Invoke("state_9_delay_fun", state_9_obj[0][state_9_obj[0].clip.name].length + 0.1f);
                    UIManager.UpdateInfo("安装球形冷凝管");
                    state++;
                }
                break;
            case 10:
                if (hitInfo.collider.name.Equals("ganZaoGuan"))
                {
                    ganZaoGuan = hitInfo.collider.gameObject;
                    PlayAnimation(state_10_obj);
                    Invoke("state_10_delay_fun", state_9_obj[0][state_9_obj[0].clip.name].length + 0.1f);
                    UIManager.UpdateInfo("安装干燥管");
                    state++;
                }
                break;
            case 11:
                if (hitInfo.collider.name.Equals("dianReTao"))
                {
                    //state_11_dianReTao
                    PlayAnimation(state_11_dianReTao, state_11_dianReTao.clip.name);
                    PlayAnimation(yuanDiShaoPingObj.GetComponent<Animation>(), "11yuanDiShaoPing");
                    UIManager.UpdateInfo(new string[] { "使用电热套加热", "第一部分完成" });
                }
                break;
        }
    }

    void state_7_delay_fun()
    {
        state_7_waterC8H10.SetActive(true);
        PlayAnimationRevert(state_7_obj, "7xiKouPingC8H10");
    }

    void state_9_delay_fun()
    {
        qiuXingLengNingGuan.transform.SetParent(yuanDiShaoPingObj.transform);
    }

    void state_10_delay_fun()
    {
        ganZaoGuan.transform.SetParent(yuanDiShaoPingObj.transform);
    }

    /// <summary>
    /// 倒放动画，由于要修改speed为-1，内部实现不可使用PlayQueued，只能使用Play，使用时要注意不要冲突。
    /// </summary>
    /// <param name="anim"></param>
    void PlayAnimationRevert(Animation[] anim)
    {
        for (int i = 0; i < anim.Length; i++)
        {
            string name = anim[i].clip.name;
            anim[i][name].time = anim[i][name].length;
            anim[i][name].speed = -1;
            anim[i].Play(name);  //倒放默认动画
        }
    }

    void PlayAnimationRevert(Animation anim, string animName)
    {
        anim[animName].time = anim[animName].length;
        anim[animName].speed = -1;
        anim.Play(animName);
    }

    private int revertIndex;
    private Animation[] revertAnim;
    /// <summary>
    /// 顺序倒放
    /// </summary>
    /// <param name="anim"></param>
    void PlayAnimationRevertQueued(Animation[] anim)
    {
        revertIndex = 0;
        revertAnim = anim;
        _PlayAnimationRevertQueued();
    }
    
    void _PlayAnimationRevertQueued()
    {
        if (revertIndex > revertAnim.Length - 1) 
            return;
        string name = revertAnim[revertIndex].clip.name;
        print(name);
        revertAnim[revertIndex][name].time = revertAnim[revertIndex][name].length;
        revertAnim[revertIndex][name].speed = -1;
        revertAnim[revertIndex].Play(name);  //倒放默认动画
        revertIndex++;
        Invoke("_PlayAnimationRevertQueued", revertAnim[revertIndex-1][name].length);
    }

    /// <summary>
    /// 利用传入的对象动画属性数组进行播放
    /// </summary>
    void PlayAnimation(Animation[] anim)
    {
        for (int i = 0; i < anim.Length; i++) 
            anim[i].PlayQueued(anim[i].clip.name);  //播放默认动画
    }

    /// <summary>
    /// 利用传入的对象动画属性数组和动画名称数组一一对应进行播放
    /// </summary>
    void PlayAnimation(Animation[] anim, string[] animName)
    {
        if (anim.Length != animName.Length)
            Debug.Log("对象动画属性数组 和 动画名称数组 长度不一");
        for (int i = 0; i < anim.Length; i++)
            anim[i].PlayQueued(animName[i]);        //播放指定动画
    }

    ///// <summary>
    ///// 播放默认动画clip
    ///// </summary>
    ///// <param name="obj"></param>
    //void PlayAnimation(GameObject obj)
    //{
    //    Animation anim = obj.GetComponent<Animation>();
    //    if (anim != null)
    //        anim.PlayQueued(anim.clip.name);
    //}

    /// <summary>
    /// 手动指定播放动画的名称
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="animName"></param>
    void PlayAnimation(Animation anim, string animName)
    {
        if (anim != null)
            anim.PlayQueued(animName);
    }
}

//------------------------------------------------------------------------
#if false

手写动画太麻烦
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lab_step_1 : MonoBehaviour
{
    public float moveDuration;
    public iTween.EaseType easeType;

    public Transform[] state_2_roadPoints;
    public GameObject state_2_obj;
    public Transform[] state_4_roadPoints;
    public GameObject state_4_obj;


    private Lab_UIManager UIManager;
    private int state = 1;
    private int animIndex = 0;
    private Transform[] roadPoints = null;
    private GameObject moveObj = null;

    void Start()
    {
        HideRoadPoints();
        UIManager = GetComponent<Lab_UIManager>();
        UIManager.UpdateInfo("实验开始，请开始你的操作", 3f);
        Invoke("ShowTipDelay", 3f);
    }

    void HideRoadPoints()
    {
        for (int i = 0; i < state_2_roadPoints.Length; i++)
            state_2_roadPoints[i].gameObject.SetActive(false);
        for (int i = 0; i < state_4_roadPoints.Length; i++)
            state_4_roadPoints[i].gameObject.SetActive(false);

    }

    void ShowTipDelay()
    {
        UIManager.UpdateInfo("按E可显示/隐藏本菜单", 3f);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))    //点击鼠标左键时，利用射线检测Equipment层的物体
        {
            RaycastHit hitInfo;
            Vector2 screenMidPoint = new Vector2(Screen.width / 2, Screen.height / 2);
            LayerMask mask = 1 << (LayerMask.NameToLayer("Equipment"));//layer为枚举
            if (Physics.Raycast(Camera.main.ScreenPointToRay(screenMidPoint), out hitInfo, 32, mask.value))
            {
                Debug.Log(hitInfo.collider.name);
                //UIManager.UpdateInfo(hitInfo.collider.name);
                ChangeState(hitInfo);
            }
        }
    }

    void ChangeState(RaycastHit hitInfo)
    {
        switch (state)
        {
            case 1:
                if (hitInfo.collider.name.Equals("yuanDiShaoPing"))
                {
                    UIManager.UpdateInfo("选中 圆底烧瓶");
                    state = 2;
                }
                break;
            case 2:
                if (hitInfo.collider.name.Equals("tableMat"))
                {
                    UIManager.UpdateInfo("移动 圆底烧瓶");
                    state = 3;
                    state_2_animation();
                }
                break;
            case 3:
                if (hitInfo.collider.name.Equals("guangKouPingNa"))
                {
                    UIManager.UpdateInfo("选中 装有Na的广口瓶");
                    state = 4;
                }
                break;
            case 4:
                if (hitInfo.collider.name.Equals("tableMat"))
                {
                    UIManager.UpdateInfo("移动 广口瓶");
                    state = 5;
                    state_4_animation();
                }
                break;
        }
    }

    void state_2_animation()
    {
        roadPoints = state_2_roadPoints;    //复制路点数组引用
        moveObj = state_2_obj;              //复制待移动的物体的引用
        animIndex = 0;                      //重置动画播放状态
        PlayAnimationMove();                //播放动画
    }

    void state_4_animation()
    {
        roadPoints = state_4_roadPoints;
        moveObj = state_4_obj;
        animIndex = 0;
        PlayAnimationMove();
    }

    /// <summary>
    /// 用于播放多动作动画，使用前需要初始化 路点数组、移动的物体、动画播放状态
    /// </summary>
    void PlayAnimationMove()                    
    {
        if (animIndex + 1 > roadPoints.Length)
            return;
        iTween.MoveTo(moveObj, iTween.Hash(
            "position", roadPoints[animIndex].position,
            "time", moveDuration,
            "easetype", easeType
            ));
        animIndex++;
        Invoke("PlayAnimationMove", moveDuration);
    }
}

#endif