using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private PlanetSpawner planetSpawner;

    [SerializeField] private float positionSmoothTime = 0.2f;
    [SerializeField] private float sizeSmoothTime = 0.3f;
    [SerializeField] private float framingPadding = 2f;
    [SerializeField] private float minOrthographicSize = 5f;
    [SerializeField] private float maxOrthographicSize = 8f;

    private Camera cam;
    private Vector3 positionVelocity;
    private float sizeVelocity;

    // Remembered so we don't snap when next planet is temporarily missing
    private Vector3 lastFramedPosition;
    private float lastFramedSize;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        lastFramedPosition = transform.position;
        lastFramedSize = minOrthographicSize;
    }

    private void LateUpdate()
    {
        if (player.CurrentPlanet == null)
            FollowPlayer();
        else
            FrameCurrentAndNextPlanet();
    }

    private void FollowPlayer()
    {
        Vector3 target = new Vector3(
            player.transform.position.x,
            player.transform.position.y,
            transform.position.z);

        transform.position = Vector3.SmoothDamp(
            transform.position, target, ref positionVelocity, positionSmoothTime);

        cam.orthographicSize = Mathf.SmoothDamp(
            cam.orthographicSize, minOrthographicSize, ref sizeVelocity, sizeSmoothTime);

        lastFramedPosition = transform.position;
        lastFramedSize = cam.orthographicSize;
    }

    private void FrameCurrentAndNextPlanet()
    {
        Planet current = player.CurrentPlanet;
        Planet next = planetSpawner.GetNextPlanetAfter(current);

        Vector3 targetPosition;
        float targetSize;

        if (next != null)
        {
            Vector3 a = current.transform.position;
            Vector3 b = next.transform.position;
            Vector3 midpoint = (a + b) * 0.5f;

            targetPosition = new Vector3(midpoint.x, midpoint.y, transform.position.z);

            float distance = Vector2.Distance(a, b);
            targetSize = Mathf.Clamp(
                distance * 0.5f + framingPadding,
                minOrthographicSize,
                maxOrthographicSize);

            lastFramedPosition = targetPosition;
            lastFramedSize = targetSize;
        }
        else
        {
            targetPosition = lastFramedPosition;
            targetSize = lastFramedSize;
        }

        transform.position = Vector3.SmoothDamp(
            transform.position, targetPosition, ref positionVelocity, positionSmoothTime);

        cam.orthographicSize = Mathf.SmoothDamp(
            cam.orthographicSize, targetSize, ref sizeVelocity, sizeSmoothTime);
    }
}