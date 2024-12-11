using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TMPTextComponentHandler : TextComponentHandler
{
    private const string ColorFieldName = "m_fontColor.a";
    private TMP_Text textComponent;

    public TMPTextComponentHandler(TMP_Text textComponent)
    {
        this.textComponent = textComponent;
    }

    public override List<Transform> CreateSlices(Transform slicedParent)
    {
        List<Transform> slices = new List<Transform>();
        List<CharacterInfo> characterInfos = GetCharacterInfo();

        int i = 0;
        foreach (var characterInfo in characterInfos)
        {
            if (char.IsWhiteSpace(characterInfo.TextValue) || characterInfo.TextValue == '\n')
            {
                continue;
            }

            GameObject slice = new GameObject(textComponent.gameObject.name + "_slice_" + i++.ToString());
            slice.transform.SetParent(slicedParent);

            TMP_Text sliceText = this.textComponent is TextMeshPro ? (TMP_Text) slice.AddComponent<TextMeshPro>() : (TMP_Text) slice.AddComponent<TextMeshProUGUI>();
            sliceText.text = characterInfo.TextValue.ToString();
            sliceText.alignment = TextAlignmentOptions.MidlineGeoAligned;
            slice.transform.localPosition = characterInfo.Position;
            CopyTextFields(sliceText, textComponent);

            slice.transform.localRotation = Quaternion.identity;
            slice.transform.localScale = Vector3.one;
            slices.Add(slice.transform);
        }

        return slices;
    }

    public override void TurnOffTextAlpha()
    {
        OriginalOpacity = textComponent.color.a;
        textComponent.color = GetTransparentColor();
    }

    public override void RevertTextAlpha()
    {
        Color currentColor = textComponent.color;
        textComponent.color = new Color(currentColor.r, currentColor.g, currentColor.b, OriginalOpacity);
    }

    public override void SetColorCurve(AnimationClip clip, string sliceName, AnimationCurve curveColor)
    {
        clip.SetCurve(sliceName, typeof(TMP_Text), ColorFieldName, curveColor);
    }

    private List<CharacterInfo> GetCharacterInfo()
    {
        List<CharacterInfo> indexes = new List<CharacterInfo>();
        textComponent.ForceMeshUpdate();

        for (int index = 0; index < textComponent.text.Length; index++)
        {
            if (!char.IsWhiteSpace(textComponent.textInfo.characterInfo[index].character))
            {
                Vector3 locUpperLeft = textComponent.textInfo.characterInfo[index].topLeft;
                Vector3 locBottomRight = textComponent.textInfo.characterInfo[index].bottomRight;

                Vector3 mid = new Vector3((locUpperLeft.x + locBottomRight.x) / 2.0f, (locUpperLeft.y + locBottomRight.y) / 2.0f, (locUpperLeft.z + locBottomRight.z) / 2.0f);

                indexes.Add(new CharacterInfo() { Position = mid, TextValue = textComponent.textInfo.characterInfo[index].character });
            }
        }

        return indexes;
    }

    private Color GetTransparentColor()
    {
        Color currentColor = textComponent.color;
        return new Color(currentColor.r, currentColor.g, currentColor.b, 0f);
    }

    private void CopyTextFields(TMP_Text sliceText, TMP_Text textComponent)
    {
        sliceText.font = textComponent.font;
        sliceText.fontSize = textComponent.fontSize;
        sliceText.fontStyle = textComponent.fontStyle;
        sliceText.color = textComponent.color;
        sliceText.fontSharedMaterial = textComponent.fontSharedMaterial;
        sliceText.colorGradient = textComponent.colorGradient;
        sliceText.colorGradientPreset = textComponent.colorGradientPreset;
        sliceText.enableVertexGradient = textComponent.enableVertexGradient;
        sliceText.lineSpacing = textComponent.lineSpacing;
        sliceText.material = textComponent.material;
        sliceText.overflowMode = TextOverflowModes.Overflow;
        sliceText.enableWordWrapping = true;
    }
}
