using UnityEngine;

public class Refill : MonoBehaviour
{
    public Car carFuelSystem; 

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) 
        {
            if (carFuelSystem != null)
            {
                carFuelSystem.RefillFuel();
                Destroy(gameObject);
            }
        }
    }
}