using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeOut : MonoBehaviour
{
    public float duration;
    private Image img;

    private void Awake()
    {
        img = GetComponent<Image>();
        #if !UNITY_EDITOR
            img.enabled = true;
        #endif
    }

    private void Start()
    {
        StartCoroutine(Fade());
    }

    IEnumerator Fade()
    {
        yield return new WaitForSeconds(.5f);
        float time = 0;
        Color color = img.color;
        while(time < duration)
        {
            float t = time / duration;
            color.a = Mathf.Lerp(1, 0, t);
            img.color = color;
            time += Time.deltaTime;
            yield return null;
        }
        img.gameObject.SetActive(false);
    }
}
