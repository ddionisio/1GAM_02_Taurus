using UnityEngine;
using System.Collections;

public class UISlot : MonoBehaviour {
    public UILabel nameLabel;
    public UILabel countLabel;
    public int slot = 0;
    public string countFormat = "{0:000}";

    private bool mStarted = false;

    void OnEnable() {
        if(mStarted) {
            RefreshLabels();
        }
    }

	// Use this for initialization
	void Start () {
        mStarted = true;

        RefreshLabels();
	}

    IEnumerator SetNameDelay(string name) {
        yield return new WaitForFixedUpdate();

        nameLabel.text = name;

        yield break;
    }

    private void RefreshLabels() {
        //check if slot is available
        string name = UserSlotData.GetSlotName(slot);
        if(!string.IsNullOrEmpty(name)) {
            StartCoroutine(SetNameDelay(name));

            //determine level count
            int c = UserSlotData.GetSlotValueInt(slot, LevelConfig.levelCountKey, 0);
            countLabel.text = string.Format(countFormat, c);
            countLabel.gameObject.SetActive(true);
        }
        else {
            countLabel.gameObject.SetActive(false);
        }
    }
}
