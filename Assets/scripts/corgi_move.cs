using UnityEngine;

public class corgi_move : MonoBehaviour {
    public Rigidbody rb;
    public GameObject camera_target;
    public corgi_animate mesh;

    public float speed = 300;
    public float camspeed = 8;
    public float jspeed = 17;
    public float maxpitch = 80;
    public float minpitch = 0;
    public bool isEnabled = false;
    public bool freeze = false;
    public bool rotationLock = false;

    private bool wasEnabled = false;

    bool boxcast;
    public Collider characterCollider;
    Vector3 bounds;
    Vector3 size;

    FixedJoint joint;
    Rigidbody held;
    RaycastHit touching;
    Vector3 move;

    private void Start() {
        mesh = transform.GetComponentInChildren<corgi_animate>();
        Physics.gravity = new Vector3(0, -50F, 0);
        wasEnabled = isEnabled;
    }

    private void Update() {
        bounds = characterCollider.bounds.center;
        size = characterCollider.bounds.size * 0.8f;

        boxcast = Physics.BoxCast(
            bounds,
            size * 0.5f,
            -transform.up,
            mesh.transform.rotation,
            0.3f,
            -1
        );

        if (
            (Input.GetKeyDown("space") || Input.GetKeyDown("joystick button 1"))
            && boxcast
            && isEnabled
            && !freeze
        ) {
            rb.AddForce(transform.up * jspeed, ForceMode.VelocityChange);
        }

        rotationLock = isEnabled && !freeze && Input.GetKey("left shift");

        if ((!isEnabled || freeze) && wasEnabled) {
            StopMovement();
        }

        wasEnabled = isEnabled;
    }

    void FixedUpdate() {
        if (isEnabled && !freeze) {
            HandleMovement();
        } else if (rb.linearVelocity.magnitude > 0f) {
            StopMovement();
        }
    }

    private void HandleMovement() {
        float hmove = Input.GetAxisRaw("Horizontal");
        float vmove = Input.GetAxisRaw("Vertical") * -1;
        float mx = Input.GetAxis("Look X");
        float my = Input.GetAxis("Look Y");

        if (!Cursor.visible) {
            mx += Input.GetAxis("Mouse X") / 2;
            my += Input.GetAxis("Mouse Y") / 2;
        }

        Transform cam = camera_target.GetComponent<Transform>();
        cam.Rotate(new Vector3(0, 0, my) * camspeed);
        transform.Rotate(new Vector3(0, mx, 0) * camspeed);

        Vector3 cam_rot = cam.eulerAngles;
        if (cam_rot.z > maxpitch) cam.Rotate(new Vector3(0, 0, my) * -camspeed);
        if (cam_rot.z < minpitch) cam.Rotate(new Vector3(0, 0, my) * -camspeed);

        Vector3 currentMoveInput = new Vector3(vmove, 0, hmove);
        move = transform.worldToLocalMatrix.inverse * new Vector3(vmove, rb.linearVelocity.y, hmove);

        if (Mathf.Abs(move.x) > 0.999f && Mathf.Abs(move.z) > 0.999f) {
            move.x *= 0.75f;
            move.z *= 0.75f;
        }

        move *= speed * Time.deltaTime;

        if (Mathf.Abs(hmove) > 0.01f || Mathf.Abs(vmove) > 0.01f) {
            rb.linearVelocity = new Vector3(move.x, rb.linearVelocity.y, move.z);
        } else {
            rb.linearVelocity = new Vector3(0.0f, rb.linearVelocity.y, 0.0f);
        }

        HandleObjectGrabbing();
        HandleAnimationAndRotation(hmove, vmove, currentMoveInput);
    }

    private void HandleObjectGrabbing() {
        Quaternion rot = Quaternion.Euler(0, mesh.transform.rotation.eulerAngles.y, 0);

        bool forwardCollision = Physics.Raycast(
            bounds,
            rot * Vector3.left,
            out touching,
            1.2f,
            ~0
        );

        if (forwardCollision && Input.GetKey("left shift") && held == null) {
            if (touching.rigidbody != null) {
                held = touching.rigidbody;
                joint = transform.gameObject.AddComponent<FixedJoint>();
                joint.connectedBody = held;
                joint.anchor = transform.InverseTransformPoint(touching.point);
                joint.enableCollision = false;
                held.mass = 0.1f;
            }
        }

        if (!Input.GetKey("left shift") && held != null) {
            if (joint != null) Destroy(joint);
            held.mass = 5f;
            held = null;
        }
    }

    private void HandleAnimationAndRotation(float hmove, float vmove, Vector3 currentMoveInput) {
        if (Mathf.Abs(move.x) + Mathf.Abs(move.z) < 0.1) {
            mesh.still = true;
        } else {
            mesh.still = false;

            if (!rotationLock || !held) {
                mesh.transform.rotation = Quaternion.AngleAxis(
                    Mathf.Atan2(vmove, hmove) * Mathf.Rad2Deg + 90 + transform.rotation.eulerAngles.y,
                    Vector3.up
                );
            }
        }
    }

    private void StopMovement() {
        Vector3 velocity = rb.linearVelocity;
        velocity.x = 0;
        velocity.z = 0;
        rb.linearVelocity = velocity;

        if (held != null) {
            if (joint != null) Destroy(joint);
            held.mass = 5f;
            held = null;
        }

        if (mesh != null) {
            mesh.still = true;
        }
    }

    void OnDrawGizmos() {
        if (!Application.isPlaying) return;

        Gizmos.color = boxcast ? Color.blue : Color.red;
        Gizmos.DrawWireCube(bounds, size);
    }
}
