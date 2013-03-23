﻿using UnityEngine;
using System.Collections;

/// <summary>
/// Set given 'select' to UICamera.selectObject upon modal active.
/// </summary>
[RequireComponent(typeof(UIController))]
public class UIModalActiveSelectNGUI : MonoBehaviour {

    public GameObject select;

    private UIController mController;

    void OnDestroy() {
        if(mController != null) {
            mController.onActiveCallback -= UIActive;
        }
    }

    void Awake() {
        mController = GetComponent<UIController>();
        if(mController != null) {
            mController.onActiveCallback += UIActive;
        }
    }

    void UIActive(bool active) {
        if(active && select.activeInHierarchy) {
            UICamera.selectedObject = select;
        }
    }
}