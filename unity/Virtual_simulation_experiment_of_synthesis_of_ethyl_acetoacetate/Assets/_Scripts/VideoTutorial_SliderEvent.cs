using UnityEngine;
using UnityEngine.EventSystems;

public class VideoTutorial_SliderEvent : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public VideoTutorial_SceneManager sceneManager;

    public void OnBeginDrag(PointerEventData eventData)
    {
        sceneManager.isDraging = true;
        sceneManager.PlayPause();           //暂停
    }

    public void OnDrag(PointerEventData eventData)
    {
        sceneManager.videoPlayer.time = sceneManager.videoTimeSlider.value * sceneManager.videoPlayer.length;
                                            //更改播放进度
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        sceneManager.isDraging = false;
        sceneManager.PlayPause();           //继续播放
    }
}
