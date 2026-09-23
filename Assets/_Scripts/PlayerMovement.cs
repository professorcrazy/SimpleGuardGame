using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody rb;
    Camera cam;

    [SerializeField] float speed = 5f;
    [SerializeField] float mouseLookDeadzone = 0.25f;
    [SerializeField] float mouseMoveDeadzonePixels = 1f;

    Vector3 moveDir;
    Vector3 lookForward;

    Vector2 lastMousePosition;
    bool hasMousePosition;

    public void Move(InputAction.CallbackContext context)
    {
        moveDir = context.ReadValue<Vector2>();

        if (moveDir.sqrMagnitude > 1)
        {
            moveDir.Normalize();
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        cam = Camera.main;

        lookForward = transform.forward;
        lookForward.y = 0f;
        lookForward.Normalize();
    }

    void FixedUpdate()
    {
        PlayerLook();
        Movement();
    }

    void Movement()
    {
        Vector3 forward = lookForward;

        // Højre/venstre bruger spillerens egen right direction
        Vector3 right = transform.right;
        right.y = 0f;
        right.Normalize();

        Vector3 move = right * moveDir.x + forward * moveDir.y;

        rb.linearVelocity = new Vector3(
            move.x * speed,
            rb.linearVelocity.y,
            move.z * speed
        );
    }

    void PlayerLook()
    {
        Vector2 currentMousePosition = Mouse.current.position.ReadValue();

        if (hasMousePosition)
        {
            float mouseMoved = (currentMousePosition - lastMousePosition).sqrMagnitude;

            if (mouseMoved < mouseMoveDeadzonePixels * mouseMoveDeadzonePixels)
            {
                return;
            }
        }

        lastMousePosition = currentMousePosition;
        hasMousePosition = true;

        Ray ray = cam.ScreenPointToRay(currentMousePosition);
        Plane groundPlane = new Plane(Vector3.up, transform.position);

        if (groundPlane.Raycast(ray, out float distance))
        {
            Vector3 mouseWorld = ray.GetPoint(distance);
            Vector3 lookDir = mouseWorld - transform.position;
            lookDir.y = 0f;

            if (lookDir.sqrMagnitude > mouseLookDeadzone * mouseLookDeadzone)
            {
                lookForward = lookDir.normalized;

                Quaternion targetRotation = Quaternion.LookRotation(lookForward);
                transform.rotation = targetRotation;
            }
        }
    }
}