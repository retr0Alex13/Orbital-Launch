using System;
using UnityEngine;

public class OrbitTutorialScanner : MonoBehaviour
{
    public event Action OnOrbitHitDetected;

    [SerializeField] private PlayerController player;
    [SerializeField] private LayerMask planetLayerMask;
    [SerializeField] private float rayLength = 6f;

    private readonly Collider2D[] overlapBuffer = new Collider2D[16];

    private void Update()
    {
        if (player.CurrentPlanet == null || player.IsAiming)
            return;

        Vector2 origin = player.transform.position;
        Vector2 direction = player.transform.up;

        if (OrbitRayUtility.TryFindNearestOrbitHit(
            origin, direction, rayLength, player.CurrentPlanet, planetLayerMask, overlapBuffer, out _))
        {
            OnOrbitHitDetected?.Invoke();
        }
    }
}