using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

[RequireComponent(typeof(VideoPlayer))]
public class VideoFromURL : MonoBehaviour
{
    private VideoPlayer videoPlayer;
    public string URL;
    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.url = URL;
    }
}
