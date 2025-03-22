using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MoneyUI : MonoBehaviour
{
    public MoneySystem moneySystem; 
    public TextMeshProUGUI moneyText; 

    void Update()
    {
        if (moneySystem != null && moneyText != null)
        {
            moneyText.text = $"Δενόγθ: {moneySystem.GetCurrentMoney()}";
        }
    }
}