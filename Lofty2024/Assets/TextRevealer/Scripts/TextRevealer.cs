using System.Collections.Generic;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[ExecuteInEditMode]
public class TextRevealer : MonoBehaviour {

    public MonoBehaviour textComponent;
    public bool AutomaticUnreveal = false;
    public bool RevealOnEnabled = false;
    public bool UnrevealOnDisabled = false;
    public float AutomaticUnrevealAfterSeconds = 1f;

    public float UnrevealDelay = 0f;

    public float revealMinHorizontalDistance = 0f;
    public float revealMaxHorizontalDistance = 50f;
    public float revealMinVerticalDistance = 0f;
    public float revealMaxVerticalDistance = 50f;
    
    public float unrevealMinHorizontalDistance = 0f;
    public float unrevealMaxHorizontalDistance = 50f;
    public float unrevealMinVerticalDistance = 0f;
    public float unrevealMaxVerticalDistance = 50f;

    public float RevealTime = 3f;
    public float UnrevealTime = 3f;

    public float RevealLetterOpacityBegin = 0f;
    public float UnrevealLetterOpacityBegin = 1f;

    public float RevealLetterOpacityEnd = 1f;
    public float UnrevealLetterOpacityEnd = 0f;

    public bool RevealSmoothOverlap = true;
    public bool UnrevealSmoothOverlap = true;
    
    public float RevealSmoothOverlapSmoothness = 0.125f;
    public float UnrevealSmoothOverlapSmoothness = 0.125f;

    public bool RevealRandomRotation = false;
    public bool UnrevealRandomRotation = false;

    public bool RevealReverse = false;
    public bool UnrevealReverse = false;

    public AnimationDirections RevealAnimationDirection = AnimationDirections.Both;
    public AnimationHorizontalDirections RevealAnimationHorizontalDirection = AnimationHorizontalDirections.Randomize;
    public AnimationVerticalDirections RevealAnimationVerticalDirection = AnimationVerticalDirections.Randomize;

    public AnimationDirections UnrevealAnimationDirection = AnimationDirections.Both;
    public AnimationHorizontalDirections UnrevealAnimationHorizontalDirection = AnimationHorizontalDirections.Randomize;
    public AnimationVerticalDirections UnrevealAnimationVerticalDirection = AnimationVerticalDirections.Randomize;

    public bool IsEditorPlayingAnimation = false;

    public UnityEvent OnRevealStarted;
    public UnityEvent OnRevealCompleted;
    public UnityEvent OnUnrevealStarted;
    public UnityEvent OnUnrevealCompleted;

    private Animation m_animationReveal;
    private Animation m_animationUnreveal;
    private const string AnimationRevealClipName = "textAnimationReveal";
    private const string AnimationUnrevealClipName = "textAnimationUnreveal";
    private const string LocalPositionXFieldName = "m_LocalPosition.x";
    private const string LocalPositionYFieldName = "m_LocalPosition.y";
    private const string LocalScaleXFieldName = "m_LocalScale.x";
    private const string LocalScaleYFieldName = "m_LocalScale.y";

    private GameObject slicedParent;
    private AnimationClip unrevealClip;
    private AnimationClip revealClip;
    private AnimationClip editorClip;
    private Animation anim;
    private TextComponentHandler textComponentHandler;

    void Start() {

        UpdateTextComponent(true);

        if (!IsInEditorMode())
        {
            GetTextComponentHandler().TurnOffTextAlpha();
        }
    }

    private void OnDisable()
    {
        if (!IsInEditorMode() && UnrevealOnDisabled)
        {
            Unreveal();
        }
    }    
    
    private void OnEnable()
    {
        if (!IsInEditorMode() && RevealOnEnabled)
        {
            Reveal();
        }
    }

    public void UpdateTextComponent(bool init)
    {
        TMP_Text tmpText = GetComponent<TMP_Text>();
        Text simpleText = GetComponent<Text>();
        if (tmpText != null)
        {
            textComponent = tmpText;
        }
        else if (simpleText != null)
        {
            textComponent = simpleText;
        }

        if (init)
        {
            GetTextComponentHandler();
        }
        else
        {
            UpdateTextComponentHandler();
        }
    }

    public void RevealVoid()
    {
        Reveal();
    }

    public void UnrevealVoid()
    {
        Unreveal();
    }

    public GameObject Reveal()
    {
        CancelInvokes();
        OnRevealStarted.Invoke();
        TurnOffTextAlpha();
        GameObject slicedGameObject = PrepareForAnimation();

        if (!IsInEditorMode())
        {
            PlayRevealAnimation();
        }

#if UNITY_EDITOR
        if (IsInEditorMode())
        {
            if (!IsEditorPlayingAnimation)
            {
                IsEditorPlayingAnimation = true;
                editorClipTime = 0;
                editorClip = revealClip;

                if (!AnimationMode.InAnimationMode())
                {
                    AnimationMode.StartAnimationMode();
                }

                EditorApplication.update += EditorUpdate;
            }
        }
#endif

        if (!IsInEditorMode() && AutomaticUnreveal)
        {
            Invoke("Unreveal", RevealTime + AutomaticUnrevealAfterSeconds);
        }

        return this.slicedParent = slicedGameObject;
    }

    public GameObject Unreveal()
    {
        CancelInvokes();
        OnUnrevealStarted.Invoke();
        TurnOffTextAlpha();
        GameObject slicedGameObject = PrepareForAnimation();

        if (!IsInEditorMode())
        {
            PlayUnrevealAnimation();
        }

#if UNITY_EDITOR
        if (IsInEditorMode())
        {
            if (!IsEditorPlayingAnimation)
            {
                IsEditorPlayingAnimation = true;
                editorClipTime = 0;
                editorClip = unrevealClip;

                if (!AnimationMode.InAnimationMode())
                {
                    AnimationMode.StartAnimationMode();
                }

                EditorApplication.update += EditorUpdate;
            }
        }
#endif

        return this.slicedParent = slicedGameObject;
    }

    public void StopAnimation()
    {
        if (anim != null && anim.isPlaying)
        {
            anim.Stop();
        }

#if UNITY_EDITOR
        if (IsInEditorMode() && IsEditorPlayingAnimation)
        {
            if (AnimationMode.InAnimationMode())
            {
                AnimationMode.StopAnimationMode();
            }

            EditorApplication.update -= EditorUpdate;
            editorClipTime = 0;
            editorClip = null;

            RevertTextAlpha();
            IsEditorPlayingAnimation = false;
            GameObject.DestroyImmediate(slicedParent);
        }
#endif
    }

    private void TurnOffTextAlpha()
    {
        GetTextComponentHandler().TurnOffTextAlpha();
    }
    
    private void RevertTextAlpha()
    {
        GetTextComponentHandler().RevertTextAlpha();
    }

    GameObject PrepareForAnimation()
    {
        if (this.slicedParent != null && !IsInEditorMode())
        {
            GameObject.Destroy(this.slicedParent);
        }

        slicedParent = new GameObject(textComponent.gameObject.name + "_sliced");
        slicedParent.AddComponent<RectTransform>();
        slicedParent.transform.SetParent(textComponent.transform.parent);
        slicedParent.transform.localScale = textComponent.transform.localScale;

        RectTransform rectTransformNew = slicedParent.GetComponent<RectTransform>();
        RectTransform rectTransformOld = textComponent.GetComponent<RectTransform>();

        Canvas.ForceUpdateCanvases();

        CopyTransformFields(rectTransformNew, rectTransformOld);

        List<Transform> slices = CreateSlices(slicedParent.transform);

        m_animationReveal = CreateRevealAnimation(slicedParent, slices);
        m_animationUnreveal = CreateUnrevealAnimation(slicedParent, slices);

        return slicedParent;
    }

    Animation CreateRevealAnimation(GameObject slicedParent, List<Transform> slices)
    {
        anim = slicedParent.GetComponent<Animation>();
        if (anim == null)
        {
            anim = slicedParent.AddComponent<Animation>();
        }

        revealClip = new AnimationClip
        {
            name = AnimationRevealClipName,
            legacy = true
        };

        float revealTimePerSlice = 0;

        if (RevealSmoothOverlap)
        {
            revealTimePerSlice = RevealTime * RevealSmoothOverlapSmoothness;
        }
        else
        {
            revealTimePerSlice = RevealTime / (float)slices.Count;
        }

        int i = RevealReverse ? slices.Count - 1 : 0;
        int iEnd = RevealReverse ? -1 : slices.Count;
        while (i != iEnd)
        {
            Transform slice = slices[i];

            float startTimeForSlice = 0;

            if (RevealSmoothOverlap)
            {
                startTimeForSlice = ((float)(RevealReverse ? slices.Count - i - 1 : i) / (float)(slices.Count - 1)) * (RevealTime - revealTimePerSlice);
            }
            else
            {
                startTimeForSlice = (float)(RevealReverse ? slices.Count - i - 1 : i) * revealTimePerSlice;
            }

            Vector2 distance = Vector2.zero; 

            if (RevealAnimationDirection == AnimationDirections.Both || RevealAnimationDirection == AnimationDirections.Horizontal)
            {
                float randomHorizontalDistance = Random.Range(revealMinHorizontalDistance, revealMaxHorizontalDistance);

                if (RevealAnimationHorizontalDirection == AnimationHorizontalDirections.Randomize)
                {
                    distance.x = randomHorizontalDistance * RandomSign();
                }
                else if (RevealAnimationHorizontalDirection == AnimationHorizontalDirections.LeftToRightOnly)
                {
                    distance.x = -randomHorizontalDistance;
                }
                else if (RevealAnimationHorizontalDirection == AnimationHorizontalDirections.RightToLeftOnly)
                {
                    distance.x = randomHorizontalDistance;
                }
            }

            if (RevealAnimationDirection == AnimationDirections.Both || RevealAnimationDirection == AnimationDirections.Vertical)
            {
                float randomVerticalDistance = Random.Range(revealMinVerticalDistance, revealMaxVerticalDistance);

                if (RevealAnimationVerticalDirection == AnimationVerticalDirections.Randomize)
                {
                    distance.y = randomVerticalDistance * RandomSign();
                }
                else if (RevealAnimationVerticalDirection == AnimationVerticalDirections.TopToBottomOnly)
                {
                    distance.y = randomVerticalDistance;
                }
                else if (RevealAnimationVerticalDirection == AnimationVerticalDirections.BottomToTopOnly)
                {
                    distance.y = -randomVerticalDistance;
                }
            }

            if (RevealAnimationDirection == AnimationDirections.Horizontal)
            {
                Keyframe[] keysY;
                keysY = new Keyframe[1];
                keysY[0] = new Keyframe(startTimeForSlice, slice.localPosition.y);

                AnimationCurve curveY = new AnimationCurve(keysY);
                revealClip.SetCurve(slice.name, typeof(Transform), LocalPositionYFieldName, curveY);
            }

            if (RevealAnimationDirection == AnimationDirections.Vertical)
            {
                Keyframe[] keysX;
                keysX = new Keyframe[1];
                keysX[0] = new Keyframe(startTimeForSlice, slice.localPosition.x);

                AnimationCurve curveX = new AnimationCurve(keysX);
                revealClip.SetCurve(slice.name, typeof(Transform), LocalPositionXFieldName, curveX);
            }

            Vector2 startPosition = new Vector2(
                slice.localPosition.x + distance.x,
                slice.localPosition.y + distance.y);

            float returnKeyTime = startTimeForSlice + revealTimePerSlice;

            GenerateRandomAnimation(startTimeForSlice, returnKeyTime, startPosition, slice, revealClip, true, RevealAnimationDirection);

            if (RevealRandomRotation)
            {
                AnimationCurve rotationCurveX = AnimationCurve.Linear(startTimeForSlice, Random.Range(-1f, 0.5f), returnKeyTime, 1f);
                AnimationCurve rotationCurveY = AnimationCurve.Linear(startTimeForSlice, Random.Range(-1f, 0.5f), returnKeyTime, 1f);
                revealClip.SetCurve(slice.name, typeof(Transform), LocalScaleXFieldName, rotationCurveX);
                revealClip.SetCurve(slice.name, typeof(Transform), LocalScaleYFieldName, rotationCurveY);
            }

            AnimationCurve curveColor = AnimationCurve.Linear(startTimeForSlice, RevealLetterOpacityBegin, returnKeyTime, RevealLetterOpacityEnd);
            GetTextComponentHandler().SetColorCurve(revealClip, slice.name, curveColor);

            if (RevealReverse)
            {
                i--;
            }
            else
            {
                i++;
            }
        }

        anim.AddClip(revealClip, revealClip.name);
        return anim;
    }

    Animation CreateUnrevealAnimation(GameObject slicedParent, List<Transform> slices)
    {
        Animation animation = slicedParent.GetComponent<Animation>();
        if (animation == null)
        {
            animation = slicedParent.AddComponent<Animation>();
        }

        unrevealClip = new AnimationClip
        {
            name = AnimationUnrevealClipName,
            legacy = true
        };

        float unrevealTimePerSlice = 0;

        if (UnrevealSmoothOverlap)
        {
            unrevealTimePerSlice = UnrevealTime * UnrevealSmoothOverlapSmoothness;
        }
        else
        {
            unrevealTimePerSlice = UnrevealTime / (float)slices.Count;
        }

        int i = UnrevealReverse ? slices.Count - 1 : 0;
        int iEnd = UnrevealReverse ? -1 : slices.Count;
        while (i != iEnd)
        {
            Transform slice = slices[i];

            float startTimeForSlice = 0;

            if (UnrevealSmoothOverlap)
            {
                startTimeForSlice = ((float)(UnrevealReverse ? slices.Count - i - 1 : i) / (float)(slices.Count - 1)) * (UnrevealTime - unrevealTimePerSlice);
            }
            else
            {
                startTimeForSlice = (float)(UnrevealReverse ? slices.Count - i - 1 : i) * unrevealTimePerSlice;
            }

            Vector2 distance = Vector2.zero;

            if (UnrevealAnimationDirection == AnimationDirections.Both || UnrevealAnimationDirection == AnimationDirections.Horizontal)
            {
                float randomHorizontalDistance = Random.Range(unrevealMinHorizontalDistance, unrevealMaxHorizontalDistance);

                if (UnrevealAnimationHorizontalDirection == AnimationHorizontalDirections.Randomize)
                {
                    distance.x = randomHorizontalDistance * RandomSign();
                }
                else if (UnrevealAnimationHorizontalDirection == AnimationHorizontalDirections.LeftToRightOnly)
                {
                    distance.x = randomHorizontalDistance;
                }
                else if (UnrevealAnimationHorizontalDirection == AnimationHorizontalDirections.RightToLeftOnly)
                {
                    distance.x = -randomHorizontalDistance;
                }
            }

            if (UnrevealAnimationDirection == AnimationDirections.Both || UnrevealAnimationDirection == AnimationDirections.Vertical)
            {
                float randomVerticalDistance = Random.Range(unrevealMinVerticalDistance, unrevealMaxVerticalDistance);

                if (UnrevealAnimationVerticalDirection == AnimationVerticalDirections.Randomize)
                {
                    distance.y = randomVerticalDistance * RandomSign();
                }
                else if (UnrevealAnimationVerticalDirection == AnimationVerticalDirections.TopToBottomOnly)
                {
                    distance.y = -randomVerticalDistance;
                }
                else if (UnrevealAnimationVerticalDirection == AnimationVerticalDirections.BottomToTopOnly)
                {
                    distance.y = randomVerticalDistance;
                }
            }

            if (UnrevealAnimationDirection == AnimationDirections.Horizontal)
            {
                Keyframe[] keysY;
                keysY = new Keyframe[1];
                keysY[0] = new Keyframe(startTimeForSlice, slice.localPosition.y);

                AnimationCurve curveY = new AnimationCurve(keysY);
                unrevealClip.SetCurve(slice.name, typeof(Transform), LocalPositionYFieldName, curveY);
            }

            if (UnrevealAnimationDirection == AnimationDirections.Vertical)
            {
                Keyframe[] keysX;
                keysX = new Keyframe[1];
                keysX[0] = new Keyframe(startTimeForSlice, slice.localPosition.x);

                AnimationCurve curveX = new AnimationCurve(keysX);
                unrevealClip.SetCurve(slice.name, typeof(Transform), LocalPositionXFieldName, curveX);
            }

            Vector2 startPosition = new Vector2(
                slice.localPosition.x + distance.x,
                slice.localPosition.y + distance.y);

            float returnKeyTime = startTimeForSlice + unrevealTimePerSlice;

            GenerateRandomAnimation(startTimeForSlice, returnKeyTime, startPosition, slice, unrevealClip, false, UnrevealAnimationDirection);

            if (UnrevealRandomRotation)
            {
                AnimationCurve rotationCurveX = AnimationCurve.Linear(startTimeForSlice, 1f, returnKeyTime, Random.Range(-1f, 0.5f));
                AnimationCurve rotationCurveY = AnimationCurve.Linear(startTimeForSlice, 1f, returnKeyTime, Random.Range(-1f, 0.5f));
                unrevealClip.SetCurve(slice.name, typeof(Transform), LocalScaleXFieldName, rotationCurveX);
                unrevealClip.SetCurve(slice.name, typeof(Transform), LocalScaleYFieldName, rotationCurveY);
            }

            AnimationCurve curveColor = AnimationCurve.Linear(startTimeForSlice, UnrevealLetterOpacityBegin, returnKeyTime, UnrevealLetterOpacityEnd);

            GetTextComponentHandler().SetColorCurve(unrevealClip, slice.name, curveColor);

            if (UnrevealReverse)
            {
                i--;
            }
            else
            {
                i++;
            }
        }

        animation.AddClip(unrevealClip, unrevealClip.name);

        return animation;
    }

    private void GenerateRandomXAnimation(float startTimeForSlice, float returnKeyTime, float startPosition, Transform slice, AnimationClip clip, bool reveal)
    {
        Keyframe[] keysX;
        keysX = new Keyframe[2];

        keysX[0] = new Keyframe(startTimeForSlice, reveal ? startPosition : slice.localPosition.x);
        keysX[1] = new Keyframe(returnKeyTime, reveal ? slice.localPosition.x : startPosition);

        AnimationCurve curveX = new AnimationCurve(keysX);
        clip.SetCurve(slice.name, typeof(Transform), LocalPositionXFieldName, curveX);
    }

    private void GenerateRandomYAnimation(float startTimeForSlice, float returnKeyTime, float startPosition, Transform slice, AnimationClip clip, bool reveal)
    {
        Keyframe[] keysY;
        keysY = new Keyframe[2];

        keysY[0] = new Keyframe(startTimeForSlice, reveal ? startPosition : slice.localPosition.y);
        keysY[1] = new Keyframe(returnKeyTime, reveal ? slice.localPosition.y : startPosition);

        AnimationCurve curveY = new AnimationCurve(keysY);
        clip.SetCurve(slice.name, typeof(Transform), LocalPositionYFieldName, curveY);
    }

    List<Transform> CreateSlices(Transform slicedParent)
    {
        return GetTextComponentHandler().CreateSlices(slicedParent);
    }

#if UNITY_EDITOR

    float editorClipTime = 0;

    private void EditorUpdate()
    {
        if (slicedParent == null || editorClip == null || !IsInEditorMode())
        {
            return;
        }

        if (editorClipTime >= editorClip.length)
        {
            StopAnimation();
            return;
        }

        AnimationMode.BeginSampling();
        AnimationMode.SampleAnimationClip(slicedParent, editorClip, editorClipTime);
        AnimationMode.EndSampling();

        editorClipTime += Time.deltaTime;
    }
#endif

    private void CopyTransformFields(RectTransform rectTransformNew, RectTransform rectTransformOld)
    {
        rectTransformNew.position = rectTransformOld.position;
        rectTransformNew.localScale = rectTransformOld.localScale;
        rectTransformNew.sizeDelta = rectTransformOld.sizeDelta;
        rectTransformNew.anchoredPosition = rectTransformOld.anchoredPosition;
        rectTransformNew.anchorMax = rectTransformOld.anchorMax;
        rectTransformNew.anchorMin = rectTransformOld.anchorMin;
        rectTransformNew.rotation = rectTransformOld.rotation;
    }

    private void PlayRevealAnimation()
    {
        m_animationReveal.Play(AnimationRevealClipName);
        Invoke("RevealCompleted", RevealTime);
    }

    private void PlayUnrevealAnimation()
    {
        m_animationUnreveal.Play(AnimationUnrevealClipName);
        Invoke("UnrevealCompleted", UnrevealTime);
    }

    private void CancelInvokes()
    {
        CancelInvoke("RevealCompleted");
        CancelInvoke("UnrevealCompleted");
    }

    private void RevealCompleted()
    {
        OnRevealCompleted.Invoke();
    }

    private void UnrevealCompleted()
    {
        OnUnrevealCompleted.Invoke();
    }

    private void GenerateRandomAnimation(float startTimeForSlice, float returnKeyTime, Vector2 startPosition, Transform slice, AnimationClip clip, bool reveal, AnimationDirections animationDirection)
    {
        if (animationDirection == AnimationDirections.Both || animationDirection == AnimationDirections.Horizontal)
        {
            GenerateRandomXAnimation(startTimeForSlice, returnKeyTime, startPosition.x, slice, clip, reveal);
        }

        if (animationDirection == AnimationDirections.Both || animationDirection == AnimationDirections.Vertical)
        {
            GenerateRandomYAnimation(startTimeForSlice, returnKeyTime, startPosition.y, slice, clip, reveal);
        }
    }


    private bool IsInEditorMode()
    {
        return Application.isEditor
#if UNITY_EDITOR 
            && !EditorApplication.isPlaying;
#else
        ;
#endif
    }


    private TextComponentHandler GetTextComponentHandler()
    {
        if (textComponentHandler == null)
        {
            return UpdateTextComponentHandler();
        }

        return textComponentHandler;
    }

    private TextComponentHandler UpdateTextComponentHandler()
    {
        if (textComponent is Text)
        {
            textComponentHandler = new SimpleTextComponentHandler((Text)textComponent);
        }
        else if (textComponent is TMP_Text)
        {
            textComponentHandler = new TMPTextComponentHandler((TMP_Text)textComponent);
        }

        return textComponentHandler;
    }

    int RandomSign()
    {
        return Random.value < .5f ? 1 : -1;
    }
}
