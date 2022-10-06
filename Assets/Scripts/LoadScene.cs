using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadScene : MonoBehaviour
{
    public float duration;
    private Image img;

    private void Awake()
    {
        img = GetComponent<Image>();
    }

    private IEnumerator Start()
    {
        yield return StartCoroutine(Fade());
        StartCoroutine(LoadSceneAsync());
    }

    IEnumerator LoadSceneAsync()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("MainScene");

        while(!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    IEnumerator Fade()
    {
        float time = 0;
        Color color = img.color;
        while (time < duration)
        {
            float t = time / duration;
            color.a = Mathf.Lerp(0, 1, t);
            img.color = color;
            time += Time.deltaTime;
            yield return null;
        }
    }

}
