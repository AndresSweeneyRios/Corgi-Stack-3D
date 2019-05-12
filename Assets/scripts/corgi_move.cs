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
	bool jump = false;
	bool boxcast;
	Collider collider;
	Vector3 bounds;
	Vector3 extents;
	RaycastHit hit;
	Camera camera;

	private void Start() {
		mesh = transform.GetComponentInChildren<corgi_animate>();
		collider = transform.GetComponentInChildren<Collider>();
		camera = transform.GetChild(0).GetChild(0).GetComponent<Camera>();
		Physics.gravity = new Vector3(0, -50F, 0);
	}

	private void Update() {
		camera.enabled = enabled;

		bounds = collider.bounds.center;
        extents = collider.bounds.extents*2;

		bounds.y += 0.2f;
		extents.y = 0.1f;

		boxcast = Physics.BoxCast(bounds, extents, -transform.up, out hit, transform.GetChild(1).rotation, 0.7f, ~0);
		if ((Input.GetKeyDown("space") || Input.GetKeyDown("joystick button 1")) && boxcast && enabled && !freeze) rb.AddForce(transform.up * jspeed, ForceMode.VelocityChange);

		if (freeze)  rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY |RigidbodyConstraints.FreezePositionZ;
		else if (enabled) rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
		else rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
	}

	void FixedUpdate () {
		if (enabled && !freeze) {
			float hmove = Input.GetAxisRaw("Horizontal");
			float vmove = Input.GetAxisRaw("Vertical") * -1; 
			float mx =  Input.GetAxis("Look X");
			float my = Input.GetAxis("Look Y");

			if (!Cursor.visible) {
				mx += Input.GetAxis("Mouse X");
				my += Input.GetAxis("Mouse Y");
			}

			Transform cam = camera_target.GetComponent<Transform>();
			
			cam.Rotate(new Vector3(0,0,my) * camspeed);
			transform.Rotate(new Vector3(0,mx,0) * camspeed);
			
			Vector3 cam_rot = cam.eulerAngles;
			if (cam_rot.z > maxpitch) cam.Rotate(new Vector3(0,0,my) * -camspeed);
			if (cam_rot.z < minpitch) cam.Rotate(new Vector3(0,0,my) * -camspeed);

			Vector3 move = transform.worldToLocalMatrix.inverse * new Vector3(vmove, rb.velocity.y, hmove);

			if (Mathf.Abs(move.x) > 0.999f && Mathf.Abs(move.z) > 0.999f) {
				move.x *= 0.75f;
				move.z *= 0.75f;
			}

			move *= speed * Time.deltaTime;
			rb.velocity = new Vector3(move.x,rb.velocity.y,move.z);

			if (Mathf.Abs(move.x) + Mathf.Abs(move.z) < 0.1) {
				mesh.still = true;
			} else {
				mesh.still = false;
				mesh.transform.rotation = Quaternion.AngleAxis(((Mathf.Atan2(vmove, hmove)*Mathf.Rad2Deg)+90)+transform.rotation.eulerAngles.y, Vector3.up);
			}
		}
	}

    void OnDrawGizmos () {
        if (!Application.isPlaying) return;
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
