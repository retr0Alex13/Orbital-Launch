using UnityEngine;

public class FuelDisplay : MonoBehaviour
{
    [SerializeField] private FuelController fuelController;
    [SerializeField] private FuelWindow fuelWindow;

    private void Start()
    {
        fuelController.OnFuelChanged += HandleFuelChanged;
        fuelController.OnFuelInsufficient += HandleFuelInsufficient;
        fuelWindow.SetFillImmediate(1f);
    }

    private void OnDestroy()
    {
        fuelController.OnFuelChanged -= HandleFuelChanged;
        fuelController.OnFuelInsufficient -= HandleFuelInsufficient;
    }

    private void HandleFuelChanged(float normalizedAmount)
    {
        fuelWindow.SetFillAmount(normalizedAmount);
    }

    private void HandleFuelInsufficient()
    {
        fuelWindow.PlayInsufficientFuelEffect();
    }
}