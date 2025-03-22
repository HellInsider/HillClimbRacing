using UnityEngine;

public class MoneySystem : MonoBehaviour
{
    [SerializeField] public int initialMoney = 0;
    [SerializeField] public int maxMoney = 999999; 

    private int currentMoney; 

    void Start()
    {
        currentMoney = initialMoney; 
        UpdateMoneyUI(); 
    }

    
    public void AddMoney(int amount)
    {
        if (amount < 0) return; 

        currentMoney += amount;
        currentMoney = Mathf.Min(currentMoney, maxMoney); 
        Debug.Log($"Добавлено {amount} денег. Текущий баланс: {currentMoney}");

        UpdateMoneyUI(); 
    }

    
   

    
    public int GetCurrentMoney()
    {
        return currentMoney;
    }

    
    private void UpdateMoneyUI()
    {
    }
}