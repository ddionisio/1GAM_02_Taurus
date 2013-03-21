using UnityEngine;
using System.Collections;

public class UISlot : MonoBehaviour {
    public UILabel nameLabel;
    public UILabel countLabel;
    public int slot = 0;
    public string countFormat = "{0:000}%";

    public GameObject delete;

    private bool mStarted = false;

    public void RefreshLabels() {
        //check if slot is available
        string name = UserSlotData.GetSlotName(slot);
        if(!string.IsNullOrEmpty(name)) {
            StopAllCoroutines();
            StartCoroutine(SetNameDelay(name));

            //determine level count
            float c = (float)UserSlotData.GetSlotValueInt(slot, LevelConfig.levelCountKey, 0);
            float sc = (float)UserSlotData.GetSlotValueInt(slot, LevelConfig.secretCountKey, 0);
            float cMax = (float)LevelConfig.instance.numLevels;

            int percent = Mathf.RoundToInt(((c+sc)/(cMax*2.0f))*100.0f);

            countLabel.text = string.Format(countFormat, percent);
            countLabel.gameObject.SetActive(true);

            delete.SetActive(true);
        }
        else {
            StopAllCoroutines();
            StartCoroutine(SetNameDelay(null));

            countLabel.gameObject.SetActive(false);

            delete.SetActive(false);
        }
    }

    void OnEnable() {
        if(mStarted) {
            RefreshLabels();
        }
    }

	// Use this for initialization
	void Start () {
        mStarted = true;

        UIEventListener.Get(delete).onClick = OnDeleteClick;

        RefreshLabels();
	}

    IEnumerator SetNameDelay(string name) {
        yield return new WaitForFixedUpdate();

        if(name == null) {
            NGUIGameLocalize localizer = nameLabel.GetComponent<NGUIGameLocalize>();
            if(localizer != null)
                localizer.Localize();
        }
        else {
            nameLabel.text = name;
        }

        yield break;
    }

    void OnDeleteClick(GameObject go) {
        if(UserSlotData.IsSlotAvailable(slot)) {
            string title = GameLocalize.GetText("eraseSlotTitle");
            title = string.Format(title, nameLabel.text);

            UIModalConfirm.Open(title, null, OnDeleteConfirm);
        }
    }

    void OnDeleteConfirm(bool yes) {
        if(yes) {
            UserSlotData.DeleteSlot(slot);
            UserSlotData.SetSlotValueInt(slot, LevelConfig.levelCountKey, 0);
            UserSlotData.SetSlotValueInt(slot, LevelConfig.secretCountKey, 0);

            RefreshLabels();
        }
    }
}
