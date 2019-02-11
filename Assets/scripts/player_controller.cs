using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player_controller : MonoBehaviour {
	private Rigidbody rb;
	public float speed;

	private void Start() {
		rb = GetComponent<Rigidbody>();
	}

	void FixedUpdate () {
		float hmove = Input.GetAxis("Horizontal");
		float vmove = Input.GetAxis("Vertical");

		Vector3 movement = new Vector3(hmove, 0, vmove);

		rb.AddForce(movement*speed);
	}
}
