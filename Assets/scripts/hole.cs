using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hole : MonoBehaviour {
    public bool active = false;
    public bool animating = false;
    public GameObject other_hole;
    public GameObject schatzi;
    public Transform schatzi_mesh;
    corgi_move schatzi_script;
    int timer = 0;

    private void Update() {
        if (active && (Input.GetKeyDown("e") || Input.GetKeyDown("joystick button 0")) && !schatzi_script.freeze) animating = true;
        if (animating == true) Animate();
    }

    private void OnTriggerEnter(Collider other) {
        if (other.transform.parent.gameObject.name == "player_schatzi") {
            active = true;
            schatzi = other.transform.parent.gameObject;
            schatzi_script = schatzi.transform.GetComponent<corgi_move>();
            schatzi_mesh = schatzi.transform.GetChild(1);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.transform.parent.gameObject.name == "player_schatzi") {
            active = false;
        }
    }

    private void Animate () {
        Vector3 rot = schatzi_mesh.transform.eulerAngles;
        Vector3 pos = schatzi_mesh.position;

        if (timer == 0) {
            schatzi.transform.position = transform.position;
            schatzi_script.freeze = true;
            rot = transform.eulerAngles;
            pos = transform.position + (Vector3.right * 1.35f);
            schatzi_mesh.GetComponent<corgi_animate>().still = false;
        } else if (timer < 60) {
            rot.z += 1.5f;
            pos.y += 0.01f;
            pos += Vector3.right * -0.01f;
            transform.localScale += new Vector3(0.02F, 0, 0.02F);
            if (timer > 30) pos.y -= 0.035f;
        } else if (timer < 90) {
            pos.y -= 0.035f;
            transform.localScale -= new Vector3(0.04F, 0, 0.04F);
        } else if (timer < 120) {
            // wait
        } else if (timer == 120) {
            Vector3 npos = other_hole.transform.position - (Vector3.right * 0.75f);
            schatzi.transform.position = npos;
            pos = new Vector3(npos.x, pos.y, npos.z);
            rot.z = -90f;
        } else if (timer < 160) {
            if (timer > 130) other_hole.transform.localScale += new Vector3(0.04F, 0, 0.04F);
            pos.y += 0.035f;
        } else if (timer < 190) {
            rot.z += 3f;
            pos += Vector3.right * -0.015f;
            other_hole.transform.localScale -= new Vector3(0.04F, 0, 0.04F);
        } else {
            schatzi_script.freeze = false;
            animating = false;
            timer = -1;
            schatzi_mesh.GetComponent<corgi_animate>().still = true;
        };

        schatzi_mesh.eulerAngles = rot;
        schatzi_mesh.position = pos;

        timer++;
    }
}
