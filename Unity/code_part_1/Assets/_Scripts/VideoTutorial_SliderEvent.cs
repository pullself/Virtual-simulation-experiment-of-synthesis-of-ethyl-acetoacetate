using UnityEngine;
using UnityEngine.EventSystems;

public class VideoTutorial_SliderEvent : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public VideoTutorial_SceneManager sceneManager;

    public void OnBeginDrag(PointerEventData eventData)
    {
        sceneManager.PlayPause();           //暂停
    }

    public void OnDrag(PointerEventData eventData)
    {
        ChangePlaybackTime();               //更改播放进度
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        sceneManager.PlayPause();           //继续播放
    }

    /// <summary>
    /// 根据视频进度条的值按比例更改视频播放进度
    /// </summary>
    private void ChangePlaybackTime()
    {
        sceneManager.videoPlayer.time = sceneManager.videoTimeSlider.value * sceneManager.videoPlayer.length;
        sceneManager.Playing();
    }
}
