using UnityEngine;
using UnityEngine.UI;
public enum myEnum
{
    IncreasingTheMaximum,
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
    public Car wheel;
    [SerializeField] myEnum DropDown;
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            switch (DropDown)
            {
                case myEnum.IncreasingTheMaximum:
                    break;
                case myEnum.Shield:
                    break;
                case myEnum.IncreasesTheGasTank:
                    break;
                case myEnum.GravityReduction:
                    break;
                case myEnum.IncreaseInPoints:
                    break;
                case myEnum.BigWheels:
                    if (wheel != null)
                    {
                        wheel.GetComponent<Car>().UpgradeWheels(); ;
                        Debug.Log("Машина заправлена!");
                        Debug.Log(DropDown);
                    }
                    break;
                case myEnum.DeadlyAcceleration:
                    break;
                case myEnum.TheClutch:
                    break;
            }            
        }
    }
}
