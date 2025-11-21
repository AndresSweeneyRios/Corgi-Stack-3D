using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class character : MonoBehaviour {
    public int index = 0;

    public camera_target camera_target;
    private List<corgi_move> characters = new();

    private void Start() {
        foreach (corgi_move corgi_move in transform.GetComponentsInChildren<corgi_move>()) {
            characters.Add(corgi_move);

            corgi_move.camera_target = camera_target.gameObject;
        }

        SwitchCharacter(index);
    }

    void SwitchCharacter(int newIndex) {
        index = newIndex;

        if (index == characters.Count) index = 0;
        if (index < 0) index = characters.Count - 1;

        foreach (corgi_move corgi_move in characters) {
            corgi_move.isEnabled = false;
        }

        characters[index].isEnabled = true;

        camera_target.transform.SetParent(characters[index].transform);

        camera_target.transform.localPosition = new(0.0f, 1.0f, 0.0f);
    }

    void Update() {
        if (Input.GetKeyDown("mouse 0") || Input.GetKeyDown("joystick button 4")) {
            SwitchCharacter(index - 1);
        } else if (Input.GetKeyDown("mouse 1") || Input.GetKeyDown("joystick button 5")) {
            SwitchCharacter(index + 1);
        } else if (Input.GetMouseButtonDown(2)) {
            Cursor.visible = !Cursor.visible;
        }
    }
}
