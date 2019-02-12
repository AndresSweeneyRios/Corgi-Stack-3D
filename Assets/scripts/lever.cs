using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lever : MonoBehaviour {
	RaycastHit hit;
	Collider collider;
    Vector3 bounds;
    Vector3 extents;
	public float distance = Mathf.Infinity;
	public float boxcast_offset = 0f;
	int layerMask = ~0;
	public bool active = false;

    void Start () {
		collider = transform.GetComponentInChildren<Collider>();
    }

    void FixedUpdate () {
		bounds = collider.bounds.center;
        extents = collider.bounds.extents*2;
		if (!active) active = Physics.BoxCast(new Vector3(bounds.x, bounds.y+boxcast_offset, bounds.z), new Vector3(extents.x,0.01f,extents.z), transform.up, out hit, transform.rotation, distance, layerMask);
        else transform.GetChild(0).gameObject.SetActive(false);
    }

    void OnDrawGizmos () {
        if (!Application.isPlaying) return;
        if (active) {
            Gizmos.color = Color.blue; 
            Gizmos.DrawRay(collider.bounds.center, transform.up * hit.distance);
            Gizmos.DrawWireCube(collider.bounds.center + transform.up * hit.distance, new Vector3(extents.x,0.01f,extents.z));
        } else {
            Gizmos.color = Color.red; 
            Gizmos.DrawRay(collider.bounds.center, transform.up * 0.1f);
            Gizmos.DrawWireCube(collider.bounds.center + transform.up * 0.1f, new Vector3(extents.x,0.01f,extents.z));
        }
    }
}
