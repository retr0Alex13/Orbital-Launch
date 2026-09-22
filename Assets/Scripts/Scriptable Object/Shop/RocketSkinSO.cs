using UnityEngine;

public class RocketSkinSO : SkinItemSO
{
    [SerializeField] private RocketVisualRig rigPrefab;
    public RocketVisualRig RigPrefab => rigPrefab;
}