#nullable enable

using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectController : MonoBehaviour {
    public int index = 0;

    public camera_target camera_target = null!;
    private readonly List<CorgiMove> characters = new();

    private void Start() {
        foreach (CorgiMove corgi_move in transform.GetComponentsInChildren<CorgiMove>()) {
            characters.Add(corgi_move);

            corgi_move.cameraTarget = camera_target.gameObject;
        }

        SwitchCharacter(index);

        InputManager.Initialize();
    }

    void SwitchCharacter(int newIndex) {
        characters[index].StopMovement();

        index = newIndex;

        if (index == characters.Count) index = 0;
        if (index < 0) index = characters.Count - 1;
    }

    void FixedUpdate() {
        if (InputManager.GetPrevious()) {
            SwitchCharacter(index - 1);
        } else if (InputManager.GetNext()) {
            SwitchCharacter(index + 1);
        }

        characters[index].Tick(
            InputManager.GetMove(),
            InputManager.GetLook(),
            InputManager.GetJump(),
            InputManager.GetGrab()
        );
    }
}
