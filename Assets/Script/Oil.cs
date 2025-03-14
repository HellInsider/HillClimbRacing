using UnityEngine;

public class Oil : MonoBehaviour
{

    [SerializeField] public float maxFuel;
    [SerializeField] public float Expenditure;
    [SerializeField] public float refillAmount; 

    private float currentFuel; 
    private bool isOutOfFuel = false; 
    private MoveCar carController; 

    void Start()
    {
        currentFuel = maxFuel; 
        carController = GetComponent<MoveCar>(); 
    }

    void Update()
    {
        if (isOutOfFuel) return; 
        if (carController != null && IsCarMoving())
        {
            currentFuel -= Expenditure * Time.deltaTime;
            currentFuel = Mathf.Max(currentFuel, 0); 

            if (currentFuel <= 0)
            {
                OutOfFuel();
            }
        }
    }

    
    private bool IsCarMoving()
    {
        
        return Mathf.Abs(carController.GetMoveInput()) > 0.1f || carController.GetCurrentSpeed() > 0.1f;
    }

    
    private void OutOfFuel()
    {
        isOutOfFuel = true;
        Debug.Log("Топливо закончилось!");
        if (carController != null)
        {
            carController.StopCar();
            carController.checkfuel = false;
        }
    }

    public void RefillFuel()
    {
        carController.checkfuel = true;
        currentFuel += refillAmount;
        currentFuel = Mathf.Min(currentFuel, maxFuel); 
        isOutOfFuel = false; 
        Debug.Log("Заправлено! Текущее топливо: " + currentFuel);
    }
    public float GetCurrentFuel()
    {
        return currentFuel;
    }
    public float GetMaxFuel()
    {
        return maxFuel;
    }
}