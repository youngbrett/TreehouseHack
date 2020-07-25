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

    public Vector3 ScreenSpace;
    private Ray CameraRay;
    private float HitDist;
    private Vector3 HitPoint;
    public RaycastHit HitInfo;
    public int EndPointMask;
    private GameObject target;
    private bool isMouseDragging;
    private Vector3 screenPosition;
    private Vector3 offset;
    private bool selectOn = false;




    void Start()
    {
        // turn off the cursor
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
        selectOn = false;
        isMouseDragging = false;

        rigid = GetComponent<Rigidbody>();
        playerCamera = GameObject.Find("Main Camera").transform;

        EndPointMask = 1 << 8;            //  LayerMasks are tricky.  Makes our raycast only hit the layer identified by the integer in the definition.
    }

    // Update is called once per frame
    void Update()
    {
        CameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);      //  Shoot a ray from the camera to the xbox cursor position.

        if (!selectOn)
        {
            LookRotation();
            Movement();
            Jumping();
        }

        if (Input.GetKeyDown("escape"))
        {
            if (!selectOn)
            {
                //Cursor.lockState = CursorLockMode.Locked;
                // Cursor.visible = false;
            }
            else
            {
                // Cursor.lockState = CursorLockMode.None;
                // Cursor.visible = true;
            }

            selectOn = !selectOn;
        }

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hitInfo;
            target = ReturnClickedObject(out hitInfo);
            if (target != null)
            {
                isMouseDragging = true;
                Debug.Log("our target position :" + target.transform.position);
                //Here we Convert world position to screen position.
                screenPosition = Camera.main.WorldToScreenPoint(target.transform.position);
                offset = target.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPosition.z));
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isMouseDragging = false;
        }

        if (isMouseDragging)
        {
            //tracking mouse position.
            Vector3 currentScreenSpace = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPosition.z);

            //convert screen position to world position with offset changes.
            Vector3 currentPosition = Camera.main.ScreenToWorldPoint(currentScreenSpace) + offset;

            //It will update target gameobject's current postion.
            target.transform.position = new Vector3(currentPosition.x, target.transform.position.y, currentPosition.z);
        }

    }
    GameObject ReturnClickedObject(out RaycastHit hit)
    {
        GameObject targetObject = null;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray.origin, ray.direction * 100, out hit, Mathf.Infinity, EndPointMask))
        {
                targetObject = hit.collider.gameObject;
        }
        return targetObject;
    }

    
    void Movement()
    {
        translation = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        strafe = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        //playerCamera.Translate(strafe, 0, translation);
        playerCamera.position += strafe * playerCamera.right + translation * new Vector3(playerCamera.forward.x, 0, playerCamera.forward.z);

        if (Input.GetKey("q"))
        {
            playerCamera.position -= new Vector3(0, playerCamera.position.y * speed / 50f * Time.deltaTime, 0);
        }
        if (Input.GetKey("e"))
        {
            playerCamera.position += new Vector3(0, playerCamera.position.y * speed / 50f * Time.deltaTime, 0);
        }
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
    /*
  void UpdatePosition(GameObject movedGO)
  {


      /*
      if (posAdjust.objPositions.ContainsKey(movedGO.name))
      {
          posAdjust.objPositions[movedGO.name] = movedGO.transform.position;
      }
      else
      {
          posAdjust.objPositions.Add(movedGO.name, movedGO.transform.position);
      }

     

}


     */
}





