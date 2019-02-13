using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class button : Activator {
    private void OnTriggerExit () {
        active = false;
        transform.GetChild(0).gameObject.SetActive(true);
    }

    private void OnTriggerEnter () {
        active = true;
        transform.GetChild(0).gameObject.SetActive(false);
    }
}
