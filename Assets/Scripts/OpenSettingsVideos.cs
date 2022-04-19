using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class OpenSettingsVideos : MonoBehaviour
{
    public ContentSizeFitter contentFitter;
    public string videoNameOff;
    public string videoNameOn;
    public VideoPlayer videoPlayerOff;
    public VideoPlayer videoPlayerOn;
    public GameObject videosContainer;
    public Image imageToChange;
    public Sprite openSprite;
    public Sprite closeSprite;
    private RectTransform rect;
    private float originalY;
    private bool open;

    private void Start()
    {
        rect = GetComponent<RectTransform>();
        videoPlayerOff.url = System.IO.Path.Combine(Application.streamingAssetsPath, videoNameOff);
        videoPlayerOn.url = System.IO.Path.Combine(Application.streamingAssetsPath, videoNameOn);

    }

    public void OnOpenCloseClick()
    {
        open = !open;
        contentFitter.enabled = false;
        videoPlayerOff.gameObject.SetActive(open);
        videoPlayerOn.gameObject.SetActive(open);
        videosContainer.SetActive(open);
        if (open)
        {
            originalY = rect.sizeDelta.y;
            videoPlayerOff.Play();
            videoPlayerOn.Play();
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, 500);
            imageToChange.sprite = openSprite;
        }
        else
        {
            videoPlayerOff.Stop();
            videoPlayerOn.Stop();
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, originalY);
            imageToChange.sprite = closeSprite;
        }
        contentFitter.enabled = true;
    }
}
