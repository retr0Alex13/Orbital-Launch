using PrimeTween;
using UnityEngine;

public class FuelWindow : MonoBehaviour
{
    [SerializeField] private RectTransform fillTransform;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Visibility")]
    [SerializeField] private float visibilityTweenDuration = 0.2f;
    [SerializeField] private float hideDelay = 0.6f;
    [SerializeField] private Ease visibilityTweenEase = Ease.OutQuad;

    [Header("Fill Animation")]
    [SerializeField] private float fillTweenDuration = 0.25f;
    [SerializeField] private Ease fillTweenEase = Ease.OutQuad;

    [Header("Insufficient Fuel Punch")]
    [SerializeField] private float punchScale = 1.15f;
    [SerializeField] private float punchDuration = 0.12f;

    private bool isVisibleTarget;
    private Tween fillTween;
    private Sequence punchSequence;
    private Sequence visibilitySequence;

    public void SetTransparencyImmediate(float alpha)
    {
        visibilitySequence.Stop();
        canvasGroup.alpha = alpha;
        isVisibleTarget = alpha > 0f;
    }

    public void SetVisible(bool visible)
    {
        if (isVisibleTarget == visible && visibilitySequence.isAlive)
            return;

        isVisibleTarget = visible;
        visibilitySequence.Stop();

        if (visible)
        {
            visibilitySequence = Sequence.Create(
                Tween.Alpha(canvasGroup, endValue: 1f, duration: visibilityTweenDuration, ease: visibilityTweenEase));
        }
        else
        {
            visibilitySequence = Sequence.Create(Tween.Delay(hideDelay))
                .Chain(Tween.Alpha(canvasGroup, endValue: 0f, duration: visibilityTweenDuration, ease: visibilityTweenEase));
        }
    }

    public void SetFillImmediate(float amount)
    {
        fillTween.Stop();

        Vector3 targetScale = fillTransform.localScale;
        targetScale.x = Mathf.Clamp01(amount);
        fillTransform.localScale = targetScale;
    }

    public void SetFillAmount(float amount)
    {
        float clamped = Mathf.Clamp01(amount);

        if (Mathf.Approximately(fillTransform.localScale.x, clamped))
            return;

        fillTween.Stop();

        Vector3 targetScale = fillTransform.localScale;
        targetScale.x = clamped;

        fillTween = Tween.Scale(fillTransform, endValue: targetScale, duration: fillTweenDuration, ease: fillTweenEase);
    }

    public void PlayInsufficientFuelEffect()
    {
        punchSequence.Stop();

        Vector3 baseScale = Vector3.one;
        Vector3 punchedScale = baseScale * punchScale;

        punchSequence = Sequence.Create(Tween.Scale(transform, endValue: punchedScale, duration: punchDuration, ease: Ease.OutQuad))
            .Chain(Tween.Scale(transform, endValue: baseScale, duration: punchDuration, ease: Ease.InQuad));
    }
}