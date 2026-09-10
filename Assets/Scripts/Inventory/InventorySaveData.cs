using System;
using System.Collections.Generic;

[Serializable]
public class InventorySaveData
{
    public List<string> ownedItemIds = new();
    public string equippedRocketSkinId;
    public string equippedTrailSkinId;
    public bool hasClaimedFreeBox;
}