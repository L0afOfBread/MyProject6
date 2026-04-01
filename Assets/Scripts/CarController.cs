using UnityEngine;

public class CarController : MonoBehaviour
{
    public float speed = 1500f;    // сила движения
    public float torque = 600f;    // сила поворота  
    public float maxSpeed = 25f;   // максимальная скорость
    public float turnSpeed = 40f;  // скорость поворота
    public float downForce = 300f; // прижимная сила
    public float groundCheckDistance = 1f; // дистанция проверки земли
    private Rigidbody rb;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        // настраиваем физику для стабильного управления
        rb.mass = 1000f; // немного уменьшаем массу для лучшей управляемости
        rb.drag = 1.5f; // сопротивление воздуха
        rb.angularDrag = 1.5f; // сопротивление вращению
        rb.centerOfMass = new Vector3(0, -0.3f, 0); // центр масс
        rb.useGravity = true;
        
        // добавляем физический коллайдер если его нет
        if (GetComponent<BoxCollider>() == null && GetComponent<MeshCollider>() == null)
        {
            BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
            boxCollider.center = new Vector3(0, 0.5f, 0);
            boxCollider.size = new Vector3(2f, 1f, 4f);
        }
    }

    void FixedUpdate()
    {
        // проверяем находимся ли на земле
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance);
        
        // получаем ввод (WASD или стрелки)
        float moveInput = Input.GetAxis("Vertical");   // W/S или стрелки вверх/вниз
        float turnInput = Input.GetAxis("Horizontal"); // A/D или стрелки влево/вправо

        // прижимная сила только когда на земле
        if (isGrounded)
        {
            rb.AddForce(-transform.up * downForce * Time.fixedDeltaTime);
            
            // движение только на земле
            if (Mathf.Abs(moveInput) > 0.1f)
            {
                Vector3 forwardForce = transform.forward * moveInput * speed * Time.fixedDeltaTime;
                rb.AddForce(forwardForce, ForceMode.Force);
            }
            else
            {
                // плавное торможение на земле
                rb.velocity *= 0.98f;
                rb.angularVelocity *= 0.95f;
            }

            // поворот только на земле и при движении
            if (Mathf.Abs(turnInput) > 0.1f && rb.velocity.magnitude > 1f)
            {
                float speedFactor = Mathf.Clamp01(rb.velocity.magnitude / maxSpeed);
                float actualTurnSpeed = turnSpeed * (1f - speedFactor * 0.6f);
                
                Vector3 turnTorque = Vector3.up * turnInput * actualTurnSpeed * Time.fixedDeltaTime;
                rb.AddTorque(turnTorque, ForceMode.Force);
            }
        }
        else
        {
            // в воздухе уменьшаем управление
            rb.angularVelocity *= 0.9f;
        }

        // ограничение максимальной скорости
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }

        // стабилизация против опрокидывания
        if (transform.up.y < 0.95f)
        {
            rb.AddForce(Vector3.up * 150f, ForceMode.Force);
            rb.angularVelocity *= 0.8f;
        }

        // предотвращение взлета
        if (rb.velocity.y > 3f)
        {
            rb.velocity = new Vector3(rb.velocity.x, 3f, rb.velocity.z);
        }
    }
}
