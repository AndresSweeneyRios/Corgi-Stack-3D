#nullable enable

using System;
using UnityEngine;

public class CorgiMove : MonoBehaviour {
    private const float MAX_PITCH = 80f;
    private const float MIN_PITCH = 0f;
    private const float GRAB_DISTANCE = 0.5f;
    private const float ROTATION_EPSILON = 0.5f;
    private static readonly Vector3 CAMERA_OFFSET = new(0f, 1f, 0f);

    [Header("References")]
    public CorgiAnimate mesh = null!;
    public Collider characterCollider = null!;

    [Header("Movement Settings")]
    public float speed = 300;
    public float jumpSpeed = 1;

    [NonSerialized]
    public Rigidbody rigidBody = null!;
    [NonSerialized]
    public GameObject cameraTarget = null!;

    private FixedJoint? heldJoint = null!;
    private Rigidbody? held = null!;
    private float cameraYaw = 0f;
    private float cameraPitch = 0f;

    private void Start() {
        rigidBody = GetComponent<Rigidbody>();
        mesh = GetComponent<CorgiAnimate>();
        rigidBody.maxAngularVelocity = 15f;
    }

    public void Tick(Vector2 move, Vector2 look, bool jump, bool grab) {
        if (jump && IsGrounded()) {
            Jump();
        }

        Vector2 moveDirection = CalculateMovementDirection(move);

        UpdateCamera(look);
        HandleAnimationAndRotation(moveDirection);
        ApplyMovement(moveDirection);
        HandleGrab(grab);
    }

    private void UpdateCamera(Vector2 lookInput) {
        cameraYaw += lookInput.x;
        cameraPitch += lookInput.y;

        if (cameraYaw < 0) cameraYaw += 360;
        if (cameraYaw >= 360) cameraYaw -= 360;

        cameraPitch = Mathf.Clamp(cameraPitch, MIN_PITCH, MAX_PITCH);

        cameraTarget.transform.SetPositionAndRotation(
            transform.position + CAMERA_OFFSET, 
            Quaternion.Euler(0f, cameraYaw, cameraPitch)
        );
    }

    private Vector2 CalculateMovementDirection(Vector2 input) {
        Vector2 normalized = speed * Time.deltaTime * input.normalized;

        float yawRad = cameraYaw * Mathf.Deg2Rad;
        float cosYaw = Mathf.Cos(yawRad);
        float sinYaw = Mathf.Sin(yawRad);
        float rotatedX = normalized.x * cosYaw + normalized.y * sinYaw;
        float rotatedY = -normalized.x * sinYaw + normalized.y * cosYaw;

        return new Vector2(rotatedX, rotatedY);
    }

    private void ApplyMovement(Vector2 moveDirection) {
        if (moveDirection.magnitude > 0f) {
            rigidBody.linearVelocity = new Vector3(-moveDirection.y, rigidBody.linearVelocity.y, moveDirection.x);
        }
        else {
            rigidBody.linearVelocity = new Vector3(0.0f, rigidBody.linearVelocity.y, 0.0f);
        }
    }

    public void StopMovement() {
        Vector3 velocity = rigidBody.linearVelocity;
        velocity.x = 0;
        velocity.z = 0;
        rigidBody.linearVelocity = velocity;

        ReleaseHeldObject();

        if (mesh != null) {
            mesh.still = true;
        }
    }

    private void HandleAnimationAndRotation(Vector2 moveDirection) {
        mesh.still = moveDirection.sqrMagnitude < Mathf.Epsilon;

        if (held || mesh.still) {
            return;
        }

        float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.y) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);

        //float currentAngle = transform.eulerAngles.y;
        //float angleDiffDeg = Mathf.DeltaAngle(currentAngle, targetAngle);

        //if (Mathf.Abs(angleDiffDeg) <= ROTATION_EPSILON) {
        //    Vector3 settledVelocity = rigidBody.angularVelocity;
        //    settledVelocity.y = 0f;
        //    rigidBody.angularVelocity = settledVelocity;
        //    return;
        //}

        //float angleDiffRad = angleDiffDeg * Mathf.Deg2Rad;
        //float stepDuration = Time.fixedDeltaTime > Mathf.Epsilon ? Time.fixedDeltaTime : Time.deltaTime;
        //float desiredAngularVelocity = angleDiffRad / stepDuration;
        //float clampedAngularVelocity = Mathf.Clamp(desiredAngularVelocity, -rigidBody.maxAngularVelocity, rigidBody.maxAngularVelocity);

        //Vector3 newAngularVelocity = rigidBody.angularVelocity;
        //newAngularVelocity.y = clampedAngularVelocity;
        //rigidBody.angularVelocity = newAngularVelocity;
    }

    private void Jump() {
        //rigidBody.linearVelocity = new Vector3(
        //    rigidBody.linearVelocity.x,
        //    jumpSpeed,
        //    rigidBody.linearVelocity.z
        //);

        rigidBody.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
    }

    private bool IsGrounded() {
        Vector3 center = characterCollider.bounds.center;
        Vector3 size = characterCollider.bounds.size * 0.8f;

        return Physics.BoxCast(
            center,
            size * 0.5f,
            -transform.up,
            transform.rotation,
            0.3f,
            -1
        );
    }

    struct GrabRay {
        public Vector3 origin;
        public Vector3 direction;
        public float distance;
        public int layerMask;
    }

    private GrabRay GetGrabRay() {
        Vector3 center = characterCollider.bounds.center;
        Vector3 size = characterCollider.bounds.size;

        float originOffset = size.z / 2f;

        Vector3 direction = (Quaternion.Euler(0f, transform.eulerAngles.y, 0f) * Vector3.left).normalized;
        Vector3 origin = direction * originOffset + center;

        GrabRay grabRay = new();

        grabRay.origin = origin;
        grabRay.direction = direction;
        grabRay.distance = GRAB_DISTANCE;
        grabRay.layerMask = ~0;

        return grabRay;
    }

    private RaycastHit? GetGrabbable() {
        if (mesh == null) {
            return null;
        }

        GrabRay grabRay = GetGrabRay();

        bool canGrab = Physics.Raycast(grabRay.origin, grabRay.direction, out RaycastHit hit, grabRay.distance, grabRay.layerMask);

        return canGrab ? hit : null;
    }

    private void HandleGrab(bool grab) {
        if (!grab) {
            ReleaseHeldObject();

            return;
        }

        if (held) {
            return;
        }

        RaycastHit? hit = GetGrabbable();

        if (!hit.HasValue) {
            return;
        }
        
        held = hit.Value.rigidbody;
        heldJoint = gameObject.AddComponent<FixedJoint>();
        heldJoint.enablePreprocessing = false;
        heldJoint.enableCollision = false;
        heldJoint.connectedBody = held;
        heldJoint.anchor = held.transform.InverseTransformPoint(hit.Value.point);
    }

    private void ReleaseHeldObject() {
        if (heldJoint) {
            Destroy(heldJoint);
            heldJoint = null;
        }

        if (held) {
            held = null;
        }
    }

    void OnDrawGizmos() {
        if (characterCollider == null) return;

        DrawGrabRayGizmo();
    }

    private void DrawGrabRayGizmo() {
        RaycastHit? hit = GetGrabbable();

        GrabRay grabRay = GetGrabRay();

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(grabRay.origin, grabRay.origin + grabRay.direction * grabRay.distance);
        Gizmos.DrawSphere(grabRay.origin, 0.025f);

        if (hit.HasValue) {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(hit.Value.point, 0.03f);
        }
    }
}
