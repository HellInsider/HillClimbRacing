using UnityEngine;
using UnityEngine.Windows;

public interface ICar
{
    float engineForce { get; }
    float maxSpeed { get; }
    float rotationSpeed { get; }
    WheelJoint2D[] wheelJoints { get; }
    Transform centerOfMass { get; }
    bool checkfuel { get; }
    Rigidbody2D rb { get; }
    float moveInput { get; }
    bool isMobileInput { get; }
    float maxFuel { get; }
    float Expenditure { get; }
    float refillAmount { get; }
    float currentFuel { get; }
    bool isOutOfFuel { get; }
    int initialMoney { get; }
    //int maxMoney { get; }
    int currentMoney { get; }
    float torqueInput { get; }
    float NInputTorque { get; }
    float PInputTorque { get; }
    float baseScale { get; }
    float upgradeMultiplier { get; }
    int maxUpgrades { get; }
    bool affectPhysics { get; }
    int currentUpgrades { get; }
    float currentScale { get; }
    Transform[] wheelVisuals { get; }
    void EnableControl(bool state);
    void StartMoveCar();
    void UpdateCarMove();
    void FixUpdateMoveCar();
    void SetMoveInput(float input);
    float GetMoveInput();
    float GetCurrentSpeed();
    void StopCar();
    void StartFuel();
    void UpdateFuel();
    bool IsCarMoving();
    void OutOfFuel();
    void RefillFuel();
    float GetCurrentFuel();
    float GetMaxFuel();
    void StartMoney();
    void AddMoney(int amount);
    int GetCurrentMoney();
    void UpdateMoneyUI();
    void StartWheel();
    void UpgradeWheels();
    void ApplyWheelScale();
    void UpdateWheelPhysics(WheelJoint2D wheel);
    void ResetUpgrades();
}
