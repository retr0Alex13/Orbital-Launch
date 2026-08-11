using UnityEngine;

public class TapController : MonoBehaviour
{
    [SerializeField]
    private RectTransform canvas;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas, Input.mousePosition, null, out localPoint);

            GameObject tapVFX = TapPool.SharedInstance.GetPooledObject();

            tapVFX.transform.SetParent(gameObject.transform);
            tapVFX.transform.localPosition = localPoint;
            tapVFX.SetActive(true);
        }
    }
}
