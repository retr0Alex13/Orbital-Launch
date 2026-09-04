using UnityEngine;

public class InfiniteTiledParallaxBackground : MonoBehaviour
{
    [SerializeField]
    private Transform target;  

    [SerializeField, Range(0f, 1f)]
    private float parallaxFactor = 0.5f;

    [SerializeField]
    private Vector2 autoScrollSpeed = new Vector2(0f, 0.5f);

    private SpriteRenderer spriteRenderer;

    private Vector3 lastTargetPos;

    private float tileWidth;
    private float tileHeight;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        tileWidth = spriteRenderer.sprite.bounds.size.x;
        tileHeight = spriteRenderer.sprite.bounds.size.y;
        lastTargetPos = target.position;
    }

    void LateUpdate()
    {
        Vector3 delta = target.position - lastTargetPos;

        Vector3 move = new Vector3(delta.x * parallaxFactor, delta.y * parallaxFactor, 0)
                      + (Vector3)(autoScrollSpeed * Time.deltaTime);

        transform.position += move;
        lastTargetPos = target.position;

        Vector3 pos = transform.position;
        float diffX = target.position.x - pos.x;
        float diffY = target.position.y - pos.y;

        if (Mathf.Abs(diffX) >= tileWidth)
            pos.x += tileWidth * Mathf.Sign(diffX);
        if (Mathf.Abs(diffY) >= tileHeight)
            pos.y += tileHeight * Mathf.Sign(diffY);

        transform.position = pos;
    }
}