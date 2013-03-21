using UnityEngine;
using System.Collections;

public class UndoClick : MonoBehaviour {

    private PlayerController mPlayerControl;

    // Use this for initialization
    void Start() {
        GameObject go = GameObject.FindGameObjectWithTag(Layers.tagController);
        mPlayerControl = go.GetComponentInChildren<PlayerController>();
    }

    void OnClick() {
        mPlayerControl.Undo();
    }
}
