using UnityEngine;

public class Coin : MonoBehaviour
{
    public int coinValue = 10; 
    public Car moneySystem; 

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) 
        {
            if (moneySystem != null)
            {
                moneySystem.AddMoney(coinValue); 
                Destroy(gameObject);
            }
        }
    }
}