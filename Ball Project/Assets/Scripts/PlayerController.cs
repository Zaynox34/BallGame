using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{

    public Vector3 move;

    [SerializeField]
    private InputActionReference moveControl;
    [SerializeField]
    private InputActionReference jumpControl;
    [SerializeField]
    private float playerSpeed = 2.0f;
    [SerializeField]
    private float jumpHeight = 20.0f;
    [SerializeField]
    private float gravityValue = -9.81f;
    [SerializeField]
    private float rotationSpeed = 200f;
    [SerializeField]
    private float jumpHeightPower = 0f;


    private CharacterController controller;

    public Vector3 playerVelocity;
    [SerializeField]
    private bool groundedPlayer;
    private Transform cameraMainTransform;

    private void OnEnable()
    {
        moveControl.action.Enable();
        jumpControl.action.Enable();
    }
    private void OnDisable()
    {
        moveControl.action.Disable();
        jumpControl.action.Disable();
    }
    private void Start()
    {
        controller = gameObject.GetComponent<CharacterController>();
        cameraMainTransform = Camera.main.transform;
        Application.targetFrameRate = 60;
    }

    void Update()
    {
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            
            playerVelocity.y = 0f;
        }
        Vector2 movement = moveControl.action.ReadValue<Vector2>();
;       move = new Vector3(movement.x, 0, movement.y);
        move = cameraMainTransform.forward * move.z + cameraMainTransform.right * move.x;
        move.y = 0f;
        controller.Move(move * Time.deltaTime * playerSpeed);


        // Changes the height position of the player..
        if (jumpControl.action.triggered && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
            jumpHeightPower = jumpHeight;
            Debug.Log(playerVelocity.y);
        }

        if (jumpControl.action.triggered )
        {
            Debug.Log('Z');
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
        if(movement!=Vector2.zero)
        {
            float targetAngle = Mathf.Atan2(movement.x, movement.y)*Mathf.Rad2Deg +cameraMainTransform.eulerAngles.y;
            Quaternion rotation = Quaternion.Euler(0f,targetAngle,0f);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);

        }
        transform.GetChild(0).forward = (playerVelocity +move).normalized;
        float intensite = (playerVelocity + move).magnitude;
        //Debug.Log(intensite);
        if (!groundedPlayer)
        {
            transform.GetChild(0).localScale = new Vector3(2 - (intensite / 30 + 1), 2 - (intensite / 30 + 1), intensite / 30 + 1);
        }
        else
        {
            if(playerVelocity.magnitude<0.2f)
            {
                transform.GetChild(0).localScale = Vector3.one;
            }
            else
            {
                transform.GetChild(0).localScale = new Vector3(intensite / 30 + 1, intensite / 30 + 1, 2 - (intensite / 30 + 1));
            }
        }
            

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            jumpHeightPower *= 0.8f;
            playerVelocity.y = Mathf.Sqrt(jumpHeightPower * -3.0f * gravityValue);
            //Debug.Log(other +"  "+playerVelocity.y);
        }
    }
}

