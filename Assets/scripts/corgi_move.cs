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

	RaycastHit hit;
	Collider collider;
	Vector3 extents;
	Vector3 bounds;
	bool boxcast;
	float distance = 0.5f;
	int layerMask = ~0;

	private void Start() {
		mesh = transform.GetComponentInChildren<corgi_animate>();
		collider = transform.GetComponentInChildren<Collider>();
		Physics.gravity = new Vector3(0, -30F, 0);
	}

	void FixedUpdate () {
		float hmove = Input.GetAxisRaw("Horizontal");
		float vmove = Input.GetAxisRaw("Vertical") * -1; 
		float mx =  Input.GetAxis("Look X");
		float my = Input.GetAxis("Look Y");

		if (!Cursor.visible) {
			mx += Input.GetAxis("Mouse X");
			my += Input.GetAxis("Mouse Y");
		}

		bounds = collider.bounds.center;
        extents = collider.bounds.extents*2;
		boxcast = Physics.BoxCast(bounds, new Vector3(extents.x, 0.01f, extents.z), -transform.up, out hit, transform.rotation, distance, layerMask);

		if (Input.GetKeyDown("space") || Input.GetKeyDown("joystick button 1")) {
			if (boxcast) rb.AddForce(transform.up * jspeed, ForceMode.VelocityChange);
		}

		Transform cam = camera_target.GetComponent<Transform>();
		
		cam.Rotate(new Vector3(0,0,my) * camspeed);
		transform.Rotate(new Vector3(0,mx,0) * camspeed);
		
		Vector3 cam_rot = cam.eulerAngles;
		if (cam_rot.z > maxpitch) cam.Rotate(new Vector3(0,0,my) * -camspeed);
		if (cam_rot.z < minpitch) cam.Rotate(new Vector3(0,0,my) * -camspeed);

		Vector3 move = transform.worldToLocalMatrix.inverse * new Vector3(vmove, rb.velocity.y, hmove);
		move *= speed * Time.deltaTime;
		rb.velocity = new Vector3(move.x,rb.velocity.y,move.z);

		if (Mathf.Abs(move.x) + Mathf.Abs(move.z) < 0.1) {
			mesh.still = true;
		} else {
			mesh.still = false;
			mesh.transform.rotation = Quaternion.AngleAxis(((Mathf.Atan2(vmove, hmove)*Mathf.Rad2Deg)+90)+transform.rotation.eulerAngles.y, Vector3.up);
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
            Gizmos.DrawRay(bounds, -transform.up * 0.5f);
            Gizmos.DrawWireCube(bounds + -transform.up * 0.5f, new Vector3(extents.x,0.01f,extents.z));
        }
    }
}
