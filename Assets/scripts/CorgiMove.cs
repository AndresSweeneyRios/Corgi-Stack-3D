#nullable enable

using System;
using UnityEngine;

public class CorgiMove : MonoBehaviour {
    private const float GRAB_MASS = 0.1f;
    private const float MAX_PITCH = 80f;
    private const float MIN_PITCH = 0f;
    private const float GRAB_DISTANCE = 0.5f;

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
    [NonSerialized]
    public float cameraSpeed = 8;

    private FixedJoint? heldJoint = null!;
    private Rigidbody? held = null!;
    private float heldMass = 0f;

    private void Start() {
        rigidBody = GetComponent<Rigidbody>();
        mesh = transform.GetComponentInChildren<CorgiAnimate>();
        Physics.gravity = new Vector3(0, -50F, 0);
    }

    public void Tick(Vector2 move, Vector2 look, bool jump, bool grab) {
        if (jump && IsGrounded()) {
            Jump();
        }

        UpdateCamera(look);
        ApplyMovement(move);
        HandleAnimationAndRotation(move);
        HandleGrab(grab);
    }

    private void UpdateCamera(Vector2 lookInput) {
        Transform cam = cameraTarget.GetComponent<Transform>();
        cam.Rotate(new Vector3(0, 0, lookInput.y) * cameraSpeed);
        transform.Rotate(new Vector3(0, lookInput.x, 0) * cameraSpeed);

        Vector3 cam_rot = cam.eulerAngles;
        if (cam_rot.z > MAX_PITCH) cam.Rotate(new Vector3(0, 0, lookInput.y) * -cameraSpeed);
        if (cam_rot.z < MIN_PITCH) cam.Rotate(new Vector3(0, 0, lookInput.y) * -cameraSpeed);
    }

    private void ApplyMovement(Vector2 input) {
        Vector3 move = CalculateMovementVector(input);

        if (Mathf.Abs(input.x) > 0.01f || Mathf.Abs(input.y) > 0.01f) {
            rigidBody.linearVelocity = new Vector3(move.x, rigidBody.linearVelocity.y, move.z);
        } else {
            rigidBody.linearVelocity = new Vector3(0.0f, rigidBody.linearVelocity.y, 0.0f);
        }
    }

    private Vector3 CalculateMovementVector(Vector2 input) {
        Vector3 move = transform.worldToLocalMatrix.inverse * new Vector3(input.y, rigidBody.linearVelocity.y, input.x);

        if (Mathf.Abs(move.x) > 0.999f && Mathf.Abs(move.z) > 0.999f) {
            move.x *= 0.75f;
            move.z *= 0.75f;
        }

        move *= speed * Time.deltaTime;
        return move;
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

    private void Jump() {
        rigidBody.AddForce(transform.up * jumpSpeed, ForceMode.VelocityChange);
    }

    private bool IsGrounded() {
        Vector3 center = characterCollider.bounds.center;
        Vector3 size = characterCollider.bounds.size * 0.8f;

        return Physics.BoxCast(
            center,
            size * 0.5f,
            -transform.up,
            mesh.transform.rotation,
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

        Vector3 direction = (Quaternion.Euler(0f, mesh.transform.eulerAngles.y, 0f) * Vector3.left).normalized;
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
        heldJoint.connectedBody = held;
        heldJoint.anchor = transform.InverseTransformPoint(hit.Value.point);
        heldJoint.enableCollision = false;
        heldMass = held.mass;
        held.mass = GRAB_MASS;
    }

    private void ReleaseHeldObject() {
        if (heldJoint) {
            Destroy(heldJoint);
            heldJoint = null;
        }

        if (held) {
            held.mass = heldMass;
            held = null;
        }
    }

    private void HandleAnimationAndRotation(Vector2 moveInput) {
        Vector3 move = CalculateMovementVector(moveInput);

        if (move.magnitude < 0.1) {
            mesh.still = true;

            return;
        }

        mesh.still = false;

        if (held) {
            return;
        }

        mesh.transform.rotation = Quaternion.AngleAxis(
            Mathf.Atan2(moveInput.y, moveInput.x) * Mathf.Rad2Deg + 90 + transform.rotation.eulerAngles.y,
            Vector3.up
        );
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
