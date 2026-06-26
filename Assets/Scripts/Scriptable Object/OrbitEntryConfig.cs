using UnityEngine;

[CreateAssetMenu(fileName = "OrbitEntryConfig", menuName = "Config/Orbit Entry Config")]
public class OrbitEntryConfig : ScriptableObject
{
    public float GoodThreshold => goodThreshold;
    public float NearMissThreshold => nearMissThreshold;

    public int GoodPoints => goodPoints;
    public int PerfectPoints => perfectPoints;
    public int NearMissPoints => nearMissPoints;

    // Tune these if GOOD fires too often → raise GoodThreshold
    // Tune these if NEAR MISS fires too often → lower NearMissThreshold
    [SerializeField]
    private float goodThreshold = 0.90f;

    [SerializeField]
    private float nearMissThreshold = 0.50f;


    [SerializeField]
    private int goodPoints = 100;

    [SerializeField]
    private int perfectPoints = 200;

    [SerializeField]
    private int nearMissPoints = 150;
}
