using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class character : MonoBehaviour {
    public int index = 0;

    void Update() {
        for(int i = 0; i < transform.childCount; i++) transform.GetChild(i).GetComponent<corgi_move>().enabled = false;
        transform.GetChild(index).GetComponent<corgi_move>().enabled = true;
        if (Input.GetKeyDown("mouse 0") || Input.GetKeyDown("joystick button 4")) index--;
        if (Input.GetKeyDown("mouse 1") || Input.GetKeyDown("joystick button 5")) index++;
        if (index == transform.childCount) index = 0;
        if (index < 0) index = transform.childCount - 1;

		if(Input.GetMouseButtonDown(2)) Cursor.visible = !Cursor.visible;
    }
}
