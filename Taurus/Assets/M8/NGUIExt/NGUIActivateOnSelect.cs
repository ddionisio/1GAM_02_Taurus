using UnityEngine;
using System.Collections;

public class NGUIActivateOnSelect : MonoBehaviour {
    public GameObject target;

    void OnSelect(bool yes) {
        target.SetActive(yes);
    }

    void Awake() {
        target.SetActive(false);
    }
}
