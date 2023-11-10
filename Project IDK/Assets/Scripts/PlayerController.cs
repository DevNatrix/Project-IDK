using Cinemachine;
using UnityEngine;
using UnityEngine.Experimental.AI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float playerSpeed = 2.0f;
    [SerializeField] private float jumpHeight = 1.0f;
    [SerializeField] private float gravityValue = -9.81f;
    [SerializeField] private float rotationSpeed = 0.9f;
    [SerializeField] private float sensitivity = 5f;

    private CharacterController controller;
    private PlayerInput playerInput;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private Transform cameraTransform;

    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction sprintAction;

    public Animator animator;
    [SerializeField] private Camera cam;
    public CinemachineVirtualCamera VirtualCamera;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        cameraTransform = cam.transform;
        moveAction = playerInput.actions["Move"];
        lookAction = playerInput.actions["Look"];
        sprintAction = playerInput.actions["Sprint"];
    }

    void Update()
    {
        sprintAction.started += ctx => Sprint(ctx);
        sprintAction.canceled += ctx => Sprint(ctx);

        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        Vector2 input = moveAction.ReadValue<Vector2>();
        Vector3 move = new Vector3(input.x, 0, input.y);
        move = move.x * cameraTransform.right.normalized + move.z * cameraTransform.forward.normalized;
        move.y = 0f;
        controller.Move(move * Time.deltaTime * playerSpeed);

        if (move != Vector3.zero)
        {
            Vector3 targetRotation = new Vector3(move.x, move.y, move.z);
            transform.forward = Vector3.Lerp(transform.forward, targetRotation, rotationSpeed * Time.deltaTime);
            animator.SetBool("Walking", true);
        }
        else if (move == Vector3.zero)
        {
            animator.SetBool("Walking", false);
        }

        // Changes the height position of the player..
        if (Input.GetButtonDown("Jump") && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        VirtualCamera.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.m_MaxSpeed = sensitivity;
        VirtualCamera.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.m_MaxSpeed = sensitivity;
    }

    public void Sprint(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            playerSpeed = 5f;
        }
        else if(context.canceled)
        {
            playerSpeed = 2f;
        }
    }
}