using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 15.0f;
    public float mouseSensitivity = 2.0f;
    public float verticalRotationLimit = 90.0f;
    public float gravity = -40.0f;
    public float jumpForce = 20.0f;

    private GameManager gameManager;
    private CharacterController controller;
    private float xRotation = 0;
    private float yRotation = 0;
    private Vector3 velocity;

    // Start is called before the first frame update
    void Start()
    {
        GameObject gameManagerObject = GameObject.Find("Game Manager");

        if (gameManagerObject != null)
        {
            gameManager = gameManagerObject.GetComponent<GameManager>();
        }

        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        float moveDirectionX = Input.GetAxis("Horizontal");
        float moveDirectionZ = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(moveDirectionX, 0, moveDirectionZ);
        move = transform.TransformDirection(move);

        velocity.x = move.x * speed;
        velocity.z = move.z * speed;

        if (controller.isGrounded)
        {
            if (Input.GetButtonDown("Jump"))
            {
                velocity.y = jumpForce;
                speed = 15;
            }

            else
            {
                if (move.magnitude > 0)
                {
                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        speed = 30;
                    }
                    else
                    {
                        speed = 15;
                    }
                }
                else
                {
                    speed = 15;
                }
            }

        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
            speed = 15;
        }

        controller.Move(velocity * Time.deltaTime);

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -verticalRotationLimit, verticalRotationLimit);
        yRotation += mouseX;

        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0);
    }
}
