using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camera_controller : MonoBehaviour {
	public GameObject player;
	private Vector3 offset;

	private void Start () {
		offset = transform.position - player.transform.position;	
	}
	
	private void LateUpdate() {
		transform.position = player.transform.position + offset;
	}
}
