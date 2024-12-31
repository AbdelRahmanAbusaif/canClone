using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScene : MonoBehaviour
{
    public GameObject LoadingScreen;
    public void LoadSceneByIndex(int sceneIndex)
    {
        StartCoroutine(LoadSceneAsync(sceneIndex));
    }
    IEnumerator LoadSceneAsync(int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);


        while (!operation.isDone)
        {
            // Here will be the code for loading the scene asynchronously
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            Debug.Log("Loading progress: " + progress * 100 + "%");

            LoadingScreen.SetActive(true);

            yield return null;
        }

        LoadingScreen.SetActive(false);
        yield return null;
    }
}