using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OrientationLayoutChanger : UIBehaviour
{
    [Serializable]
    private class ContainerPair
    {
        public Transform landscape;
        public Transform portrait;
        public Transform Get(bool isPortrait) => isPortrait ? portrait : landscape;
    }

    [SerializeField] private ContainerPair[] containers;
    [SerializeField] private GameObject landscapeRoot;
    [SerializeField] private GameObject portraitRoot;

    private bool? _appliedPortrait;

    private static bool IsPortrait => Screen.height > Screen.width;

    public Transform GetContainer(int index) => containers[index].Get(IsPortrait);

    protected override void OnEnable()
    {
        base.OnEnable();
        Apply(force: true);
    }

    protected override void OnRectTransformDimensionsChange()
    {
        base.OnRectTransformDimensionsChange();
        Apply(force: false);
    }

    private void Apply(bool force)
    {
        if (!isActiveAndEnabled) return;

        bool portrait = IsPortrait;
        if (!force && _appliedPortrait == portrait) return;
        _appliedPortrait = portrait;

        landscapeRoot.SetActive(!portrait);
        portraitRoot.SetActive(portrait);

        foreach (var pair in containers)
            MoveCards(pair.Get(!portrait), pair.Get(portrait));

        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform);
    }

    private static void MoveCards(Transform from, Transform to)
    {
        var cards = new Transform[from.childCount];
        for (int i = 0; i < cards.Length; i++)
            cards[i] = from.GetChild(i);

        foreach (var card in cards)
            card.SetParent(to, false);
    }
}
