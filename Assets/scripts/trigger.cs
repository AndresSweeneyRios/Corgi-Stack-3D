using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trigger : MonoBehaviour {
    public GameObject activator;
    public bool reverse;
    private lever activator_script;

    private void Start() {
        activator_script = activator.GetComponent<lever>();
    }

    void Update () {
        if (activator_script.active) {
            for(int i = 0; i < transform.GetChildCount(); i++) {
                if (reverse) transform.GetChild(i).setActive(true);
                else transform.GetChild(i).setActive(false);
            }
        }
    }
}
