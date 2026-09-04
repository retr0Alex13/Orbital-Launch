using UnityEngine;

public class RocketParts : MonoBehaviour
{
    [SerializeField]
    private GameObject rocketPartsParent;

    private GameObject instantiatedParts;

    public void SpawnParts(Vector2 position)
    {
        instantiatedParts = Instantiate(rocketPartsParent, position, transform.rotation);

        foreach(var part in instantiatedParts.GetComponentsInChildren<Rigidbody2D>())
        {
            part.AddForce(new Vector2(Random.Range(0f, 360f), Random.Range(0f, 360f)) * 0.3f);
        }
    }
}
