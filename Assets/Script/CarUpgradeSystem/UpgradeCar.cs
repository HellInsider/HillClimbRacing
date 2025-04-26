using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UpgradeCar : MonoBehaviour
{
    public Car car;
   
    [Header("Колеса")]
    [SerializeField] public float baseScale = 1f;
    [SerializeField] public float upgradeMultiplier = 1.2f;
    [SerializeField] public int maxUpgrades = 5;
    [SerializeField] public bool affectPhysics = true;
    [SerializeField] public Transform[] wheelVisuals;
    [SerializeField] public WheelJoint2D[] wheelJoints;
    
    [Header("Топливо")]
    [SerializeField] public float initialFuelBonus = 20;     
    [SerializeField] public float speedPenaltyPerLevel = 10f;
    [SerializeField] int maxUpgradeLevel = 7;
   
    [Header("Скорость")]
    [SerializeField] float TheMaximumSpeed;

    [Header("Гравитация")]
    [SerializeField] public float baseGravity = 1f;
    [SerializeField] public float gravityReduction = 10;
    
    [Header("Настройки щита")]
    [SerializeField] private GameObject helmetPrefab;
    [SerializeField] private Transform helmetSocket;
    [SerializeField] private int shieldLevel = 0;
    [SerializeField] public HeadDeath head;
    public bool isShieldActive;

    [Header("Увеличение накопленных очков")]
    [SerializeField] private int increase;
    [Header("Смертельное ускорение")]
    [SerializeField] public float time = 5f;
    [Header("Настройки сцепления")]
  
    [SerializeField] private float baseFrequency = 5f;       
    [SerializeField] private float baseMotorTorque = 500f;   
    [SerializeField] private float gripStep = 1f;           
    [SerializeField] private float maxGripMultiplier = 3f;

    private float currentGripMultiplier = 1f;
    private float timer;
    private bool DeadlyAcceleration;
    private GameObject activeHelmet;
    private float savedFuel; 
    private int currentUpgrades = 0;
    private float currentScale;
    private Transform centerOfMass;
    private int currentLevel = 0;
    private float percent;
    private float currentGravityMultiplier =1f;
    private GameObject currentHelmet;
    private Vector3 deathPosition;
    private float tempSpeed;
    private void Start()
    {
        StartWheel();
        wheelJoints = car._wheelJoints;
        centerOfMass = car.centerOfMass;
        isShieldActive = false;
        DeadlyAcceleration = false;
        tempSpeed = car._engineForce;
        //LoadShieldProgress();
        ApplyGripSettings();
    }
   
    public float UpdateVariable (float Variable, float Percent)
    {
        return (Variable * Percent)/100;
    }
    public void StartWheel()
    {
        currentScale = baseScale;
        ApplyWheelScale();
    }
    void Update()
    {
        if (timer > 0 && DeadlyAcceleration)
        {
            timer -= Time.deltaTime;
            Debug.Log(timer);
        }
        else
        {
            DeadlyAcceleration = false;
            car._engineForce = tempSpeed;
        }
    }
   
    public void UpgradeWheels()
    {
        if (currentUpgrades >= maxUpgrades)
        {
            Debug.Log("Достигнут максимум улучшений колес!");
            return;
        }

        currentUpgrades++;
        currentScale *= upgradeMultiplier;

        ApplyWheelScale();
        Debug.Log($"Улучшение колес (уровень {currentUpgrades}/{maxUpgrades})");
    }
    public void ApplyWheelScale()
    {
        foreach (var wheel in wheelJoints)
        {
            if (wheel == null) continue;

            Transform wheelTransform = wheel.transform;
            wheelTransform.localScale = Vector3.one * currentScale;
        }

        if (wheelVisuals != null)
        {
            foreach (var visual in wheelVisuals)
            {
                if (visual == null) continue;
                visual.localScale = Vector3.one * currentScale;
            }
        }
    }
    public void ResetUpgrades()
    {
        currentUpgrades = 0;
        currentScale = baseScale;
        ApplyWheelScale();
    }
    public void FuelUpdate()
    {
        if (currentLevel <= maxUpgradeLevel)
        {
            percent = UpdateVariable(car._maxFuel, initialFuelBonus);
            car._maxFuel += percent;
            percent = UpdateVariable(car._engineForce, speedPenaltyPerLevel);
            car._engineForce -= percent;
            Debug.Log(car._maxFuel.ToString());
            Debug.Log(car._engineForce.ToString());
            //currentLevel++;
        }
        
    }
    public void SpeedUpdate()
    {
        if (currentLevel <= maxUpgradeLevel)
        {
            percent = UpdateVariable(car._engineForce, TheMaximumSpeed);
            car._engineForce += percent;
            Debug.Log(car._engineForce.ToString());
        }
    }
    public void ApplyGravity()
    {
        currentGravityMultiplier = UpdateVariable(currentGravityMultiplier, gravityReduction);
        Physics2D.gravity = new Vector2(
            0f,
            -Mathf.Abs(Physics2D.gravity.y) * currentGravityMultiplier           
        );
        Debug.Log(currentGravityMultiplier.ToString());
        Debug.Log(-Mathf.Abs(Physics2D.gravity.y) * currentGravityMultiplier);
    }
    public void ActivateShield()
    {
        isShieldActive = true;
        car._isDead++;
    }
    public void IncreaseinAaccumulated()
    {
        car._currentMoney += (int)UpdateVariable(car._currentMoney, increase);
    }
    public void  Acceleration()
    {
        DeadlyAcceleration = true;
        car._engineForce *= 2;
        timer = time;
    }
    public void IncreaseGrip()
    {
        currentGripMultiplier += gripStep;
        currentGripMultiplier = Mathf.Min(currentGripMultiplier, maxGripMultiplier);
        ApplyGripSettings();
    }
    private void ApplyGripSettings()
    {
        foreach (var wheelJoint in wheelJoints)
        {
            if (wheelJoints == null) return;
            var suspension = wheelJoint.suspension;
            suspension.frequency = baseFrequency * currentGripMultiplier;
            wheelJoint.suspension = suspension;
            var motor = wheelJoint.motor;
            motor.maxMotorTorque = baseMotorTorque * currentGripMultiplier;
            wheelJoint.motor = motor;
        }
    }
}
