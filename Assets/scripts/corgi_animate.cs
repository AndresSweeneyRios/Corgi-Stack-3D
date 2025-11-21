using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class corgi_animate : MonoBehaviour {
	public List<GameObject> children;
	private float timer = 0;
	public float speed = 0.1f;
	private int index = 1;
	public bool still = true;

	void Update () {
		if (!transform.parent.GetComponent<corgi_move>().isEnabled) still = true;

		if (!still){
			if (timer < speed) {
				timer += Time.deltaTime;
			} else {
				timer = timer % speed;
				children[index].SetActive(false);
				index++;

				if (index > children.Count-1) {
					index = 1;
				}

				children[index].SetActive(true);
			}
		} else {
			children[index].SetActive(false);
			index = 0;
			children[index].SetActive(true);
		}
	}
}
