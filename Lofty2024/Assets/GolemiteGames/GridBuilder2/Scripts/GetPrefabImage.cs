using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/*************Basic script for demo purposes to place the prefab image on the SelectObjects buttons**************/
public class GetPrefabImage : MonoBehaviour
{
    public GameObject objectPrefab;
    Image image;
    Texture2D thumbnail;
    // Start is called before the first frame update
#if UNITY_EDITOR
    void Start()
    {
        image = GetComponent<Image>();
        StartCoroutine(SetThumbnail());
    }
    IEnumerator SetThumbnail()
    {
        thumbnail = null;
        while (thumbnail == null)
        {
            thumbnail = UnityEditor.AssetPreview.GetAssetPreview(objectPrefab);
            yield return new WaitForSeconds(.5f);
        }
        var sprite = Sprite.Create(thumbnail, new Rect(0.0f, 0.0f, thumbnail.width, thumbnail.height), new Vector2(0.5f, 0.5f), 100.0f);
        
        image.sprite = sprite;
        
    }
#endif
}
