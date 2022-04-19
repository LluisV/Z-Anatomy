using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class OpenHelpItem : MonoBehaviour
{
    public ContentSizeFitter contentFitter;
    public VideoPlayer videoPlayer;
    public GameObject videoImage;
    public Image imageToChange;
    public Sprite openSprite;
    public Sprite closeSprite;
    public string videoName;
    private RectTransform rect;
    [HideInInspector]
    public bool open;

    private void Start()
    {
        rect = GetComponent<RectTransform>();
        videoPlayer.url = System.IO.Path.Combine(Application.streamingAssetsPath, videoName);
    }

    public void OnOpenCloseClick()
    {
        contentFitter.enabled = false;
        if (!open)
        {
            open = true;
            videoPlayer.Play();
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, 1600);
            imageToChange.sprite = openSprite;
        }
        else
        {
            open = false;
            videoPlayer.Stop();
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, 100);
            imageToChange.sprite = closeSprite;
        }
        videoPlayer.gameObject.SetActive(open);
        videoImage.SetActive(open);
        contentFitter.enabled = true;
    }

    public void Close()
    {
        contentFitter.enabled = false;
        videoPlayer.gameObject.SetActive(false);
        videoImage.SetActive(false);
        videoPlayer.Stop();
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, 100);
        imageToChange.sprite = closeSprite;
        contentFitter.enabled = true;
    }
}
