using UnityEngine;

public class Car : MonoBehaviour, ICar
{
    [Header("Movement Settings")]
    [SerializeField] public float _engineForce;
    [SerializeField] public float _maxSpeed;
    [SerializeField] public float _rotationSpeed;
    [Header("Money")]
    [SerializeField] public int _initialMoney = 0;
    [SerializeField] public int _maxMoney = 999999;
    [Header("Fuel")]
    [SerializeField] public float _maxFuel;
    [SerializeField] public float _Expenditure;
    [SerializeField] public float _refillAmount;
    private int _currentMoney;
    private float _currentFuel;
    private bool _isOutOfFuel = false;
    public WheelJoint2D[] _wheelJoints;
    public Transform _centerOfMass;
    public bool _checkfuel = true;
    private Rigidbody2D _rb;
    private float _moveInput;
    public float engineForce => _engineForce;
    public float maxSpeed => _maxSpeed;
    public float rotationSpeed => _rotationSpeed;
    public WheelJoint2D[] wheelJoints => _wheelJoints;
    public Transform centerOfMass => _centerOfMass;
    public bool checkfuel => _checkfuel;
    public Rigidbody2D rb => _rb;
    public float moveInput => _moveInput;
    public float maxFuel => _maxFuel;
    public float Expenditure => _Expenditure;
    public float refillAmount => _refillAmount;
    public float currentFuel => _currentFuel;
    public bool isOutOfFuel => _isOutOfFuel;
    public int initialMoney => _initialMoney;
    public int maxMoney => _maxMoney;
    public int currentMoney => _currentMoney;
    //private float torqueInput;
    void Start()
    {
        StartMoveCar();
        StartMoney();
        StartFuel();
        
    }
    void Update()
    {
        UpdateCarMove();
        UpdateFuel();
        UpdateMoneyUI();
    }
    private void FixedUpdate()
    {
        FixUpdateMoveCar();
    }
    public void StartMoveCar()
    {
        _rb = GetComponent<Rigidbody2D>();
        if (_centerOfMass != null)
        {
            _rb.centerOfMass = _centerOfMass.localPosition;
        }
    }
    public void UpdateCarMove()
    {
        if (_checkfuel) _moveInput = Input.GetAxis("Horizontal");
        else _moveInput = 0;
    }
    public void FixUpdateMoveCar()
    {
        foreach (var wheelJoint in _wheelJoints)
        {
            if (wheelJoint != null)
            {
                JointMotor2D motor = wheelJoint.motor;
                motor.motorSpeed = _moveInput * _engineForce;
                motor.maxMotorTorque = 10000;
                wheelJoint.motor = motor;
                wheelJoint.useMotor = _moveInput != 0;
            }
        }
        if (_rb.linearVelocity.magnitude > _maxSpeed)
        {
            _rb.linearVelocity = _rb.linearVelocity.normalized * _maxSpeed;
        }
        /*if (torqueInput != 0)
        {
            rb.AddTorque(torqueInput * rotationSpeed);
        }*/
    }
    public float GetMoveInput()
    {
        return _moveInput;
    }
    public float GetCurrentSpeed()
    {
        return _rb.linearVelocity.magnitude;
    }
    public void StopCar()
    {
        foreach (var wheelJoint in _wheelJoints)
        {
            if (wheelJoint != null)
            {
                JointMotor2D motor = wheelJoint.motor;
                motor.motorSpeed = 0;
                motor.maxMotorTorque = 0;
                wheelJoint.motor = motor;
                wheelJoint.useMotor = false;
                _checkfuel = false;
            }
        }
        _rb.linearVelocity = Vector2.zero;
        _rb.angularVelocity = 0f;
    }
    public void StartFuel()
    {
        _currentFuel = _maxFuel;
        //_carController = GetComponent<MoveCar>();
    }
    public void UpdateFuel()
    {
        if (_isOutOfFuel) return;
        if (IsCarMoving())
        {
            _currentFuel -= _Expenditure * Time.deltaTime;
            _currentFuel = Mathf.Max(_currentFuel, 0);

            if (_currentFuel <= 0)
            {
                OutOfFuel();
            }
        }
    }
    public bool IsCarMoving()
    {
        return Mathf.Abs(GetMoveInput()) > 0.1f || GetCurrentSpeed() > 0.1f;
    }
    public void OutOfFuel()
    {
        _isOutOfFuel = true;
        Debug.Log("Топливо закончилось!");
        StopCar();
        _checkfuel = false;

    }
    public void RefillFuel()
    {
        _checkfuel = true;
        _currentFuel += _refillAmount;
        _currentFuel = Mathf.Min(_currentFuel, _maxFuel);
        _isOutOfFuel = false;
        Debug.Log("Заправлено! Текущее топливо: " + _currentFuel);
    }
    public float GetCurrentFuel()
    {
        return _currentFuel;
    }
    public float GetMaxFuel()
    {
        return _maxFuel;
    }
    public void StartMoney()
    {
        _currentMoney = initialMoney;
        UpdateMoneyUI();
    }
    public void AddMoney(int amount)
    {
        if (amount < 0) return;

        _currentMoney += amount;
        _currentMoney = Mathf.Min(currentMoney, maxMoney);
        Debug.Log($"Добавлено {amount} денег. Текущий баланс: {currentMoney}");
        UpdateMoneyUI();
    }
    public int GetCurrentMoney()
    {
        return currentMoney;
    }
    public void UpdateMoneyUI()
    {

    }
}
