using System;
using PrimeTween;
using UnityEngine;

public class SlideInFromBottom : MonoBehaviour
{
    [SerializeField] private RectTransform[] items;
    [SerializeField] private float showDuration = 0.5f;
    [SerializeField] private float hideDuration = 0.3f;
    [SerializeField] private float stagger = 0.08f;
    [SerializeField] private Ease showEase = Ease.OutBack;
    [SerializeField] private Ease hideEase = Ease.InBack;

    private Vector2[] shownPositions;
    private bool isHiding;

    private void Awake()
    {
        shownPositions = new Vector2[items.Length];
        for (int i = 0; i < items.Length; i++)
            shownPositions[i] = items[i].anchoredPosition;
    }

    private void OnEnable()
    {
        isHiding = false;
        float offset = GetOffset();

        for (int i = 0; i < items.Length; i++)
        {
            RectTransform item = items[i];
            Vector2 shown = shownPositions[i];

            Tween.StopAll(item);
            item.anchoredPosition = new Vector2(shown.x, shown.y - offset);
            Tween.UIAnchoredPositionY(item, shown.y, showDuration, showEase, startDelay: i * stagger);
        }
    }

    private void OnDisable()
    {
        foreach (RectTransform item in items)
            Tween.StopAll(item);
    }

    public void Hide(Action onComplete = null)
    {
        if (isHiding)
            return;

        if (!gameObject.activeInHierarchy || items.Length == 0)
        {
            onComplete?.Invoke();
            return;
        }

        isHiding = true;
        float offset = GetOffset();
        Tween last = default;

        for (int i = 0; i < items.Length; i++)
        {
            RectTransform item = items[i];

            Tween.StopAll(item);
            last = Tween.UIAnchoredPositionY(
                item,
                shownPositions[i].y - offset,
                hideDuration,
                hideEase,
                startDelay: i * stagger);
        }

        last.OnComplete(() =>
        {
            isHiding = false;
            onComplete?.Invoke();
        });
    }

    private float GetOffset()
    {
        Canvas rootCanvas = GetComponentInParent<Canvas>().rootCanvas;
        return ((RectTransform)rootCanvas.transform).rect.height;
    }
}