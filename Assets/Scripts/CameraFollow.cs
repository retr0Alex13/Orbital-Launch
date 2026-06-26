using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    private Player player;

    [SerializeField]
    private PlanetSpawner planetSpawner;

    [SerializeField]
    private float followSpeed = 8f;

    [SerializeField]
    private float framingPadding = 2f;

    [SerializeField]
    private float minOrthographicSize = 5f;

    [SerializeField]
    private float maxOrthographicSize = 8f;

    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        if (player.CurrentPlanet == null)
        {
            FollowPlayer();
        }
        else
        {
            FrameCurrentAndNextPlanet();
        }
    }

    private void FollowPlayer()
    {
        Vector3 targetPosition = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);

        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, minOrthographicSize, followSpeed * Time.deltaTime);
    }

    private void FrameCurrentAndNextPlanet()
    {
        Planet currentPlanet = player.CurrentPlanet;
        Planet nextPlanet = planetSpawner.NextPlanet;

        if (currentPlanet == null || nextPlanet == null)
        {
            FollowPlayer();
            return;
        }

        Vector3 a = currentPlanet.transform.position;
        Vector3 b = nextPlanet.transform.position;
        Vector3 midpoint = (a + b) * 0.5f;

        Vector3 targetPosition = new Vector3(midpoint.x, midpoint.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);

        float distance = Vector2.Distance(a, b);
        float targetSize = Mathf.Clamp(distance * 0.5f + framingPadding, minOrthographicSize, maxOrthographicSize);
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize, followSpeed * Time.deltaTime);
    }
}