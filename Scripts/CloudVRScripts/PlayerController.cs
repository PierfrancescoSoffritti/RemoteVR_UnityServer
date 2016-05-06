using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(InputManager))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float m_WalkSpeed;
    [SerializeField]
    private float m_StickToGroundForce;
    [SerializeField]
    private float m_GravityMultiplier;

    private Vector3 m_MoveDir = Vector3.zero;
    private CharacterController m_CharacterController;

    public InputManager inputManager;

    // Use this for initialization
    private void Start()
    {
        m_CharacterController = GetComponent<CharacterController>();
    }

    private void FixedUpdate()
    {
        float speed;
        GetInput(out speed);

        // always move along the camera forward as it is the direction that it being aimed at
        Vector3 desiredMove = transform.forward;

        // get a normal for the surface that is being touched to move along it
        RaycastHit hitInfo;
        Physics.SphereCast(transform.position, m_CharacterController.radius, Vector3.down, out hitInfo, m_CharacterController.height / 2f, ~0, QueryTriggerInteraction.Ignore);
        desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

        m_MoveDir.x = desiredMove.x * speed;
        m_MoveDir.z = desiredMove.z * speed;

        m_CharacterController.Move(m_MoveDir * Time.fixedDeltaTime);
    }

    private void GetInput(out float speed)
    {
        if (inputManager.Move)
            speed = m_WalkSpeed;
        else
            speed = 0;
    }
}
