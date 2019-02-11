using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class corgi_animate : MonoBehaviour {
	public List<GameObject> children;
	private int timer = 0;
	public int speed = 20;
	private int index = 1;
	public bool still = true;

	void Update () {
		if (!still){
			if (timer < speed) {
				timer++;
			} else {
				timer = 0;
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
