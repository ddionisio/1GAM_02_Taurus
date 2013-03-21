using UnityEngine;
using System.Collections;

public class NGUISelectClick : MonoBehaviour {
    public GameObject select;

    void OnClick() {
        if(enabled && select != null) {
            UICamera.selectedObject = select;
        }
    }
}
