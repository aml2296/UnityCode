using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField]
    GameObject loadingProgressImage;

    public void LoadScene(string sceneName)
    {
        StartCoroutine(UpdateLoader(sceneName));
    }
    public IEnumerator UpdateLoader(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        loadingProgressImage.transform.parent.gameObject.SetActive(true);

        while (!operation.isDone)
        {
            loadingProgressImage.transform.localScale = new Vector3(Mathf.Clamp01(operation.progress / 0.9f), 1, 1);
            yield return null;
        }
        loadingProgressImage.transform.parent.gameObject.SetActive(false);
    }
}
