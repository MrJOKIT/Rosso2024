using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ArtifactUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TextMeshProUGUI artifactName;
    public Image artifactImage;
    public Sprite defaultImage;
    public GameObject hoverObject;
    public TextMeshProUGUI cardDetail;

    public void SetArtifactUI(string name, Sprite sprite, string detail)
    {
        artifactName.text = name ?? "Unknown Artifact";
        artifactImage.sprite = sprite ?? defaultImage;
        cardDetail.text = detail ?? string.Empty;
    }

    public void ClearArtifactSlot()
    {
        artifactName.text = "Empty";
        artifactImage.sprite = defaultImage;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverObject) hoverObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (hoverObject) hoverObject.SetActive(false);
    }
}