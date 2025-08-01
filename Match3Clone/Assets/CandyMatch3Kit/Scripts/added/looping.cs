using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Threading.Tasks;

public class LoopingScroll : MonoBehaviour
{
    [Header("Scroll Configuration")]
    public RectTransform content;
    public GameObject page;
    
    [Header("Threshold Settings")]
    public float MaxThreshold = 7000f;
    public float resetAfterMax = -7800f;
    public float MinThreshold = -7000f;
    public float resetAfterMin = 7800f;
    
    [Header("Animation Settings")]
    public float transitionDelay = 1f;
    public string transitionAnimationName = "transition";
    
    [Header("Level Settings")]
    public int levelsPerLoop = 200;
    
    // Public properties for debugging
    public int CurrentLoop => loop;
    public int OriginLoop => originLoop;
    public bool IsResetting => isResetting;
    
    // Private fields
    private Animator trans;
    private int loop;
    private int originLoop;
    private GameObject target;
    private bool newLoop = false;
    private bool isResetting = false;
    private PlayerProfile playerProfile;
    private bool isInitialized = false;
    
    // Cache for better performance
    private const string TARGET_NAME = "LevelMapAvatar(Clone)";
    private const int PROFILE_LOAD_RETRY_DELAY = 100;
    private const int MAX_PROFILE_LOAD_ATTEMPTS = 100; // 10 seconds max wait

    private async void Start()
    {
        await InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        try
        {
            // Initialize animator
            if (page != null)
            {
                trans = page.GetComponent<Animator>();
                if (trans == null)
                {
                    Debug.LogError($"Animator component not found on {page.name}");
                    return;
                }
            }
            else
            {
                Debug.LogError("Page GameObject is not assigned");
                return;
            }

            // Load player profile with timeout
            await LoadPlayerProfileAsync();
            
            // Calculate initial loop values
            if (playerProfile != null)
            {
                CalculateLoopValues();
                FindAndCacheTarget();
                isInitialized = true;
                Debug.Log($"LoopingScroll initialized - Level: {playerProfile.Level}, Loop: {loop}");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error initializing LoopingScroll: {e.Message}");
        }
    }

    private async Task LoadPlayerProfileAsync()
    {
        int attempts = 0;
        
        while (playerProfile == null && attempts < MAX_PROFILE_LOAD_ATTEMPTS)
        {
            try
            {
                playerProfile = await SaveData.LocalSaveManager.Instance.LoadDataAsync<PlayerProfile>("PlayerProfile");
                
                if (playerProfile != null)
                {
                    break;
                }
                
                attempts++;
                await Task.Delay(PROFILE_LOAD_RETRY_DELAY);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error loading player profile (attempt {attempts + 1}): {e.Message}");
                attempts++;
                await Task.Delay(PROFILE_LOAD_RETRY_DELAY);
            }
        }

        if (playerProfile == null)
        {
            Debug.LogError($"Failed to load player profile after {MAX_PROFILE_LOAD_ATTEMPTS} attempts");
        }
    }

    private void CalculateLoopValues()
    {
        if (playerProfile != null)
        {
            var nextLevel = playerProfile.Level;
            loop = nextLevel / levelsPerLoop;
            originLoop = loop;
        }
    }

    private void FindAndCacheTarget()
    {
        if (target == null)
        {
            target = GameObject.Find(TARGET_NAME);
            if (target == null)
            {
                Debug.LogWarning($"Target GameObject '{TARGET_NAME}' not found");
            }
        }
    }

    void Update()
    {
        if (!isInitialized || isResetting || content == null)
            return;

        // Cache target finding to avoid frequent GameObject.Find calls
        FindAndCacheTarget();

        CheckScrollThresholds();
    }

    private void CheckScrollThresholds()
    {
        float currentY = content.anchoredPosition.y;

        if (currentY > MaxThreshold && loop > 0)
        {
            TriggerScrollReset(resetAfterMax, -1);
        }
        else if (currentY < MinThreshold)
        {
            TriggerScrollReset(resetAfterMin, 1);
        }
    }

    private void TriggerScrollReset(float newYPosition, int loopDelta)
    {
        if (trans != null)
        {
            trans.Play(transitionAnimationName);
        }
        
        StartCoroutine(ResetScrollAfterDelay(newYPosition, loopDelta));
    }

    private IEnumerator ResetScrollAfterDelay(float newYPosition, int loopDelta)
    {
        isResetting = true;

        yield return new WaitForSeconds(transitionDelay);

        // Update scroll position
        if (content != null)
        {
            Vector2 pos = content.anchoredPosition;
            pos.y = newYPosition;
            content.anchoredPosition = pos;
        }

        // Update loop values
        loop += loopDelta;
        newLoop = true;

        // Update target visibility
        UpdateTargetVisibility();

        isResetting = false;
    }

    private void UpdateTargetVisibility()
    {
        if (target == null) return;

        bool shouldBeActive = (originLoop == loop);
        
        if (target.activeSelf != shouldBeActive)
        {
            target.SetActive(shouldBeActive);
            Debug.Log($"Target visibility updated - Active: {shouldBeActive}, Current Loop: {loop}, Origin Loop: {originLoop}");
        }
    }

    // Public methods for external control
    public void SetLoop(int newLoop)
    {
        loop = newLoop;
        UpdateTargetVisibility();
    }

    public void ResetToOriginLoop()
    {
        loop = originLoop;
        UpdateTargetVisibility();
    }

    // Validation in editor
    private void OnValidate()
    {
        if (MinThreshold > 0)
        {
            Debug.LogWarning("MinThreshold should typically be negative for proper scrolling behavior");
        }
        
        if (levelsPerLoop <= 0)
        {
            levelsPerLoop = 200;
        }
        
        if (transitionDelay < 0)
        {
            transitionDelay = 1f;
        }
    }

    private void OnDestroy()
    {
        // Clean up any running coroutines
        StopAllCoroutines();
    }
}
