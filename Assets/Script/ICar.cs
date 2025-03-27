using UnityEngine;

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
    float maxFuel { get; }
    float Expenditure { get; }
    float refillAmount { get; }
    float currentFuel { get; }
    bool isOutOfFuel { get; }
    int initialMoney { get; }
    int maxMoney { get; }
    int currentMoney { get; }
    void StartMoveCar();
    void UpdateCarMove();
    void FixUpdateMoveCar();
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
}
