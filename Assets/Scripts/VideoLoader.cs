using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoLoader : MonoBehaviour
{
    public string uri = "";
    public bool clearCache = false;

    private VideoPlayer videoPlayer;
    private AssetBundle bundle;

    private void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        Caching.compressionEnabled = false;

        if (clearCache)
            Caching.CleanCache();
    }

    public void PlayVideo() => StartCoroutine(DownloadAndPlay());

    private IEnumerator DownloadAndPlay()
    {
        yield return GetBundle();

        if(!bundle)
        {
            Debug.Log("Bundle failed to load");
            yield break;
        }

        VideoClip newVideoClip = bundle.LoadAsset<VideoClip>("");
        videoPlayer.clip = newVideoClip;
        videoPlayer.Play();
    }

    private IEnumerator GetBundle()
    {
        WWW request = WWW.LoadFromCacheOrDownload(uri, 0);

        while(!request.isDone)
        {
            Debug.Log(request.progress);
            yield return null;
        }

        if(request.error == null)
        {
            bundle = request.assetBundle;
            Debug.Log("Asset downloaded");
        }
        else
        {
            Debug.Log(request.error);
        }

        yield return null;
    }
}
