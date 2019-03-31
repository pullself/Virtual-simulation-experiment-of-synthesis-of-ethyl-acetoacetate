using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationUtils : MonoBehaviour
{
    [HideInInspector]
    public float currAnimDuration;
    private iTween.EaseType type = iTween.EaseType.linear;
    private float animPartDuration;
    private GameObject obj;
    private Transform[] tr;
    private int roadPointTarget;
    private int roadPointNum;
    private int animItemTarget;
    private int animItemNum;
    private OperationAnimation[] animationItem;

    public void HideRoadPoints(ExperimentOperation[] ops)
    {
        for (int i = 0; i < ops.Length; i++) 
        {
            OperationAnimation[] oa = ops[i].animationItem;
            for (int j = 0; j < oa.Length; j++) 
            {
                Transform[] tr = oa[j].roadPointTransforms;
                for (int k = 0; k < tr.Length; k++) 
                {
                    tr[k].gameObject.SetActive(false);
                }
            }
        }
    }
    public void PlayAnimations(ExperimentOperation[] ops,int state)
    {
        if (state > ops.Length - 1)
            return;
        this.animationItem = ops[state].animationItem;
        animItemNum = animationItem.Length;
        animItemTarget = 0;
        _ChooseAnimItem();
    }

    private void _ChooseAnimItem()
    {
        if (animItemTarget > animItemNum - 1)
            return;
        currAnimDuration = animationItem[animItemTarget].animDuration;
        _PlayObjAnim(animationItem[animItemTarget]);
        animItemTarget++;
        //print("currAnimDuration = " + currAnimDuration.ToString());
        Invoke("_ChooseAnimItem", currAnimDuration + 0.1f);
    }

    private void _PlayObjAnim(OperationAnimation oa)
    {
        obj = oa.obj;
        tr = oa.roadPointTransforms;
        roadPointTarget = 0;
        roadPointNum = tr.Length;
        animPartDuration = currAnimDuration / roadPointNum;
        //print("animPartDuration = " + animPartDuration.ToString());
        MoveToNextRoadPoint();
    }

    private void MoveToNextRoadPoint()
    {
        if (roadPointTarget > roadPointNum - 1)
            return;
        iTween.MoveTo(obj, iTween.Hash(
            "position",tr[roadPointTarget].position,
            "time", animPartDuration,
            "easetype", type
            ));
        iTween.RotateTo(obj, iTween.Hash(
            "rotation", tr[roadPointTarget].eulerAngles,
            "time", animPartDuration,
            "easetype", type
            ));
        roadPointTarget++;
        Invoke("MoveToNextRoadPoint", animPartDuration);
    }
}
