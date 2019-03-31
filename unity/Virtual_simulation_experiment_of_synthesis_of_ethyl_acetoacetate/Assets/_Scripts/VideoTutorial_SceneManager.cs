using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using UnityEngine.UI;

public class VideoTutorial_SceneManager : MonoBehaviour
{
    public GameObject videoPlayArea;            //视频播放区域
    public Text videoTimeText;                  //视频时间/视频总时长
    public Slider videoTimeSlider;              //播放进度条
    public Image playPauseIcon;                 //播放/暂停按钮
    public Sprite playSprite;                   //播放的按钮图标
    public Sprite pauseSprite;                  //暂停的图标
    
    [HideInInspector]
    public VideoPlayer videoPlayer;

    private RawImage rawImage;
    [HideInInspector]
    public bool isDraging;

    void Start()
    {
        isDraging = false;
        videoPlayer = videoPlayArea.GetComponent<VideoPlayer>();
        rawImage = videoPlayArea.GetComponent<RawImage>();
        videoPlayer.loopPointReached += VideoPlayFinished;
    }

    private void Update()
    {
        rawImage.texture = videoPlayer.texture;
        Playing();
    }

    /// <summary>
    /// 根据当前播放时间更改播放进度条和显示的时间文本
    /// </summary>
    public void Playing()
    {
        // progressBarSlider.value  进度条比例 0-1
        // videoPlayer.time         已播放视频时长/秒
        // videoPlayer.length       视频总时长/秒
        if(!isDraging)
            videoTimeSlider.value = videoPlayer.time == 0 ? 0 : (float)(videoPlayer.time / videoPlayer.length);         //更新进度条
        videoTimeText.text = TimeFormatterUtil(videoPlayer.time) + " / " + TimeFormatterUtil(videoPlayer.length);   //更新文本显示
    }

    /// <summary>
    /// 点播放/暂停按钮触发
    /// </summary>
    public void PlayPause()
    {
        if (videoPlayer.isPlaying)
        {
            videoPlayer.Pause();                //暂停视频
            playPauseIcon.sprite = playSprite;  //更改按钮图标为播放
        }
        else
        {
            videoPlayer.Play();                 //继续播放视频
            playPauseIcon.sprite = pauseSprite; //更改按钮图标为暂停
        }
    }

    /// <summary>
    /// 点重新播放按钮触发
    /// </summary>
    public void Replay()                        //重新播放
    {
        videoPlayer.Stop();                     //停止上一轮播放
        videoPlayer.Play();                     //开始下一次播放
        playPauseIcon.sprite = pauseSprite;     //更改按钮图标为暂停
    }

    /// <summary>
    /// 点返回按钮触发
    /// </summary>
    public void Back()                          //返回上级
    {
        SceneManager.LoadScene("MainMenu");
    }

    /// <summary>
    /// 视频播放完成调用
    /// </summary>
    /// <param name="vPlayer"></param>
    private void VideoPlayFinished(VideoPlayer vPlayer)
    {
        playPauseIcon.sprite = playSprite;      //视频播放完成，更改按钮图标为播放
    }


    /// <summary>
    /// 时间格式化工具，传入时间(秒)，返回格式化的时间，形如00:00:00
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    private string TimeFormatterUtil(double time)
    {
        int hour = (int)time / 3600;
        int tmp = (int)(time - hour * 3600);
        int minute = tmp / 60;
        int second = (int)(tmp - minute * 60);
        return string.Format("{0:D2}:{1:D2}:{2:D2}", hour, minute, second);
    }
}
