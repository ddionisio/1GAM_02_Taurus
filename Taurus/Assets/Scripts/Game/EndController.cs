using UnityEngine;
using System.Collections;

public class EndController : MonoBehaviour {
    public float delay;

    public NGUIPage dialog;

    void OnDestroy() {
    }

    void Awake() {
        dialog.pageEndCallback = OnDialogEnd;

        dialog.gameObject.SetActive(false);
    }

    // Use this for initialization
    void Start() {
        Invoke("DisplayDialog", delay);
    }

    void DisplayDialog() {
        dialog.gameObject.SetActive(true);
    }

    void OnDialogEnd() {
        Main.instance.sceneManager.LoadScene(Main.instance.startScene);
    }
}
