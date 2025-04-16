using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Video;
using UnityEngine.UI;
using Unity.Services.Core;
using static RemotelyDownloadAssets;
using Unity.Services.RemoteConfig;
using Newtonsoft.Json;
using System.Linq;
using TMPro;


public class VideoAdManager : MonoBehaviour
{
    private Queue<VideoAd> videoAds = new Queue<VideoAd>();
    [SerializeField] private static List<VideoAdComponent> waitingAds = new List<VideoAdComponent>();
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private RawImage vertexImage;
    [SerializeField] private Button closedButton;
    [SerializeField] private Button linkButton;
    [SerializeField] private TextMeshProUGUI closeTimeText;

    private VideoAd video;
    private static VideoAdComponent videoAdComponent = null;
    private DateTime timeToClose;
    private AudioSource backGroundMusicSource;

    private async void Start()
    {
        await UnityServices.InitializeAsync();
        await RemoteConfigService.Instance.FetchConfigsAsync(new UserAttributes(), new AppAttributes());

        videoPlayer.loopPointReached += OnVideoEnd;
        videoPlayer.prepareCompleted += OnVideoPrepared;

        closedButton.onClick.AddListener(ClosedClicked);
        linkButton.onClick.AddListener(LinkButtonClicked);

        closedButton.interactable = false;
        vertexImage.gameObject.SetActive(false);

        backGroundMusicSource = FindAnyObjectByType<AudioSource>().GetComponent<AudioSource>();

        if(PlayerPrefs.GetInt("IsFirstTimeVideoAd", 1) == 1)
        {
            Debug.Log("First time video ad.");
            ApplyRemoteConfig(ConfigRequestStatus.Success);
        }
        else
        {
            Debug.Log("Not First time airship ad.");
        }
    }
    private void Update() 
    {
        if(timeToClose < DateTime.Now)
        {
            closeTimeText.text = "00:00";
            closedButton.interactable = true;
        }
        else
        {
            closedButton.interactable = false;
            closeTimeText.text = TimeSpan.FromSeconds((timeToClose - DateTime.Now).TotalSeconds).ToString(@"mm\:ss");
        }
        if(waitingAds.Count > 0 || videoAdComponent != null)
        {
            if( waitingAds.Count > 0 && string.IsNullOrEmpty(videoAdComponent.TimeToShow))
            {
                Debug.Log("Video Ad is null");
                videoAdComponent = waitingAds.FirstOrDefault();
            }
            if(videoAdComponent != null && waitingAds.Contains(videoAdComponent))
            {
                
                waitingAds.Remove(videoAdComponent);
            }
            Debug.Log(" Video Current Time: " + DateTime.Now.ToString() + " TimeToShow: " + videoAdComponent.TimeToShow + "true or false: " + (DateTime.Now.ToString() == videoAdComponent.TimeToShow));
            if(DateTime.Now.ToString() == videoAdComponent.TimeToShow)
            {
                Debug.Log("Ad Show");
                videoAds.Enqueue(videoAdComponent.videoAd);
                videoAdComponent = null;
                PlayVideo();
            }
        }   
    }
    private void LinkButtonClicked()
    {
        if(video != null)
        {
            Application.OpenURL(video.LinkUrl);
        }
        else
        {
            Debug.LogError("Video is null.");
        }
    }

    private void ClosedClicked()
    {
        if(timeToClose < DateTime.Now)
        {
            videoPlayer.Stop();
            vertexImage.gameObject.SetActive(false);
            videoPlayer.clip = null;
            AdCoordinator.Instance.NotifyAdEnded(); // Notify end
            PlayVideo();
        }
        else
        {
            Debug.Log("Video is still playing.");
        }
    }

    private void OnVideoPrepared(VideoPlayer source)
    {
        videoPlayer.prepareCompleted -= OnVideoPrepared;
        videoPlayer.Play();
        backGroundMusicSource.mute = true;
        closedButton.interactable = false;
        timeToClose = DateTime.Now.AddSeconds(5);
    }

    private void OnVideoEnd(VideoPlayer source)
    {
        Debug.Log("Video ended.");
        AdCoordinator.Instance.NotifyAdEnded(); // Notify end

        videoPlayer.Stop();
        backGroundMusicSource.mute = false;
        vertexImage.gameObject.SetActive(false);
        videoPlayer.clip = null;

        if (videoAds.Count > 0)
        {
            PlayVideo(); // attempt next ad if possible
        }
    }

    private void ApplyRemoteConfig(ConfigRequestStatus success)
    {
        if (success == ConfigRequestStatus.Success)
        {
            string json = RemoteConfigService.Instance.appConfig.GetJson("VideoAds");

            if(string.IsNullOrEmpty(json))
            {
                Debug.LogError("VideoAdConfig is empty or null.");
                return;
            }

            List<VideoAd> videoAdComponents = JsonConvert.DeserializeObject<List<VideoAd>>(json);
            Debug.Log("VideoAdConfig: " + json);
            foreach(var videoAdComponent in videoAdComponents)
            {
                videoAds.Enqueue(videoAdComponent);
            }
            PlayerPrefs.SetInt("IsFirstTimeVideoAd", 0);
            PlayerPrefs.Save();

            PlayVideo();
        }
    }

    private void PlayVideo()
    {
         if (!AdCoordinator.Instance.CanShowAd())
        {
            Debug.Log("Another ad is showing. Delaying video ad.");
            return;
        }
        if (videoAds.Count > 0)
        {
            video = videoAds.Dequeue();

            waitingAds.Add(new VideoAdComponent
            {
                videoAd = video,
                TimeToShow = DateTime.Now.AddMinutes(video.Duration).ToString(),
            });

            videoPlayer.url = video.Url;
            vertexImage.gameObject.SetActive(true);
            AdCoordinator.Instance.NotifyAdStarted(); // Notify start
            videoPlayer.Prepare();
            videoPlayer.prepareCompleted += OnVideoPrepared;
        }
        else
        {
            Debug.Log("No more ads to show.");
            vertexImage.gameObject.SetActive(false);
        }
    }

    private void OnApplicationQuit() 
    {
        vertexImage.gameObject.SetActive(false);
        videoPlayer.Stop();
        videoPlayer.clip = null;
        videoPlayer.url = null;
        videoAdComponent = null;
        video = null;
        videoAds.Clear();
        waitingAds.Clear();
        PlayerPrefs.DeleteKey("IsFirstTimeVideoAd");
        PlayerPrefs.Save();
    }
}
[Serializable]
public class VideoAdComponent
{
    public VideoAd videoAd;
    public string TimeToShow;
}

[Serializable]
public class VideoAd : Ad
{
    public string LinkUrl;
    public int Duration;
}