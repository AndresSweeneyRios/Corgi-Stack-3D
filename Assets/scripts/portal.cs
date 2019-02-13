using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class portal : MonoBehaviour {
    public string scene;
    void OnTriggerEnter () { SceneManager.LoadScene(scene); }
}
