using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private Rigidbody Rb;

    [Header("Настройки движения")]
    public float speed = 15f; // Для MovePosition значения обычно меньше, чем для Translate
    public float rotationSpeed = 650f;
    public float jumpForce = 5f;

    [Header("Состояние")]
    private float horizontal;
    private float vertical;
    private bool isGrounded = false;

    public static float playerHp = 100f;
    private Animator anim;

    public GameObject deathWindow;

    void Start()
    {
        Rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>(); // Получаем компонент

        Rb.freezeRotation = true;
        Rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    void Update()
    {
        if (playerHp <= 0)
        {
            deathWindow.SetActive(true);
            Time.timeScale = 0f;
        }

        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        // Проверяем, есть ли движение
        bool isMoving = horizontal != 0 || vertical != 0;

        // Мгновенно передаем состояние в аниматор
        anim.SetBool("isMoving", isMoving);
        float mouseHorizontal = Input.GetAxis("Mouse X");

        // Поворот персонажа мышью
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime * mouseHorizontal);

        // Прыжок
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            Rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
        }
    }

    void FixedUpdate()
    {
        // Рассчитываем направление движения относительно того, куда смотрит персонаж
        Vector3 moveDirection = (transform.forward * vertical + transform.right * horizontal).normalized;

        // Физическое перемещение без "вталкивания" в стены
        Rb.MovePosition(Rb.position + moveDirection * speed * Time.fixedDeltaTime);
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}