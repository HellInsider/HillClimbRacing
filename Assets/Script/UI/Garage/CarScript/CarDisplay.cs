using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CarDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text carName;
    [SerializeField] private TMP_Text carDescription;
    [SerializeField] private Image carImage;
    [SerializeField] private Button selectButton;

    public void DisplayCar(CarSelect car)
    {
        carName.text = car.carName;
        carDescription.text = car.carDescription;
        carImage.sprite = car.carImage;
        carImage.color = Color.white;

        selectButton.onClick.RemoveAllListeners();
        selectButton.onClick.AddListener(() => SelectCar(car));
    }

    private void SelectCar(CarSelect car)
    {
        PlayerPrefs.SetInt("selectedCarIndex", car.carIndex);
        Debug.Log($"Selected car: {car.carName}");
    }
}