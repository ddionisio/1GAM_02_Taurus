using UnityEngine;
using System.Collections;

public class UIModalClickStackPop : MonoBehaviour {
    void OnClick() {
        UIModalManager.instance.ModalCloseTop();
    }
}
