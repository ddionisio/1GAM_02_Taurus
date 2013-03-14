using UnityEngine;
using System.Collections;

/// <summary>
/// Use this to simulate a click with an ngui widget using input manager.
/// </summary>
public class InputNGUIClick : MonoBehaviour {
    public InputAction action;
    public InputAction alternate = InputAction.NumAction;

    /// <summary>
    /// Check to see if this object is selected via NGUI for input to process.
    /// </summary>
    public bool checkSelected;

    private bool mStarted;

    void OnEnable() {
        if(mStarted && Main.instance != null && Main.instance.input != null) {
            Main.instance.input.AddButtonCall(action, OnInputEnter);

            if(alternate != InputAction.NumAction)
                Main.instance.input.AddButtonCall(alternate, OnInputEnter);
        }

    }

    void OnDisable() {
        if(mStarted && Main.instance != null && Main.instance.input != null) {
            Main.instance.input.RemoveButtonCall(action, OnInputEnter);

            if(alternate != InputAction.NumAction)
                Main.instance.input.RemoveButtonCall(alternate, OnInputEnter);
        }
    }

	// Use this for initialization
	void Start () {
        mStarted = true;
        if(Main.instance != null && Main.instance.input != null) {
            Main.instance.input.AddButtonCall(action, OnInputEnter);

            if(alternate != InputAction.NumAction)
                Main.instance.input.AddButtonCall(alternate, OnInputEnter);
        }
	}
	
    void OnInputEnter(InputManager.Info data) {
        if(data.state == InputManager.State.Pressed && (!checkSelected || UICamera.selectedObject == gameObject)) {
            UICamera.Notify(gameObject, "OnClick", null);
        }
    }
}
