using PrimeTween;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField] private float secondsPerRevolution = 2f;
    [SerializeField] private bool clockwise = true;
    [SerializeField] private bool playOnEnable = true;

    private Tween _tween;

    private void OnEnable()
    {
        if (playOnEnable)
            StartRotation();
    }

    private void OnDisable()
    {
        StopRotation();
    }

    public void StartRotation()
    {
        StopRotation();

        float startZ = transform.localEulerAngles.z;
        float endZ = startZ + (clockwise ? -360f : 360f);

        _tween = Tween.LocalEulerAngles(
            transform,
            new Vector3(0f, 0f, startZ),
            new Vector3(0f, 0f, endZ),
            secondsPerRevolution,
            Ease.Linear,
            cycles: -1,
            cycleMode: CycleMode.Incremental);
    }

    public void StopRotation()
    {
        if (_tween.isAlive)
            _tween.Stop();
    }
}
