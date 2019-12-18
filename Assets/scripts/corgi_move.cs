using System.Collections;
using System.Collections.Generic;
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
	public bool enabled = false;
	public bool freeze = false;
    public bool rotationLock = false;
	bool jump = false;
	bool boxcast;
	Collider collider;
	Vector3 bounds;
	Vector3 extents;
	RaycastHit hit;
	Camera camera;

    FixedJoint joint;

    Rigidbody held;
    RaycastHit touching;
    Vector3 move;

	private void Start() {
		mesh = transform.GetComponentInChildren<corgi_animate>();
		collider = transform.GetComponentInChildren<Collider>();
		camera = transform.GetChild(0).GetChild(0).GetComponent<Camera>();
		Physics.gravity = new Vector3(0, -50F, 0);
	}

	private void Update() {
		camera.enabled = enabled;

		bounds = collider.bounds.center;
        extents = collider.bounds.extents * 2;

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
            && enabled 
            && !freeze
        ) {
            jump = true;

            rb.AddForce(
                transform.up * jspeed, 
                ForceMode.VelocityChange
            );
        }

        rotationLock = enabled && !freeze && Input.GetKey("left shift");

        // transform.GetChild(1).GetComponent<BoxCollider>().material.staticFriction = enabled ? 0f : 4.2f;
	}

	void FixedUpdate () {
		if (enabled && !freeze) {
			float hmove = Input.GetAxisRaw("Horizontal");
			float vmove = Input.GetAxisRaw("Vertical") * -1; 
			float mx = Input.GetAxis("Look X");
			float my = Input.GetAxis("Look Y");

			if (!Cursor.visible) {
				mx += Input.GetAxis("Mouse X") / 2;
				my += Input.GetAxis("Mouse Y") / 2;
			}

			Transform cam = camera_target.GetComponent<Transform>();
			
			cam.Rotate(new Vector3(0,0,my) * camspeed);
			transform.Rotate(new Vector3(0,mx,0) * camspeed);
			
			Vector3 cam_rot = cam.eulerAngles;
			if (cam_rot.z > maxpitch) cam.Rotate(new Vector3(0,0,my) * -camspeed);
			if (cam_rot.z < minpitch) cam.Rotate(new Vector3(0,0,my) * -camspeed);

			move = transform.worldToLocalMatrix.inverse * new Vector3(vmove, rb.velocity.y, hmove);

			if (Mathf.Abs(move.x) > 0.999f && Mathf.Abs(move.z) > 0.999f) {
				move.x *= 0.75f;
				move.z *= 0.75f;
			}

			move *= speed * Time.deltaTime;
			rb.velocity = new Vector3(
                move.x, 
                rb.velocity.y, 
                move.z
            );

            Quaternion rot = Quaternion.Euler(0, collider.transform.rotation.eulerAngles.y, 0);

            bool forwardCollision = Physics.Raycast(
                bounds, 
                rot * Vector3.left,
                out touching,
                1.2f,
                ~0
            );

            if (forwardCollision && Input.GetKey("left shift") && held == null) {
                if (touching.rigidbody == null) return;
                held = touching.rigidbody;
                joint = transform.gameObject.AddComponent<FixedJoint>();
                joint.connectedBody = held;
                joint.anchor = transform.InverseTransformPoint(touching.point);
                joint.enableCollision = false;
                held.mass = 0.1f;
            }
            
            if (!Input.GetKey("left shift") && held != null) {
                Destroy(joint);
                held.mass = 5f;
                held = null;
            }

			if (Mathf.Abs(move.x) + Mathf.Abs(move.z) < 0.1) {
				mesh.still = true;
			} else {
				mesh.still = false;

				if (!rotationLock || !held) {
                    mesh.transform.rotation = Quaternion.AngleAxis(
                          Mathf.Atan2(vmove, hmove) 
                        * Mathf.Rad2Deg 
                        + 90 
                        + transform.rotation.eulerAngles.y, 

                        Vector3.up
                    );
                }
			}
		}
	}

    void OnDrawGizmos () {
        if (!Application.isPlaying) return;

        Quaternion rot = Quaternion.Euler(0, collider.transform.rotation.eulerAngles.y, 0);

        Gizmos.color = Color.black; 
        Gizmos.DrawRay(
            bounds, 
            rot *
            Vector3.left *
            1.2f
        );

        // Gizmos.color = Color.green; 
        // Gizmos.DrawRay(
        //     bounds, 
        //     new Vector3(0, 0, Mathf.Clamp(move.z, -1f, 1f)) * 
        //     (extents.z / 2)
        // );

        if (boxcast) {
            Gizmos.color = Color.blue; 
            Gizmos.DrawRay(bounds, -transform.up * hit.distance);
            Gizmos.DrawWireCube(bounds + -transform.up * hit.distance, new Vector3(extents.x,0.01f,extents.z));
        } else {
            Gizmos.color = Color.red; 
            Gizmos.DrawRay(bounds, -transform.up * 0.1f);
            Gizmos.DrawWireCube(bounds + -transform.up * 0.1f, new Vector3(extents.x,0.01f,extents.z));
        }
    }
}
