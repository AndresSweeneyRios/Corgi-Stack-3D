using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class button : Activator {
    int triggers = 0;

    private void Step () {
        if (triggers > 0) {
            active = true;
            transform.GetChild(0).gameObject.SetActive(false);
        }
        else {
            active = false;
            transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit () {
        triggers--;
        Step();
    }

    private void OnTriggerEnter () {
        triggers++;
        Step();
    }
}
