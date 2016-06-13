using UnityEngine;

/// <summary>
/// Class used to update the rotation and position of the player.
/// </summary>
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(RemoteInputManager))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float m_WalkSpeed;
    [SerializeField]
    private float m_StickToGroundForce;
    [SerializeField]
    private float m_GravityMultiplier;

    private Vector3 m_MoveDir = Vector3.zero;
    private Quaternion targetInitialRotation;

    private GameObject target;

    private CharacterController m_CharacterController;

    public RemoteInputManager inputManager;

    private void Start()
    {
        m_CharacterController = GetComponent<CharacterController>();
    }

    /// <summary>
    /// Call this method when creating the <see cref="Player"/>
    /// </summary>
    public void init(GameObject target, RemoteInputManager inputManager)
    {
        this.target = target;
        this.inputManager = inputManager;

        targetInitialRotation = target.transform.rotation;
    }

    private void FixedUpdate()
    {
        if (target == null)
            return;

        updateRotation();
        updatePosition();
    }

    /// <summary>
    /// Updates the rotation of the target
    /// </summary>
    private void updateRotation()
    {
        target.transform.rotation = targetInitialRotation * inputManager.Rotation;
    }

    /// <summary>
    /// Updates the position of the target
    /// </summary>
    private void updatePosition()
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
        if (inputManager.TouchDown)
            speed = m_WalkSpeed;
        else
            speed = 0;
    }
}
