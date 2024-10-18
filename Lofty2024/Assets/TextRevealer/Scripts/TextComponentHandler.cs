using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TextComponentHandler
{
    public float OriginalOpacity;

    public abstract void TurnOffTextAlpha();

    public abstract void RevertTextAlpha();

    public abstract List<Transform> CreateSlices(Transform slicedParent);

    public abstract void SetColorCurve(AnimationClip clip, string sliceName, AnimationCurve curveColor);

    protected class CharacterInfo
    {
        public Vector3 Position { get; set; }
        public char TextValue { get; set; }
    }
}
