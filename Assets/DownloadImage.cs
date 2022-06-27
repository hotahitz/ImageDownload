using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class DownloadImage : MonoBehaviour
{
    public static DownloadImage Instance;

    [SerializeField]
    List<ImageRequestArgs> requestQueue;
    [SerializeField]
    int processingRequests=0;

    private void Awake()
    {
        Instance = this;
        requestQueue = new List<ImageRequestArgs>();
    }

    private void Update()
    {
        CheckDownloadQueue();
    }

    void CheckDownloadQueue()
    {
        if (requestQueue.Count == 0)
            return;

        if(processingRequests < 3)
        {
            StartCoroutine(GetImage(requestQueue[0]));
            requestQueue.RemoveAt(0);
            processingRequests++;

            if(processingRequests < 3)
            {
                CheckDownloadQueue();
            }
        }
    }

    public void AddToQueue(ImageRequestArgs requestArgs)
    {
        requestQueue.Add(requestArgs);
    }

    IEnumerator GetImage(ImageRequestArgs requestArgs)
    {
        Debug.Log(requestArgs.URL);

        UnityWebRequest downloadedImage = UnityWebRequestTexture.GetTexture(requestArgs.URL);

        yield return downloadedImage.SendWebRequest();

        Texture2D imageTex = null;

        if (downloadedImage.result == UnityWebRequest.Result.ConnectionError ||
           downloadedImage.result == UnityWebRequest.Result.DataProcessingError ||
           downloadedImage.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log("There is Some Error");
        }
        else
        {
            imageTex = DownloadHandlerTexture.GetContent(downloadedImage);
            requestArgs.TargetImage.sprite = Sprite.Create(imageTex, new Rect(0f, 0f, imageTex.width, imageTex.height), Vector2.zero);
        }

        if (requestArgs.toBeCached)
        {
            yield return StartCoroutine(SaveImageToDisk(requestArgs.TargetImage.name, imageTex));
        }

        processingRequests--;
        CheckDownloadQueue();
    }

    IEnumerator SaveImageToDisk(string name, Texture2D tex)
    {
        if(tex != null)
        {
            File.WriteAllBytes(Path.Combine(Application.persistentDataPath, name), tex.EncodeToPNG());
        }
        
        yield return null;
    }

    public IEnumerator ApplySpriteToImage(Image image, string path)
    {
        image.sprite = LoadSprite(path);
        yield return null;
    }

    public Sprite LoadSprite(string path)
    {
        if (string.IsNullOrEmpty(path)) return null;
        if (System.IO.File.Exists(path))
        {
            byte[] bytes = System.IO.File.ReadAllBytes(path);
            Texture2D texture = new Texture2D(1, 1);
            texture.LoadImage(bytes);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            return sprite;
        }
        return null;
    }
}

[System.Serializable]
public class ImageRequestArgs
{
    public string URL;
    public Image TargetImage;
    public bool toBeCached;

    public ImageRequestArgs(string url, Image image, bool cache)
    {
        URL = url;
        TargetImage = image;
        toBeCached = cache;
    }
}