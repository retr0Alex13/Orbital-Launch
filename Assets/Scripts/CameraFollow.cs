using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private PlanetSpawner planetSpawner;

    [SerializeField] private float positionSmoothTime = 0.2f;
    [SerializeField] private float sizeSmoothTime = 0.3f;
    [SerializeField] private float framingPadding = 2f;
    [SerializeField] private float minOrthographicSize = 5f;
    [SerializeField] private float maxOrthographicSize = 8f;

    private Camera cam;
    private Vector3 positionVelocity;
    private float sizeVelocity;

    private Vector3 lastFramedPosition;
    private float lastFramedSize;

    private Vector3 lastPlayerPosition;
    private Vector3 playerVelocityEstimate;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        lastFramedPosition = transform.position;
        lastFramedSize = minOrthographicSize;
        lastPlayerPosition = player.transform.position;
    }

    private void LateUpdate()
    {
        Vector3 currentPlayerPos = player.transform.position;
        if (Time.deltaTime > 0f)
            playerVelocityEstimate = (currentPlayerPos - lastPlayerPosition) / Time.deltaTime;
        lastPlayerPosition = currentPlayerPos;

        if (player.CurrentPlanet == null)
            FollowPlayer();
        else
            FrameCurrentAndNextPlanet();
    }

    private void FollowPlayer()
    {
        Vector3 predicted = player.transform.position + playerVelocityEstimate * positionSmoothTime;

        Vector3 target = new Vector3(predicted.x, predicted.y, transform.position.z);

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
            Vector2 a = current.transform.position;
            Vector2 b = next.transform.position;
            float rA = current.OrbitRadius;
            float rB = next.OrbitRadius;

            float minX = Mathf.Min(a.x - rA, b.x - rB);
            float maxX = Mathf.Max(a.x + rA, b.x + rB);
            float minY = Mathf.Min(a.y - rA, b.y - rB);
            float maxY = Mathf.Max(a.y + rA, b.y + rB);

            Vector3 midpoint = new Vector3((minX + maxX) * 0.5f, (minY + maxY) * 0.5f, transform.position.z);
            targetPosition = midpoint;

            float halfWidth = (maxX - minX) * 0.5f;
            float halfHeight = (maxY - minY) * 0.5f;

            float sizeForHeight = halfHeight;
            float sizeForWidth = cam.aspect > 0f ? halfWidth / cam.aspect : halfWidth;

            targetSize = Mathf.Clamp(
                Mathf.Max(sizeForHeight, sizeForWidth) + framingPadding,
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