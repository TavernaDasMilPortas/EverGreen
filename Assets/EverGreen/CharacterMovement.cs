using UnityEngine;
using KinematicCharacterController;

[RequireComponent(typeof(KinematicCharacterMotor))]

public class CharacterMovement : MonoBehaviour, ICharacterController
{
    public KinematicCharacterMotor motor;
    public float gravity = 30;
    public float airMaxSpeed = 3;
    public float acceleration = 50;
    public float airAcceleration;
    public float drag;
    private Vector3 moveInput;


    public float maxSpeed;
    public float rotationSpeed = 10;

    private void Awake()
    {
    
     motor.CharacterController = this;
        
    }


    private void Inputs()
    {
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");

        moveInput = new Vector3(moveHorizontal, 0, moveVertical);
        moveInput = transform.rotation * moveInput;
        moveInput.y = 0;
        moveInput.Normalize();

    }
    public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
    {
        if (moveInput != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveInput);
            currentRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
    /// <summary>
    /// This is called when the motor wants to know what its velocity should be right now
    /// </summary>
    public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        Inputs();

        if (motor.GroundingStatus.IsStableOnGround)
        {
            // Calcula velocidade do player andando.

            Vector3 targetVelocity = moveInput * maxSpeed;
            currentVelocity = Vector3.MoveTowards(currentVelocity, targetVelocity, acceleration * deltaTime);
            

        }
        else // Se o player estiver no ar.
        {
            Vector2 targetVelocityXZ = new Vector2(moveInput.x, moveInput.z) * airMaxSpeed;
            Vector2 currentVelocityXZ = new Vector2(currentVelocity.x, currentVelocity.z);

            currentVelocityXZ = Vector2.MoveTowards(currentVelocityXZ, targetVelocityXZ, airAcceleration * deltaTime);

            currentVelocity.x = ApplyDrag(currentVelocityXZ.x, drag, deltaTime);
            currentVelocity.z = ApplyDrag(currentVelocityXZ.y, drag, deltaTime);
            currentVelocity.y -= gravity * deltaTime;
        }

    }
   
    private static float ApplyDrag(float velocity, float drag, float deltaTime)
    {
        return velocity * (1 / (1 + drag * deltaTime));
    }

    #region not implemented
    /// <summary>
    /// This is called before the motor does anything
    /// </summary>
    public void BeforeCharacterUpdate(float deltaTime)
    {

    }
    /// <summary>
    /// This is called after the motor has finished its ground probing, but before PhysicsMover/Velocity/etc.... handling
    /// </summary>
    public void PostGroundingUpdate(float deltaTime)
    {

    }
    /// <summary>
    /// This is called after the motor has finished everything in its update
    /// </summary>
    public void AfterCharacterUpdate(float deltaTime)
    {

    }
    /// <summary>
    /// This is called after when the motor wants to know if the collider can be collided with (or if we just go through it)
    /// </summary>
    public bool IsColliderValidForCollisions(Collider coll)
    {
        return true;
    }
    /// <summary>
    /// This is called when the motor's ground probing detects a ground hit
    /// </summary>
    public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {

    }
    /// <summary>
    /// This is called when the motor's movement logic detects a hit
    /// </summary>
    public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {

    }
    /// <summary>
    /// This is called after every move hit, to give you an opportunity to modify the HitStabilityReport to your liking
    /// </summary>
    public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
    {

    }
    /// <summary>
    /// This is called when the character detects discrete collisions (collisions that don't result from the motor's capsuleCasts when moving)
    /// </summary>
    public void OnDiscreteCollisionDetected(Collider hitCollider)
    {

    }

    #endregion
}