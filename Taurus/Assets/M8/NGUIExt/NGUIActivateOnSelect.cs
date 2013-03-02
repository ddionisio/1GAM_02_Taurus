using UnityEngine;
using System.Collections;

public class NGUIActivateOnSelect : MonoBehaviour {
    public GameObject target;

    private bool mStarted = false;

    void OnEnable() {
        if(mStarted) {
            target.SetActive(UICamera.selectedObject == gameObject);
        }
    }

    void OnSelect(bool yes) {
        target.SetActive(yes);
    }

    void Awake() {
        target.SetActive(false);
    }

    void Start() {
        mStarted = true;
    }
}
