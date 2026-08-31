using System;
using UnityEngine;

public class FuelController : MonoBehaviour
{
    public event Action<float> OnFuelChanged;
    public event Action OnFuelInsufficient;

    public bool HasFuel => currentFuel >= launchCost;

    [SerializeField] private float maxFuel = 100f;
    [SerializeField] private float launchCost = 35f;

    private float currentFuel;

    private void Awake()
    {
        currentFuel = maxFuel;
    }

    public bool TryConsume()
    {
        if (!HasFuel)
        {
            OnFuelInsufficient?.Invoke();
            return false;
        }

        currentFuel = Mathf.Max(0f, currentFuel - launchCost);
        NotifyChanged();
        return true;
    }

    public void Refill()
    {
        currentFuel = maxFuel;
        NotifyChanged();
    }

    private void NotifyChanged()
    {
        OnFuelChanged?.Invoke(currentFuel / maxFuel);
    }

    public void NotifyInsufficient()
    {
        OnFuelInsufficient?.Invoke();
    }
}