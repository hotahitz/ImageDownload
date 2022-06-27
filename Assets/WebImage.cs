using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

[RequireComponent(typeof(Image))]
public class WebImage : MonoBehaviour
{
    [SerializeField]
    Image imageComponent;
    [SerializeField]
    string url;
    [SerializeField]
    string localPath;
    [SerializeField]
    bool toBeCached=true;
    private void Start()
    {
        if(imageComponent == null)
        {
            imageComponent = GetComponent<Image>();
        }

        if (toBeCached)
        {
            if (File.Exists(Path.Combine(Application.persistentDataPath, gameObject.name)))
            {
                localPath = Path.Combine(Application.persistentDataPath, gameObject.name);
                StartCoroutine(DownloadImage.Instance.ApplySpriteToImage(imageComponent, localPath));
            }
            else
            {
                DownloadImage.Instance.AddToQueue(new ImageRequestArgs(url, imageComponent, toBeCached));
            }
        }
        else
        {
            DownloadImage.Instance.AddToQueue(new ImageRequestArgs(url, imageComponent, toBeCached));
        }
        
    }
}
