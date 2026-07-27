using PrimeTween;
using System.Collections;
using TMPro;
using UnityEngine;

public class DeathTimer : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI timerText;

    [SerializeField]
    private CanvasGroup canvasGroup;

    [SerializeField]
    private PlayerController player;

    [SerializeField]
    private float delayTime = 2f;

    [SerializeField]
    private float openSpaceTime = 3f;

    private Sequence timerAnimation;
    private Vector2 startPosition;

    private void Awake()
    {
        startPosition = timerText.rectTransform.anchoredPosition;
    }

    private void OnEnable()
    {
        player.OnPlayerLaunched += StartTimer;
        player.OnPlayerCaptured += StopTimer;
    }

    private void OnDestroy()
    {
        player.OnPlayerLaunched -= StartTimer;
        player.OnPlayerCaptured -= StopTimer;
    }

    private void StartTimer()
    {
        StartCoroutine(nameof(DeathTimerCoroutine));
    }

    private void StopTimer(Planet planet)
    {
        StopCoroutine(nameof(DeathTimerCoroutine));
    }

    private IEnumerator DeathTimerCoroutine()
    {
        yield return new WaitForSeconds(delayTime);

        float remainingTime = openSpaceTime;
        int currentTime = Mathf.CeilToInt(openSpaceTime) + 1;

        while (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;

            timerText.text = Mathf.CeilToInt(remainingTime).ToString();

            if (Mathf.CeilToInt(remainingTime) < currentTime)
            {
                currentTime = Mathf.CeilToInt(remainingTime);
                timerText.rectTransform.anchoredPosition = startPosition;
                canvasGroup.alpha = 1;
                AnimateAndHide();
            }

            yield return null;
        }

        GameManager.Instance.RestartGame();
    }

    private void AnimateAndHide()
    {
        if (timerAnimation.isAlive)
            timerAnimation.Stop();

        timerAnimation = Tween.UIAnchoredPositionY(timerText.rectTransform, endValue: startPosition.y + 100f, duration: 1f, ease: Ease.InOutSine)
            .Group(Tween.Alpha(canvasGroup, startValue: 1f, endValue: 0, duration: 1f, ease: Ease.InOutSine));
    }
}
