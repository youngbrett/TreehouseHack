using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicController : MonoBehaviour
{
/// <summary>
///
/// A Basic Character Controller.
///
/// Brett Young
/// 5/25/2019
///
/// </summary>

[Range(0, 20)]
public float speed = 10.0f;
[Range(0, 20)]
public float xSensitivity = 5f;
[Range(0, 20)]
public float ySensitivity = 5f;
[Range(0, 20)]
public float jumpForce = 5f;
[SerializeField]
private Transform playerCamera;
private float translation;
private float strafe;
private float yRot;
private float xRot;
private Rigidbody rigid;
private bool canJump = true;

// Use this for initialization
    void Start()
    {
    // turn off the cursor
    Cursor.lockState = CursorLockMode.Locked;
    Cursor.visible = false;

    rigid = GetComponent<Rigidbody>();
    playerCamera = GameObject.Find("Main Camera").transform;
    }

    // Update is called once per frame
    void Update()
    {

        if (!Cursor.visible)
        {
            LookRotation();
            Movement();
            Jumping();
        }

        if (Input.GetKeyDown("escape"))
        {
                if (Cursor.visible)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
                else
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
        }
    }

    void Movement()
    {

        translation = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        strafe = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        playerCamera.Translate(strafe, 0, translation);

    }

    void LookRotation()
    {
        yRot += Input.GetAxis("Mouse X") * ySensitivity;
        xRot += Input.GetAxis("Mouse Y") * xSensitivity;
        xRot = Mathf.Clamp(xRot, -45, 45);

            // playerCamera.localEulerAngles = new Vector3(0f, yRot, 0f);
        playerCamera.localEulerAngles = new Vector3(-xRot, yRot, 0f);

    }

    void Jumping()
    {
        if (Input.GetButtonDown("Jump") && canJump)
            rigid.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    void OnCollisionStay(Collision other)
    {
        canJump = true;
    }

    void OnCollisionExit(Collision other)
    {

        canJump = false;
    }
}
