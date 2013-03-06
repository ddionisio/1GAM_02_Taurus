using UnityEngine;
using System.Collections;

public class ModalOptions : UIController {
    public UISlider volumeSlider;
    public UICheckbox sound;
    public UICheckbox music;

    protected override void OnActive(bool active) {
        if(active) {
            volumeSlider.sliderValue = Main.instance.userSettings.volume;
            sound.isChecked = Main.instance.userSettings.isSoundEnable;
            music.isChecked = Main.instance.userSettings.isMusicEnable;

            volumeSlider.onValueChange += OnVolumeChange;
            sound.onStateChange += OnMuteSoundChange;
            music.onStateChange += OnMuteMusicChange;
        }
        else {
            volumeSlider.onValueChange -= OnVolumeChange;
            sound.onStateChange -= OnMuteSoundChange;
            music.onStateChange -= OnMuteMusicChange;
        }
    }

    protected override void OnOpen() {
    }

    protected override void OnClose() {
    }

    void OnVolumeChange(float val) {
        Main.instance.userSettings.volume = val;
    }

    void OnMuteSoundChange(bool state) {
        Main.instance.userSettings.isSoundEnable = state;
    }

    void OnMuteMusicChange(bool state) {
        Main.instance.userSettings.isMusicEnable = state;
    }
}
