using UnityEngine;

public class corgi_move : MonoBehaviour {
    public Rigidbody rb;
    public GameObject camera_target;
    public corgi_animate mesh;

    public float speed = 20;
    public float camspeed = 10;
    public float jspeed;
    public float maxpitch;
    public float minpitch;
    public bool isEnabled = false;
    public bool freeze = false;
    public bool rotationLock = false;

    // Movement state tracking
    private bool wasEnabled = false;
    private Vector3 lastMoveInput = Vector3.zero;

    bool boxcast;
    Collider characterCollider;
    Vector3 bounds;
    Vector3 extents;
    RaycastHit hit;
    Camera characterCamera;

    FixedJoint joint;
    Rigidbody held;
    RaycastHit touching;
    Vector3 move;

    private void Start() {
        mesh = transform.GetComponentInChildren<corgi_animate>();
        characterCollider = transform.GetComponentInChildren<Collider>();
        characterCamera = transform.GetChild(0).GetChild(0).GetComponent<Camera>();
        Physics.gravity = new Vector3(0, -50F, 0);
        wasEnabled = isEnabled;
    }

    private void Update() {
        characterCamera.enabled = isEnabled;

        bounds = characterCollider.bounds.center;
        extents = characterCollider.bounds.extents * 2;

        bounds.y += 0.2f;
        extents.y = 0.1f;

        boxcast = Physics.BoxCast(
            bounds,
            extents,
            -transform.up,
            out hit,
            transform.GetChild(1).rotation,
            0.7f,
            ~0
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

        // Stop movement when disabled or frozen
        if ((!isEnabled || freeze) && wasEnabled) {
            StopMovement();
        }

        wasEnabled = isEnabled;
    }

    void FixedUpdate() {
        if (isEnabled && !freeze) {
            HandleMovement();
        }

        else {
            // Ensure no residual movement when disabled
            if (rb.linearVelocity.magnitude > 0.1f) {
                StopMovement();
            }
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

        // Reset velocity


        // Camera rotation
        Transform cam = camera_target.GetComponent<Transform>();
        cam.Rotate(new Vector3(0, 0, my) * camspeed);
        transform.Rotate(new Vector3(0, mx, 0) * camspeed);

        Vector3 cam_rot = cam.eulerAngles;
        if (cam_rot.z > maxpitch) cam.Rotate(new Vector3(0, 0, my) * -camspeed);
        if (cam_rot.z < minpitch) cam.Rotate(new Vector3(0, 0, my) * -camspeed);

        // Movement calculation
        Vector3 currentMoveInput = new Vector3(vmove, 0, hmove);
        move = transform.worldToLocalMatrix.inverse * new Vector3(vmove, rb.linearVelocity.y, hmove);

        // Diagonal movement normalization
        if (Mathf.Abs(move.x) > 0.999f && Mathf.Abs(move.z) > 0.999f) {
            move.x *= 0.75f;
            move.z *= 0.75f;
        }

        move *= speed * Time.deltaTime;

        // Apply movement only if there's input, otherwise stop horizontal movement
        if (Mathf.Abs(hmove) > 0.01f || Mathf.Abs(vmove) > 0.01f) {
            rb.linearVelocity = new Vector3(move.x, rb.linearVelocity.y, move.z);
        }
        else {
            // Stop horizontal movement but preserve vertical velocity (for jumping/gravity)
            rb.linearVelocity = new Vector3(0.0f, rb.linearVelocity.y, 0.0f);
        }

        // Object grabbing logic
        HandleObjectGrabbing();

        // Animation and rotation
        HandleAnimationAndRotation(hmove, vmove, currentMoveInput);

        lastMoveInput = currentMoveInput;
    }

    private void HandleObjectGrabbing() {
        Quaternion rot = Quaternion.Euler(0, characterCollider.transform.rotation.eulerAngles.y, 0);

        bool forwardCollision = Physics.Raycast(
            bounds,
            rot * Vector3.left,
            out touching,
            1.2f,
            ~0
        );

        // Grab object
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

        // Release object
        if (!Input.GetKey("left shift") && held != null) {
            if (joint != null) Destroy(joint);
            held.mass = 5f;
            held = null;
        }
    }

    private void HandleAnimationAndRotation(float hmove, float vmove, Vector3 currentMoveInput) {
        if (Mathf.Abs(move.x) + Mathf.Abs(move.z) < 0.1) {
            mesh.still = true;
        }
        else {
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
        // Stop horizontal movement immediately
        Vector3 velocity = rb.linearVelocity;
        velocity.x = 0;
        velocity.z = 0;
        rb.linearVelocity = velocity;

        // Release any held objects
        if (held != null) {
            if (joint != null) Destroy(joint);
            held.mass = 5f;
            held = null;
        }

        // Set animation to still
        if (mesh != null) {
            mesh.still = true;
        }
    }

    void OnDrawGizmos() {
        if (!Application.isPlaying) return;

        Quaternion rot = Quaternion.Euler(0, characterCollider.transform.rotation.eulerAngles.y, 0);

        Gizmos.color = Color.black;
        Gizmos.DrawRay(bounds, rot * Vector3.left * 1.2f);

        if (boxcast) {
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(bounds, -transform.up * hit.distance);
            Gizmos.DrawWireCube(bounds + -transform.up * hit.distance, new Vector3(extents.x, 0.01f, extents.z));
        }
        else {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(bounds, -transform.up * 0.1f);
            Gizmos.DrawWireCube(bounds + -transform.up * 0.1f, new Vector3(extents.x, 0.01f, extents.z));
        }
    }
}
