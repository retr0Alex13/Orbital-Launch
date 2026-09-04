using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class BackgroundElement : MonoBehaviour
{
    public float Radius { get; private set; }
    public Sprite CurrentSprite { get; private set; }

    private float parallaxFactor;
    private Transform cameraTransform;
    private Vector3 lastCameraPos;
    private SpriteRenderer spriteRenderer;

    private Vector2 driftVelocity;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Initialize(Sprite sprite, float scale, float parallax, Transform cam, Color tint, Vector2 driftVelocity = default)
    {
        spriteRenderer.sprite = sprite;
        CurrentSprite = sprite;
        spriteRenderer.color = tint;

        transform.localScale = new Vector3(scale, scale, 1f);
        Radius = Mathf.Max(spriteRenderer.bounds.extents.x, spriteRenderer.bounds.extents.y);

        parallaxFactor = parallax;
        cameraTransform = cam;
        lastCameraPos = cameraTransform.position;

        this.driftVelocity = driftVelocity;
    }

    private void LateUpdate()
    {
        if (cameraTransform == null) return;

        Vector3 delta = cameraTransform.position - lastCameraPos;

        transform.position += delta * parallaxFactor;
        lastCameraPos = cameraTransform.position;

        if (driftVelocity != Vector2.zero)
            transform.position += (Vector3)(driftVelocity * Time.deltaTime);
    }
}