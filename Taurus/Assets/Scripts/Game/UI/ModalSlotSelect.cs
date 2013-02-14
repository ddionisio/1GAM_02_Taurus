using UnityEngine;
using System.Collections;

public class ModalSlotSelect : UIController {

    public UIEventListener[] slots;

    protected override void OnActive(bool active) {
        if(active) {
            foreach(UIEventListener slot in slots) {
                slot.onClick = SelectSlotClick;
            }
        }
        else {
            foreach(UIEventListener slot in slots) {
                slot.onClick = null;
            }
        }
    }

    protected override void OnOpen() {
    }

    protected override void OnClose() {
    }

    void SelectSlotClick(GameObject go) {
        UISlot uiSlot = go.GetComponentInChildren<UISlot>();
        if(uiSlot != null) {
            UserSlotData usd = (UserSlotData)UserData.instance;

            int slot = uiSlot.slot;

            if(UserSlotData.IsSlotAvailable(slot)) {
                //load slot
                usd.SetSlot(slot, true);

                //level select
            }
            else {
                //start new game, first change the name
                UIModalManager.UIData uid = UIModalManager.instance.ModalGetData(Modals.newGame);
                ModalNewGame modalNewGame = (ModalNewGame)uid.ui;
                modalNewGame.slot = slot;

                UIModalManager.instance.ModalOpen(Modals.newGame);
            }
        }
    }
}
