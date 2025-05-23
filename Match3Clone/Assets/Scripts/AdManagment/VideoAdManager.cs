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
using System.Threading.Tasks;
using UnityEngine.Purchasing.MiniJSON;


public class VideoAdManager : MonoBehaviour
{
    [Header("Required Components")]
    [SerializeField] private Queue<VideoAdComponent> waitingAds = new();
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private RawImage vertexImage;
    [SerializeField] private Button closedButton;
    [SerializeField] private Button linkButton;
    [SerializeField] private TextMeshProUGUI closeTimeText;
    [SerializeField] private Slider progressBar;
    [SerializeField] private GameObject notificationPrefab;
    private Queue<VideoAd> videoAds = new Queue<VideoAd>();
    private VideoAd video;
    private List<VideoAd> videoAdComponents = new List<VideoAd>();
    private VideoAdComponent videoAdComponent;
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

        if(File.Exists(Path.Combine(Application.persistentDataPath,"waitingVideo.json")))
        {
            var jsonFile = File.ReadAllText(Path.Combine(Application.persistentDataPath,"waitingVideo.json"));
            waitingAds = JsonConvert.DeserializeObject<Queue<VideoAdComponent>>(jsonFile);
            Debug.Log("Waiting ads loaded from file.");
            Debug.Log("Waiting ads count: " + waitingAds.Count);
            Debug.Log("Json file: " + jsonFile);
            foreach(var waitingAd in waitingAds)
            {
                videoAds.Enqueue(waitingAd.videoAd);

            }
            Debug.Log("Video ads count: " + videoAds.Count);

            Debug.Log("Video file exists.");
            StartCoroutine(PlayAdAgain());
        }
        else
        {
            Debug.Log("Video file does not exist.");
            ApplyRemoteConfig(ConfigRequestStatus.Success);
        }
    }
    private void Update() 
    {
        if(videoAdComponent != null && waitingAds.Count > 0 && timeToClose < DateTime.Now)
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

            videoAdComponents = JsonConvert.DeserializeObject<List<VideoAd>>(json);
            Debug.Log("VideoAdConfig: " + json);
            StartCoroutine(CheckTimer());
        }
    }
    private IEnumerator PlayAdAgain()
    {
        Debug.Log("Playing ad Again.");
        
        DateTime timeToShow = DateTime.Parse(waitingAds.Peek().TimeToShow);
        double timeToShowAgain = timeToShow.Subtract(DateTime.Now).TotalSeconds;
        Debug.Log("Time to show again: " + timeToShowAgain);
        while(timeToShowAgain > 0)
        {
            timeToShowAgain--;
            Debug.Log("Time to show again: " + timeToShowAgain);
            yield return new WaitForSeconds(1f);
        }
        StartCoroutine(PlayVideo());
    }

    private IEnumerator CheckTimer()
    {
        Debug.Log("Checking timer for video ad.");
        timeToShowInSecond = PlayerPrefs.GetInt("TimeToShowInSecond");

        
        Debug.Log("Time to show in second: " + timeToShowInSecond);
        while(timeToShowInSecond > 0)
        {
            timeToShowInSecond--;
            Debug.Log("Time to show in second: " + timeToShowInSecond);
            PlayerPrefs.SetInt("TimeToShowInSecond", timeToShowInSecond);
            yield return new WaitForSeconds(1f);
        }
        if(timeToShowInSecond <= 0)
        {
            Debug.Log("Time to show video ad is up.");
            timeToShowInSecond = 0;
            PlayerPrefs.SetInt("TimeToShowInSecond", timeToShowInSecond);
            PlayerPrefs.Save();

            Debug.Log("No more ads to show.");
            foreach(var videoAdComponent in videoAdComponents)
            {
                videoAds.Enqueue(videoAdComponent);
                waitingAds.Enqueue(new VideoAdComponent
                {
                    videoAd = videoAdComponent,
                    TimeToShow = DateTime.Now.AddMinutes(videoAdComponent.Duration).ToString(),
                });
            }
            
            StartCoroutine(PlayVideo());
        }
        yield return null;
    }


    private IEnumerator PlayVideo()
    {
        while(!AdCoordinator.Instance.CanShowAd())
        {
            Debug.Log("Another ad is showing. Delaying video ad.");
            yield return new WaitForSeconds(1f);
        }
        var videoAd = waitingAds.Peek();
        var timeToShow = DateTime.Parse(videoAd.TimeToShow);
        Debug.Log("Time to show video ad: " + timeToShow.ToString());
        var timeToShowInSecond = timeToShow.Subtract(DateTime.Now).TotalSeconds;
        while((timeToShow > DateTime.Now) && AdCoordinator.Instance.CanShowAd() && timeToShowInSecond != 5 && PlayerPrefs.GetInt("IsFirstTimeVideoAd") != 0)
        {
            Debug.Log("Video ad is not ready to show yet.");
            yield return new WaitForSeconds(1f);
        }
        notificationPrefab.SetActive(true);
        while(AdCoordinator.Instance.CanShowAd() && timeToShowInSecond <= 0  && PlayerPrefs.GetInt("IsFirstTimeVideoAd") != 0)
        {
            yield return new WaitForSeconds(1f);
        }
       
        if (videoAds.Count > 0)
        {
             var fiveSeconds = 5;
            Debug.Log("PlayerPrefs: " + PlayerPrefs.GetInt("IsFirstTimeVideoAd"));
            Debug.Log("Five seconds: " + fiveSeconds);
            while(PlayerPrefs.GetInt("IsFirstTimeVideoAd") == 0 && fiveSeconds >= 0)
            {
                Debug.Log("Video ad is not ready to show yet.");
                fiveSeconds--;
                yield return new WaitForSeconds(1f);
            }
            notificationPrefab.SetActive(false);

            video = videoAds.Dequeue();
            videoAdComponent = waitingAds.Dequeue();

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

            videoPlayer.url = videoPath;
            Debug.Log("Video URL: " + video.Url);
            vertexImage.gameObject.SetActive(true);
            videoPlayer.Prepare();
            videoPlayer.prepareCompleted += OnVideoPrepared;
        }
        else
        {
            File.WriteAllTextAsync(Path.Combine(Application.persistentDataPath,"waitingVideo.json"), JsonConvert.SerializeObject(waitingAds));
            PlayerPrefs.SetInt("IsFirstTimeVideoAd", 1);
            PlayerPrefs.Save();
            vertexImage.gameObject.SetActive(false);
        }
    }
    private void OnVideoPrepared(VideoPlayer source)
    {
        videoPlayer.prepareCompleted -= OnVideoPrepared;
        videoPlayer.Play();
        if(PlayerPrefs.GetInt("sound_enabled") == 1)
        {
            backGroundMusicSource.mute = true;
        }
        closedButton.interactable = false;
        timeToClose = DateTime.Now.AddSeconds(5);
    }

    private void OnVideoEnd(VideoPlayer source)
    {
        Debug.Log("Video ended.");

        AdCoordinator.Instance.NotifyAdEnded(); // Notify end

        videoPlayer.Stop();
        if(PlayerPrefs.GetInt("sound_enabled") == 1)
        {
            backGroundMusicSource.mute = false;
        }
        vertexImage.gameObject.SetActive(false);
        videoPlayer.clip = null;

        videoAdComponent.TimeToShow = DateTime.Now.AddMinutes(video.Duration).ToString(); // Set the time to show the ad again
        waitingAds.Enqueue(videoAdComponent); // add video ad to waiting ads
        File.WriteAllTextAsync(Path.Combine(Application.persistentDataPath,"waitingVideo.json"), JsonConvert.SerializeObject(waitingAds)); // save waiting ads to file
        videoAdComponent = null;

        DeleteVideoFile(); // delete video file after playing

        if (videoAds.Count > 0)
        {
            StartCoroutine(PlayVideo()); // attempt next ad if possible
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
            videoAdComponent.TimeToShow = DateTime.Now.AddMinutes(video.Duration).ToString(); // Set the time to show the ad again
            waitingAds.Enqueue(videoAdComponent); // add video ad to waiting ads
            File.WriteAllTextAsync(Path.Combine(Application.persistentDataPath,"waitingVideo.json"), JsonConvert.SerializeObject(waitingAds)); // save waiting ads to file
            if(PlayerPrefs.GetInt("sound_enabled")==1)
            {
                backGroundMusicSource.mute = false;
            }
            AdCoordinator.Instance.NotifyAdEnded(); // Notify end
            StartCoroutine(PlayVideo());
            notificationPrefab.SetActive(false);
        }
        else
        {
            Debug.Log("Video is still playing.");
        }
    }
    private void OnApplicationQuit() 
    {
        vertexImage.gameObject.SetActive(false);
        videoPlayer.Stop();
        videoPlayer.clip = null;
        videoPlayer.url = null;
        video = null;
        videoAds.Clear();
        waitingAds.Clear();
        File.Delete(Path.Combine(Application.persistentDataPath,"waitingVideo.json"));
    }
    private void OnDisable() 
    {
        closedButton.onClick.RemoveListener(ClosedClicked);
        linkButton.onClick.RemoveListener(LinkButtonClicked);
        videoPlayer.loopPointReached -= OnVideoEnd;
        videoPlayer.prepareCompleted -= OnVideoPrepared;
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