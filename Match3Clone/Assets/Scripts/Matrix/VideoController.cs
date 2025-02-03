using UnityEngine;
using UnityEngine.Video;

public class VideoController : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public float displayDuration = 3.0f; // Duration to display the video
    private void Start()
    {
        videoPlayer.loopPointReached += OnVideoEnd;
        PlayVideo();
    }

    private void PlayVideo()
    {
        videoPlayer.Play();
        Invoke("HideVideo", displayDuration);
    }

    private void HideVideo()
    {
        videoPlayer.Stop();
        gameObject.SetActive(false);
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        HideVideo();
    }
}
