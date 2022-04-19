using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoFromStreamingAssets : MonoBehaviour
{
    private VideoPlayer videoPlayer;
    public string videoName;

    private void Awake()
    {
        if (!videoName.EndsWith(".mp4"))
            videoName += ".mp4";
        videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.url = System.IO.Path.Combine(Application.streamingAssetsPath, videoName);
    }

    void Start()
    {
        videoPlayer.Play();
    }

}
