using UnityEngine;
using System.Collections;

public class UIModalClickOpen : MonoBehaviour {
    public string modal;

    void OnClick() {
        if(!UIModalManager.instance.ModalIsInStack(modal))
            UIModalManager.instance.ModalOpen(modal);
    }
}
