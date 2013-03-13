using UnityEngine;
using System.Collections;

public class NGUIPage : MonoBehaviour {

    public UIButton prevButton;
    public UIButton nextButton;

    public GameObject[] pages;

    private int mCurPageInd = 0;

    void Awake() {
        if(prevButton != null)
            UIEventListener.Get(prevButton.gameObject).onClick += PrevClick;

        if(nextButton != null)
            UIEventListener.Get(nextButton.gameObject).onClick += NextClick;
    }

    // Use this for initialization
    void Start() {
        if(prevButton != null)
            prevButton.isEnabled = false;

        //for dynamic pages or for some reason there's just one page
        if(pages.Length <= 1 && nextButton != null)
            nextButton.isEnabled = false;

        if(pages.Length >= 1) {
            pages[0].SetActive(true);

            for(int i = 1; i < pages.Length; i++) {
                pages[i].SetActive(false);
            }
        }
    }

    // Update is called once per frame
    void Update() {

    }

    void PrevClick(GameObject go) {
        if(mCurPageInd > 0) {
            //TODO: fancy transition
            pages[mCurPageInd].SetActive(false);

            mCurPageInd--;

            pages[mCurPageInd].SetActive(true);

            if(mCurPageInd == 0 && prevButton != null) {
                prevButton.isEnabled = false;
            }

            if(nextButton != null && !nextButton.isEnabled)
                nextButton.isEnabled = true;
        }
    }

    void NextClick(GameObject go) {
        if(mCurPageInd < pages.Length-1) {
            //TODO: fancy transition
            pages[mCurPageInd].SetActive(false);

            mCurPageInd++;

            pages[mCurPageInd].SetActive(true);

            if(mCurPageInd == pages.Length-1 && nextButton != null)
                nextButton.isEnabled = false;

            if(prevButton != null && !prevButton.isEnabled) {
                prevButton.isEnabled = true;
            }
        }
    }
}
