using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trigger : MonoBehaviour {
    public GameObject activator;
    public bool reverse;
    public Activator activator_script;

    private void Start() {
        activator_script = activator.GetComponent<Activator>();
        for(int i = 0; i < transform.childCount; i++) {
            if (reverse) transform.GetChild(i).gameObject.SetActive(false);
            else transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    void Update () {
        if (activator_script.active) {
            for(int i = 0; i < transform.childCount; i++) {
                if (reverse) transform.GetChild(i).gameObject.SetActive(true);
                else transform.GetChild(i).gameObject.SetActive(false);
            }
        } else {
            for(int i = 0; i < transform.childCount; i++) {
                if (reverse) transform.GetChild(i).gameObject.SetActive(false);
                else transform.GetChild(i).gameObject.SetActive(true);
            }
        }
    }
}
