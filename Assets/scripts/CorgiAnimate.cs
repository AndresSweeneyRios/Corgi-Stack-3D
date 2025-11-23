#nullable enable

using System;
using System.Collections.Generic;
using UnityEngine;

public class CorgiAnimate : MonoBehaviour {
	public float speed = 0.1f;

	private float timer = 0;
	private int index = 1;
    private List<GameObject> children = new();
    private CorgiMove corgiMove = null!;

    [NonSerialized]
    public bool still = true;

    void SetActiveIndex(int newIndex) {
        children[index].SetActive(false);

        index = newIndex;

        if (index < 0) {
            index = children.Count - 1;
        } else if (index > children.Count - 1) {
            index = 0;
        }

        children[index].SetActive(true);
    }

    void Start() {
        corgiMove = transform.parent.GetComponent<CorgiMove>();

        foreach (Transform child in transform) {
            children.Add(child.gameObject);
            child.gameObject.SetActive(false);
        }
    }

    void Update () {
		if (still) {
            SetActiveIndex(0);

            return;
        }

        if (timer < speed) {
            timer += Time.deltaTime;
        } else {
            timer %= speed;

            SetActiveIndex(index + 1);
        }
    }
}
