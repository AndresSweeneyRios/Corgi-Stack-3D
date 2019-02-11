using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camera_target : MonoBehaviour
{
    private RaycastHit hit;
    private int layerMask = ~(1 << 2);
    public float distance;

    void Update() {
        Vector3 position = transform.position;
        Vector3 direction = transform.TransformDirection(Vector3.right);
        
        if (Physics.Raycast(position, direction, out hit, distance, layerMask)) {
            Debug.DrawRay(position, direction * hit.distance, Color.yellow);
            this.gameObject.transform.GetChild(0).gameObject.transform.position = hit.point;
        } else {
            Debug.DrawRay(position, direction * distance, Color.white);
            this.gameObject.transform.GetChild(0).gameObject.transform.position = transform.position + (direction * distance);
        }
    }
}
