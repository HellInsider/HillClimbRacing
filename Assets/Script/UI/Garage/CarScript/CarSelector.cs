using UnityEngine;
using UnityEngine.SceneManagement;

public class CarSelector : MonoBehaviour
{
    [SerializeField] private CarSelect[] cars;  // “ип массива изменЄн на CarSelect
    [SerializeField] private CarDisplay carDisplay;
    private int currentIndex;

    public void BackToMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);

    }
    private void Awake()
    {
        if (cars.Length > 0) ChangeCar(0);
    }

    public void ChangeCar(int change)
    {
        currentIndex += change;
        if (currentIndex < 0) currentIndex = cars.Length - 1;
        else if (currentIndex >= cars.Length) currentIndex = 0;

        if (carDisplay != null) carDisplay.DisplayCar(cars[currentIndex]);
    }
}