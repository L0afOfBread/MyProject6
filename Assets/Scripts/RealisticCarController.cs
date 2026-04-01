using UnityEngine;

public class RealisticCarController : MonoBehaviour
{
    [Header("Настройки двигателя")]
    public float maxSpeed = 60f;           // Максимальная скорость (км/ч)
    public float acceleration = 15f;       // Ускорение
    public float reverseSpeed = 20f;       // Скорость заднего хода
    public float brakeForce = 30f;          // Сила торможения
    
    [Header("Настройки поворотов")]
    public float turnSpeed = 80f;           // Скорость поворота
    public float turnRadius = 5f;           // Радиус поворота
    public float driftFactor = 0.95f;       // Коэффициент заноса (0.9-1.0)
    
    [Header("Физика")]
    public float downForce = 100f;          // Прижимная сила
    public float dragCoefficient = 0.3f;    // Аэродинамическое сопротивление
    public float friction = 5f;             // Трение с дорогой
    
    [Header("Колеса (опционально)")]
    public Transform[] wheelMeshes;         // Визуальные модели колес
    public WheelCollider[] wheelColliders;  // Физические колеса
    
    private Rigidbody rb;
    private float currentSpeed;
    private float steeringAngle;
    private bool isBraking;
    private float motorTorque;
    private float brakeTorque;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        // Настройка физики
        rb.drag = dragCoefficient;
        rb.angularDrag = dragCoefficient * 0.5f;
        rb.centerOfMass = new Vector3(0, -0.5f, 0); // Снижаем центр масс
        
        // Если колеса не назначены, пытаемся найти их автоматически
        if (wheelColliders == null || wheelColliders.Length == 0)
        {
            wheelColliders = GetComponentsInChildren<WheelCollider>();
        }
    }
    
    void FixedUpdate()
    {
        HandleInput();
        UpdatePhysics();
        UpdateWheelVisuals();
    }
    
    void HandleInput()
    {
        // Получаем ввод
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        // Рассчитываем текущую скорость
        currentSpeed = rb.velocity.magnitude * 3.6f; // Конвертируем в км/ч
        
        // Управление газом/тормозом
        if (vertical > 0)
        {
            // Движение вперед
            motorTorque = vertical * acceleration * 100f;
            brakeTorque = 0f;
            isBraking = false;
        }
        else if (vertical < 0)
        {
            if (currentSpeed > 1f)
            {
                // Торможение при движении вперед
                motorTorque = 0f;
                brakeTorque = brakeForce * 100f;
                isBraking = true;
            }
            else
            {
                // Задний ход
                motorTorque = vertical * reverseSpeed * 100f;
                brakeTorque = 0f;
                isBraking = false;
            }
        }
        else
        {
            // Нейтральная передача - естественное замедление
            motorTorque = 0f;
            brakeTorque = friction * 10f;
            isBraking = false;
        }
        
        // Управление рулем
        if (Mathf.Abs(horizontal) > 0.1f)
        {
            // Уменьшаем угол поворота на высокой скорости
            float speedFactor = Mathf.Clamp01(currentSpeed / maxSpeed);
            steeringAngle = horizontal * turnSpeed * (1f - speedFactor * 0.7f);
        }
        else
        {
            steeringAngle = 0f;
        }
    }
    
    void UpdatePhysics()
    {
        // Прижимная сила для лучшего сцепления
        rb.AddForce(-transform.up * downForce * rb.velocity.magnitude / maxSpeed);
        
        // Если есть WheelColliders, используем их
        if (wheelColliders != null && wheelColliders.Length > 0)
        {
            ApplyWheelPhysics();
        }
        else
        {
            // Простое физическое управление
            ApplySimplePhysics();
        }
    }
    
    void ApplyWheelPhysics()
    {
        // Передние колеса - рулевые
        if (wheelColliders.Length >= 2)
        {
            wheelColliders[0].steerAngle = steeringAngle;
            wheelColliders[1].steerAngle = steeringAngle;
        }
        
        // Задние колеса - ведущие
        if (wheelColliders.Length >= 4)
        {
            wheelColliders[2].motorTorque = motorTorque;
            wheelColliders[3].motorTorque = motorTorque;
            wheelColliders[2].brakeTorque = brakeTorque;
            wheelColliders[3].brakeTorque = brakeTorque;
        }
        
        // Применяем тормоза ко всем колесам
        foreach (var wheel in wheelColliders)
        {
            wheel.brakeTorque = brakeTorque;
        }
    }
    
    void ApplySimplePhysics()
    {
        // Движение вперед/назад
        if (motorTorque != 0)
        {
            Vector3 forwardForce = transform.forward * motorTorque * Time.fixedDeltaTime;
            rb.AddForce(forwardForce, ForceMode.Force);
        }
        
        // Поворот через изменение направления
        if (Mathf.Abs(steeringAngle) > 0.1f && rb.velocity.magnitude > 0.5f)
        {
            Vector3 turnForce = transform.up * steeringAngle * Time.fixedDeltaTime;
            rb.AddTorque(turnForce, ForceMode.VelocityChange);
            
            // Имитация заноса
            Vector3 lateralVelocity = transform.right * Vector3.Dot(rb.velocity, transform.right);
            rb.AddForce(-lateralVelocity * (1f - driftFactor), ForceMode.Force);
        }
        
        // Ограничение максимальной скорости
        if (rb.velocity.magnitude > maxSpeed / 3.6f)
        {
            rb.velocity = rb.velocity.normalized * (maxSpeed / 3.6f);
        }
    }
    
    void UpdateWheelVisuals()
    {
        if (wheelMeshes == null || wheelColliders == null || wheelMeshes.Length != wheelColliders.Length)
            return;
            
        for (int i = 0; i < wheelMeshes.Length; i++)
        {
            if (wheelMeshes[i] != null && wheelColliders[i] != null)
            {
                wheelColliders[i].GetWorldPose(out Vector3 position, out Quaternion rotation);
                wheelMeshes[i].position = position;
                wheelMeshes[i].rotation = rotation;
            }
        }
    }
    
    // Геттеры для UI или других скриптов
    public float GetCurrentSpeed() => currentSpeed;
    public float GetMaxSpeed() => maxSpeed;
    public bool IsBraking() => isBraking;
    
    // Визуализация в редакторе
    void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            // Показываем скорость
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, 2f);
            
            // Показываем направление движения
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + transform.forward * 3f);
        }
    }
}
