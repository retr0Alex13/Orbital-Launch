using UnityEngine;

public class FuelDisplay : MonoBehaviour
{
    [SerializeField] private FuelController fuelController;
    [SerializeField] private FuelWindow fuelWindow;
    private float previousFuelAmount = 1f;

    private void Start()
    {
        fuelController.OnFuelChanged += HandleFuelChanged;
        fuelController.OnFuelInsufficient += HandleFuelInsufficient;
        fuelWindow.SetFillImmediate(1f);
        fuelWindow.SetTransparencyImmediate(0f);
    }

    private void OnDestroy()
    {
        fuelController.OnFuelChanged -= HandleFuelChanged;
        fuelController.OnFuelInsufficient -= HandleFuelInsufficient;
    }

    private void HandleFuelChanged(float normalizedAmount)
    {

        bool isDecreasing = normalizedAmount < previousFuelAmount;
        previousFuelAmount = normalizedAmount;

        fuelWindow.SetFillAmount(normalizedAmount);
        fuelWindow.SetVisible(isDecreasing);
    }

    private void HandleFuelInsufficient()
    {
        fuelWindow.PlayInsufficientFuelEffect();
    }
}