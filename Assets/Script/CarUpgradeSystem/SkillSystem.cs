using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class SkillSystem : MonoBehaviour
{
    [SerializeField] private Car car;
    [SerializeField] private UpgradeCar upgradeCar; 
    [SerializeField] private Canvas skillSelectionCanvas; 
    [SerializeField] private Button[] skillButtons; 
    [SerializeField] private TextMeshProUGUI[] skillNames; 
    [SerializeField] private TextMeshProUGUI[] skillDescriptions; 
    [Header("Настройки")]
    [SerializeField] private float distanceThreshold = 5f; 
    [SerializeField] private float initialDistance = 0f; 
    private float totalDistanceTraveled = 0f; 
    private Vector2 lastPosition; 
    private bool isSkillSelectionActive = false; 

    
    private enum SkillType
    {
        SpeedUpdate,
        FuelUpdate,
        UpgradeWheels,
        ApplyGravity,
        ActivateShield,
        IncreaseInPoints,
        Acceleration,
        IncreaseGrip
    }
    [System.Serializable]
    private struct SkillInfo
    {
        public SkillType type;
        public string name;
        public string description;
    }

    [SerializeField]
    private SkillInfo[] availableSkills = new SkillInfo[]
    {
        new SkillInfo { type = SkillType.SpeedUpdate, name = "Увеличение скорости", description = "Увеличивает силу двигателя" },
        new SkillInfo { type = SkillType.FuelUpdate, name = "Увеличение бака", description = "Увеличивает максимальный объем топлива но снижает скорость" },
        new SkillInfo { type = SkillType.UpgradeWheels, name = "Большие колеса", description = "Увеличивает размер колес" },
        new SkillInfo { type = SkillType.ApplyGravity, name = "Снижение гравитации", description = "Временно уменьшает гравитацию но и скорость" },
        new SkillInfo { type = SkillType.ActivateShield, name = "Щит", description = "Добавляет одну жизнь." },
        new SkillInfo { type = SkillType.IncreaseInPoints, name = "Больше очков", description = "Увеличивает деньги" },
        new SkillInfo { type = SkillType.Acceleration, name = "Смертельное ускорение", description = "Временно удваивает скорость." },
        new SkillInfo { type = SkillType.IncreaseGrip, name = "Улучшение сцепления", description = "Увеличивает сцепление колес." }
    };
    private void Start()
    {
        if (car == null || upgradeCar == null)
        {
            Debug.LogError("Car или UpgradeCar не назначены в инспекторе!");
            return;
        }

        lastPosition = car.transform.position;
        totalDistanceTraveled = initialDistance;

        if (skillSelectionCanvas != null)
            skillSelectionCanvas.enabled = false;

        if (skillButtons.Length != 3 || skillNames.Length != 3 || skillDescriptions.Length != 3)
        {
            Debug.LogError("Нужно назначить ровно 3 кнопки, названия и описания для UI!");
        }
    }
    private void Update()
    {
        if (car == null || isSkillSelectionActive) return;

        Vector2 currentPosition = car.transform.position;
        float distanceThisFrame = Vector2.Distance(currentPosition, lastPosition);
        totalDistanceTraveled += distanceThisFrame;
        lastPosition = currentPosition;
        if (totalDistanceTraveled >= distanceThreshold)
        {
            ShowSkillSelection();
            totalDistanceTraveled = 0f; 
        }
    }

    private void ShowSkillSelection()
    {
        if (skillSelectionCanvas == null) return;

        Time.timeScale = 0f;
        isSkillSelectionActive = true;
        car.EnableControl(false); 
        skillSelectionCanvas.enabled = true;
        List<SkillInfo> selectedSkills = GetRandomSkills(3);
        for (int i = 0; i < skillButtons.Length; i++)
        {
            int index = i;
            SkillInfo skill = selectedSkills[i];
            skillNames[i].text = skill.name;
            skillDescriptions[i].text = skill.description;
            skillButtons[i].onClick.RemoveAllListeners();
            skillButtons[i].onClick.AddListener(() => ApplySkill(skill.type));
        }
    }

    private List<SkillInfo> GetRandomSkills(int count)
    {
        List<SkillInfo> result = new List<SkillInfo>();
        List<SkillInfo> available = new List<SkillInfo>(availableSkills);
        for (int i = 0; i < count && available.Count > 0; i++)
        {
            int randomIndex = Random.Range(0, available.Count);
            result.Add(available[randomIndex]);
            available.RemoveAt(randomIndex);
        }
        return result;
    }

    private void ApplySkill(SkillType skillType)
    {
        if (upgradeCar == null) return;

        switch (skillType)
        {
            case SkillType.SpeedUpdate:
                upgradeCar.SpeedUpdate();
                break;
            case SkillType.FuelUpdate:
                upgradeCar.FuelUpdate();
                break;
            case SkillType.UpgradeWheels:
                upgradeCar.UpgradeWheels();
                break;
            case SkillType.ApplyGravity:
                upgradeCar.ApplyGravity();
                break;
            case SkillType.ActivateShield:
                upgradeCar.ActivateShield();
                break;
            case SkillType.IncreaseInPoints:
                upgradeCar.IncreaseinAaccumulated();
                break;
            case SkillType.Acceleration:
                upgradeCar.Acceleration();
                break;
            case SkillType.IncreaseGrip:
                upgradeCar.IncreaseGrip();
                break;
        }
        CloseSkillSelection();
    }

    private void CloseSkillSelection()
    {
        if (skillSelectionCanvas != null)
        {
            skillSelectionCanvas.enabled = false;
        }
        Time.timeScale = 1f;
        isSkillSelectionActive = false;
        car.EnableControl(true);
    }
}