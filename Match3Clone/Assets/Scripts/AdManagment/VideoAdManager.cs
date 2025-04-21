using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Video;
using UnityEngine.UI;
using Unity.Services.RemoteConfig;
using Newtonsoft.Json;
using System.Linq;
using TMPro;
using UnityEngine.Networking;
using System.IO;
using UnityEngine.SceneManagement;


public class VideoAdManager : MonoBehaviour
{
    [Header("Required Components")]
    [SerializeField] private List<VideoAdComponent> waitingAds = new List<VideoAdComponent>();
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private RawImage vertexImage;
    [SerializeField] private Button closedButton;
    [SerializeField] private Button linkButton;
    [SerializeField] private TextMeshProUGUI closeTimeText;
    [SerializeField] private Slider progressBar;
    private Queue<VideoAd> videoAds = new Queue<VideoAd>();
    private VideoAd video;
    private VideoAdComponent videoAdComponent = null;
    private DateTime timeToClose;
    private AudioSource backGroundMusicSource;

    private int timeToShowInSecond;
    private void OnEnable()
    {
        Debug.Log("VideoAdManager started.");

        closedButton.interactable = false;
        vertexImage.gameObject.SetActive(false);

        videoPlayer.loopPointReached += OnVideoEnd;
        videoPlayer.prepareCompleted += OnVideoPrepared;

        closedButton.onClick.AddListener(ClosedClicked);
        linkButton.onClick.AddListener(LinkButtonClicked);

        backGroundMusicSource = FindAnyObjectByType<AudioSource>().GetComponent<AudioSource>();

        ApplyRemoteConfig(ConfigRequestStatus.Success);
        if(PlayerPrefs.GetInt("IsFirstTimeVideoAd",1) == 1)
        {
            Debug.Log("Not First time video ad.");

            PlayerPrefs.SetInt("IsFirstTimeVideoAd", 0);
            PlayerPrefs.SetInt("TimeToShowInSecond", 10);
            PlayerPrefs.Save();
            
            timeToShowInSecond = PlayerPrefs.GetInt("TimeToShowInSecond");
            StartCoroutine(CheckTimer());
        }
        else if(PlayerPrefs.GetInt("IsFirstTimeVideoAd") == 2)
        {
            Debug.Log("Not First time video ad.");
            timeToShowInSecond = PlayerPrefs.GetInt("TimeToShowInSecond");
            StartCoroutine(CheckTimer());
        }
    }
    private void Update() 
    {
        if(SceneManager.GetActiveScene().name != "LevelScene")
        {
            return;
        }
        if(timeToClose < DateTime.Now)
        {
            closeTimeText.text = "00:00";
            closedButton.interactable = true;
            progressBar.value = 0f;
        }
        else
        {
            closedButton.interactable = false;
            closeTimeText.text = TimeSpan.FromSeconds((timeToClose - DateTime.Now).TotalSeconds).ToString(@"mm\:ss");
            progressBar.value = (float)(timeToClose - DateTime.Now).TotalSeconds / 5f;
        }
        if(waitingAds.Count > 0 || videoAdComponent != null)
        {
            if( waitingAds.Count > 0 && videoAdComponent == null)
            {
                Debug.Log("Video Ad is null");
                videoAdComponent = waitingAds.FirstOrDefault();
            }
            if(videoAdComponent != null && waitingAds.Contains(videoAdComponent))
            {
                waitingAds.Remove(videoAdComponent);
            }
            if(DateTime.Now.ToString() == videoAdComponent.TimeToShow)
            {
                Debug.Log("Ad Show");
                videoAds.Enqueue(videoAdComponent.videoAd);
                videoAdComponent = null;
                StartCoroutine(PlayVideo());
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

    private IEnumerator CheckTimer()
    {
        Debug.Log("Checking timer for video ad.");
        Debug.Log("Time to show in second: " + timeToShowInSecond);
        while(timeToShowInSecond > 0)
        {
            timeToShowInSecond--;
            Debug.Log("Time to show in second: " + timeToShowInSecond);
            PlayerPrefs.SetInt("TimeToShowInSecond", timeToShowInSecond);
            PlayerPrefs.SetInt("IsFirstTimeVideoAd", 2);
            PlayerPrefs.Save();
            yield return new WaitForSeconds(1f);
        }
        if(timeToShowInSecond <= 0)
        {
            Debug.Log("Time to show video ad is up.");
            timeToShowInSecond = 0;
            PlayerPrefs.SetInt("TimeToShowInSecond", timeToShowInSecond);
            PlayerPrefs.SetInt("IsFirstTimeVideoAd", 3);
            PlayerPrefs.Save();
            StartCoroutine(PlayVideo());
        }
        yield return null;
    }


    private void ClosedClicked()
    {
        if(timeToClose < DateTime.Now)
        {
            videoPlayer.Stop();
            vertexImage.gameObject.SetActive(false);
            videoPlayer.clip = null;
            backGroundMusicSource.mute = false;
            AdCoordinator.Instance.NotifyAdEnded(); // Notify end
            StartCoroutine(PlayVideo());
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

        DeleteVideoFile(); // delete video file after playing

        if (videoAds.Count > 0)
        {
            StartCoroutine(PlayVideo()); // attempt next ad if possible
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
        }
    }

    private IEnumerator PlayVideo()
    {
        PlayerPrefs.SetInt("IsFirstTimeVideoAd", 4);
        PlayerPrefs.Save();
        while(!AdCoordinator.Instance.CanShowAd())
        {
            Debug.Log("Another ad is showing. Delaying video ad.");
            yield return new WaitForSeconds(1f);
        }
        if (videoAds.Count > 0)
        {
            video = videoAds.Dequeue();

            waitingAds.Add(new VideoAdComponent
            {
                videoAd = video,
                TimeToShow = DateTime.Now.AddMinutes(video.Duration).ToString(),
            });

            AdCoordinator.Instance.NotifyAdStarted(); // Notify start
            
            // Download video
            using UnityWebRequest request = UnityWebRequest.Get(video.Url);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error downloading video: " + request.error);
                yield break;
            }

            Debug.Log("Video downloaded successfully.");
            byte[] videoData = request.downloadHandler.data;
            string videoPath = Path.Combine(Application.persistentDataPath, "video.mp4");

            File.WriteAllBytes(videoPath, videoData);

            videoPlayer.prepareCompleted += OnVideoPrepared;
            videoPlayer.loopPointReached += OnVideoEnd;

            videoPlayer.url = videoPath;
            Debug.Log("Video URL: " + video.Url);
            vertexImage.gameObject.SetActive(true);
            videoPlayer.Prepare();
            videoPlayer.prepareCompleted += OnVideoPrepared;
        }
        else
        {
            Debug.Log("No more ads to show.");
            vertexImage.gameObject.SetActive(false);
        }
    }

    private void DeleteVideoFile()
    {
        string videoPath = Path.Combine(Application.persistentDataPath, "video.mp4");
        if (File.Exists(videoPath))
        {
            File.Delete(videoPath);
            Debug.Log("Video file deleted: " + videoPath);
        }
        else
        {
            Debug.LogError("Video file not found: " + videoPath);
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