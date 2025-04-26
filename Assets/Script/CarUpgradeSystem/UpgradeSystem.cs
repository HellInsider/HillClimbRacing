using UnityEngine;
using UnityEngine.UI;
public enum myEnum
{
    IncreasingTheMaximumSpeed,
    Shield,
    IncreasesTheGasTank,
    GravityReduction,
    IncreaseInPoints,
    BigWheels,
    DeadlyAcceleration,
    TheClutch
};
public class UpgradeSystem : MonoBehaviour
{
    public Car car;
    public UpgradeCar UpgCar;
    [SerializeField] myEnum DropDown;
    private void Start()
    {
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            switch (DropDown)
            {
                case myEnum.IncreasingTheMaximumSpeed:
                    //Это пока примерный код потом доработать
                    UpgCar.SpeedUpdate();
                    break;
                case myEnum.Shield:
                    UpgCar.ActivateShield();
                    break;
                case myEnum.IncreasesTheGasTank:
                    UpgCar.FuelUpdate();
                    break;
                case myEnum.GravityReduction:
                    UpgCar.ApplyGravity();
                    break;
                case myEnum.IncreaseInPoints:
                    UpgCar.IncreaseinAaccumulated();
                    break;
                case myEnum.BigWheels:
                    if (car != null)
                    {
                        car.GetComponent<UpgradeCar>().UpgradeWheels();
                        Debug.Log(DropDown);
                    }
                    break;
                case myEnum.DeadlyAcceleration:
                    UpgCar.Acceleration();
                    break;
                case myEnum.TheClutch:
                    UpgCar.IncreaseGrip();
                    break;
            }            
        }
    }
}
