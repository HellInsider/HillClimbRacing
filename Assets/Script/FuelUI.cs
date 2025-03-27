using UnityEngine;
using UnityEngine.UI;

public class FuelUI : MonoBehaviour
{
    public Car carFuelSystem; 
    public Slider fuelSlider; 

    void Update()
    {
        if (carFuelSystem != null && fuelSlider != null)
        {
            fuelSlider.value = carFuelSystem.GetCurrentFuel() / carFuelSystem.GetMaxFuel();
        }
    }
}