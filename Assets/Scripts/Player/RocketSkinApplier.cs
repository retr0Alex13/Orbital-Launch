using System;
using UnityEngine;

public class RocketSkinApplier : MonoBehaviour
{
    [SerializeField] private Transform visualsRoot;
    [SerializeField] private RocketSkinSO defaultSkin;

    private Rigidbody2D _playerRigidBody;
    private InventoryService _inventory;
    private ShopItemDatabase _database;
    private RocketVisualRig _currentRig;

    public RocketVisualRig CurrentRig => _currentRig;
    public event Action<RocketVisualRig> OnRigChanged;

    public void Initialize(InventoryService inventory, ShopItemDatabase database, Rigidbody2D playerRigidBody)
    {
        _inventory = inventory;
        _database = database;
        _playerRigidBody = playerRigidBody;

        _inventory.OnSkinEquipped += HandleSkinEquipped;
        ApplyCurrentSkin();
    }

    private void OnDestroy()
    {
        if (_inventory != null)
            _inventory.OnSkinEquipped -= HandleSkinEquipped;
    }

    private void ApplyCurrentSkin()
    {
        string equippedId = _inventory.GetEquippedRocketSkinId();
        var skin = FindSkinById(equippedId) ?? defaultSkin;

        if (skin != null)
            SwapRig(skin.RigPrefab);
    }

    private void HandleSkinEquipped(SkinItemSO skin)
    {
        if (skin is RocketSkinSO rocketSkin)
            SwapRig(rocketSkin.RigPrefab);
    }

    private void SwapRig(RocketVisualRig rigPrefab)
    {
        if (_currentRig != null)
            Destroy(_currentRig.gameObject);

        _currentRig = Instantiate(rigPrefab, visualsRoot);
        _currentRig.transform.localPosition = Vector3.zero;
        _currentRig.transform.localRotation = Quaternion.identity;

        foreach (var trailController in _currentRig.TrailControllers)
            trailController.Initialize(_playerRigidBody);

        OnRigChanged?.Invoke(_currentRig);
    }

    private RocketSkinSO FindSkinById(string id)
    {
        if (string.IsNullOrEmpty(id)) return null;
        foreach (var skin in _database.RocketSkins)
            if (skin.ItemId == id) return skin;
        return null;
    }
}