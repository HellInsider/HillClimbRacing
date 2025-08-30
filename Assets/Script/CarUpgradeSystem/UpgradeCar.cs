using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UpgradeCar : MonoBehaviour
{
    [SerializeField] public Car car;
   
    [Header("Колеса")]
    [SerializeField] public float baseScale = 1.5f;
    [SerializeField] public float upgradeMultiplier = 1.2f;
    [SerializeField] public int maxUpgrades = 5;
    private WheelJoint2D[] wheelJoints;
    
    [Header("Топливо")]
    [SerializeField] public float initialFuelBonus = 20;     
    [SerializeField] public float speedPenaltyPerLevel = 10f;
    [SerializeField] int maxUpgradeLevel = 7;
   
    [Header("Скорость")]
    [SerializeField] float TheMaximumSpeed;

    [Header("Гравитация")]
    [SerializeField] public float baseGravity = 100f;
    [SerializeField] public float gravityReductionPercent = 70f; 
    [SerializeField] public float minGravityMultiplier = 0.1f; 
    [SerializeField] public int maxGravityUpgrades = 5; 
    [SerializeField] public float gravityEffectDuration = 10f; 
    [SerializeField] public float gravityTransitionSpeed = 2f; 
    [SerializeField] public float rechargeTime = 15f;

    [Header("Настройки щита")]
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

    private float currentGravityMultiplier = 1f;
    private float targetGravityMultiplier = 1f;
    private float previousGravityMultiplier = 1f;
    private int gravityUpgradeCount = 0;
    private bool isGravityEffectActive = false;
    private float rechargeTimer = 0f;
    private Rigidbody2D rb;
    private float currentGripMultiplier = 1f;
    private float timer;
    private bool DeadlyAcceleration;
  //private GameObject activeHelmet;
  //private float savedFuel; 
    private int currentUpgrades = 0;
    private float currentScale;
    private Transform centerOfMass;
    private int currentLevel = 0;
    private float percent;
    private GameObject currentHelmet;
    private Vector3 deathPosition;
    private float tempSpeed;
    private void Start()
    {
        wheelJoints = car._wheelJoints;
        //rb = car.rb;
        centerOfMass = car.centerOfMass;
        isShieldActive = false;
        DeadlyAcceleration = false;
        tempSpeed = car._engineForce;
        //LoadShieldProgress();
        ApplyGripSettings();
        ResetGravity();
        StartWheel();
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
        }
        else if (timer < 0)
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
        Debug.Log(currentScale);
        ApplyWheelScale();
        Debug.Log($"Улучшение колес (уровень {currentUpgrades}/{maxUpgrades})");
    }
    public void ApplyWheelScale()
    {
        foreach (WheelJoint2D wheel in wheelJoints)
        {
            if (wheel == null)
            {

                continue;
            }
            wheel.transform.localScale= Vector3.one* currentScale;
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
    /*public void ApplyGravity()
    {
       if (gravityUpgradeCount >= maxGravityUpgrades)
        {
            Debug.Log("Достигнут максимум улучшений гравитации!");
            return;
        }
        if (isGravityEffectActive)
        {
            Debug.Log("Эффект гравитации уже активен, ждите завершения!");
            return;
        }
        previousGravityMultiplier = currentGravityMultiplier;
        currentGravityMultiplier = UpdateVariable(currentGravityMultiplier, gravityReduction);
        currentGravityMultiplier = Mathf.Max(currentGravityMultiplier, minGravityMultiplier);
        gravityUpgradeCount++;
        Physics2D.gravity = new Vector2(0f, -baseGravity * currentGravityMultiplier);
        isGravityEffectActive = true;
        Debug.Log($"Гравитация уменьшена: {Physics2D.gravity.y} (Множитель: {currentGravityMultiplier}, Уровень: {gravityUpgradeCount})");
        StartCoroutine(GravityEffectTimer());
    }
    private IEnumerator GravityEffectTimer()
    {
        yield return new WaitForSeconds(gravityEffectDuration);
        currentGravityMultiplier = previousGravityMultiplier;
        Physics2D.gravity = new Vector2(0f, -baseGravity * currentGravityMultiplier);
        isGravityEffectActive = false;
        Debug.Log($"Гравитация восстановлена: {Physics2D.gravity.y} (Множитель: {currentGravityMultiplier})");
    }*/
    public void ApplyGravity()
    {
        if (gravityUpgradeCount >= maxGravityUpgrades)
        {
            Debug.Log("Достигнут максимум улучшений гравитации!");
            return;
        }
        if (isGravityEffectActive || rechargeTimer > 0)
        {
            Debug.Log($"Эффект гравитации недоступен: активен={isGravityEffectActive}, перезарядка={rechargeTimer}");
            return;
        }

        previousGravityMultiplier = currentGravityMultiplier;
        targetGravityMultiplier = Mathf.Max(minGravityMultiplier, 1f - (gravityReductionPercent / 100f));
        gravityUpgradeCount++;
        isGravityEffectActive = true;
        rechargeTimer = rechargeTime;

        Debug.Log($"Гравитация уменьшена: начальное значение={currentGravityMultiplier}, целевое={targetGravityMultiplier}, уровень={gravityUpgradeCount}");
        StartCoroutine(GravityEffectTransition());
    }

    private IEnumerator GravityEffectTransition()
    {
        float elapsedTime = 0f;
        while (elapsedTime < gravityEffectDuration)
        {
            elapsedTime += Time.deltaTime;
            currentGravityMultiplier = Mathf.Lerp(currentGravityMultiplier, targetGravityMultiplier, elapsedTime * gravityTransitionSpeed);
            rb.gravityScale = currentGravityMultiplier;
            Debug.Log($"Гравитация в процессе: {rb.gravityScale}");
            yield return null;
        }

        elapsedTime = 0f;
        while (elapsedTime < gravityEffectDuration * 0.2f) 
        {
            elapsedTime += Time.deltaTime;
            currentGravityMultiplier = Mathf.Lerp(targetGravityMultiplier, previousGravityMultiplier, elapsedTime * gravityTransitionSpeed);
            rb.gravityScale = currentGravityMultiplier;
            Debug.Log($"Гравитация возвращается: {rb.gravityScale}");
            yield return null;
        }

        currentGravityMultiplier = previousGravityMultiplier;
        rb.gravityScale = currentGravityMultiplier;
        isGravityEffectActive = false;
        Debug.Log($"Гравитация восстановлена: {rb.gravityScale}");
    }
    public void ResetGravity()
    {
        currentGravityMultiplier = 1f;
        previousGravityMultiplier = 1f;
        gravityUpgradeCount = 0;
        isGravityEffectActive = false;
        Physics2D.gravity = new Vector2(0f, -baseGravity);
        StopAllCoroutines();
        Debug.Log($"Гравитация сброшена: {Physics2D.gravity.y}");
       /* currentGravityMultiplier = 1f;
        targetGravityMultiplier = 1f;
        previousGravityMultiplier = 1f;
        gravityUpgradeCount = 0;
        isGravityEffectActive = false;
        rechargeTimer = 0f;
        if (rb != null) rb.gravityScale = currentGravityMultiplier;
        StopAllCoroutines();
        Debug.Log($"Гравитация сброшена: {rb?.gravityScale ?? 0f}");*/
    }
    public float UpdateVariable(float Variable, float Percent)
    {
        return (Variable * Percent) / 100;
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
            if (wheelJoints == null)
            {
                return;
            }
            var suspension = wheelJoint.suspension;
            suspension.frequency = baseFrequency * currentGripMultiplier;
            wheelJoint.suspension = suspension;
            var motor = wheelJoint.motor;
            motor.maxMotorTorque = baseMotorTorque * currentGripMultiplier;
            wheelJoint.motor = motor;
        }
    }
}
