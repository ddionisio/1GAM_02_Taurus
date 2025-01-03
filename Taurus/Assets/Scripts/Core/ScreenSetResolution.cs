using UnityEngine;
using System.Collections;

public class ScreenSetResolution : MonoBehaviour {
    public int width = 1280;
    public int height = 720;
    public bool fullscreen = true;

    // Use this for initialization
    void Start() {
        Screen.SetResolution(width, height, fullscreen);
    }
}
