using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody Rb;
    public int speed = 35;
    public float rotationSpeed = 650f;
    private float horizontal;
    private float vertical;
    public int jumpForce = 100;
    private bool isGrounded = false;
    private void Start()
    {
        Rb = GetComponent<Rigidbody>();
    }
    void Update()
    {
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            Rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
        }
        float mouseHorizontal = Input.GetAxis("Mouse X");
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime * mouseHorizontal);
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(Vector3.forward * Time.deltaTime * speed);
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(Vector3.left * Time.deltaTime * speed);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(Vector3.back * Time.deltaTime * speed);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(Vector3.right * Time.deltaTime * speed);
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            Debug.Log("Ты на земле");
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
