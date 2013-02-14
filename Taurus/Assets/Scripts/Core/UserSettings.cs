using UnityEngine;
using System.Collections;

public class UserSettings {
    public const string muteKey = "m";
    public const string volumeKey = "v";
    public const string musicEnableKey = "mus";
    public const string languageKey = "l";

    public bool isMute {
        get { return mMute; }

        set {
            if(mMute != value) {
                mMute = value;
                PlayerPrefs.SetInt(muteKey, mMute ? 1 : 0);

                ApplyAudioSettings();

                RelaySettingsChanged();
            }
        }
    }

    public bool isMusicEnable {
        get { return mMusicEnabled; }

        set {
            if(mMusicEnabled != value) {
                mMusicEnabled = value;
                PlayerPrefs.SetInt(musicEnableKey, mMusicEnabled ? 1 : 0);

                RelaySettingsChanged();
            }
        }
    }

    public float volume {
        get { return mVolume; }

        set {
            if(mVolume != value) {
                mVolume = value;
                PlayerPrefs.SetFloat(volumeKey, mVolume);

                ApplyAudioSettings();
            }
        }
    }

    public GameLanguage language {
        get { return mLanguage; }
        set {
            if(mLanguage != value) {
                mLanguage = value;
                PlayerPrefs.SetInt(languageKey, (int)mLanguage);
            }
        }
    }

    //need to debug while listening to music
#if UNITY_EDITOR
    private const int muteDefault = 1;
#else
	private const int muteDefault = 0;
#endif

    private const int musicEnableDefault = 1;

    private bool mMute;
    private float mVolume;
    private bool mMusicEnabled;
    private GameLanguage mLanguage = GameLanguage.English;

    // Use this for initialization
    public UserSettings() {
        //load settings
        mVolume = PlayerPrefs.GetFloat(volumeKey, 1.0f);

        mMute = PlayerPrefs.GetInt(muteKey, muteDefault) > 0;

        mMusicEnabled = PlayerPrefs.GetInt(musicEnableKey, musicEnableDefault) > 0;

        ApplyAudioSettings();

        mLanguage = (GameLanguage)PlayerPrefs.GetInt(languageKey, (int)GameLanguage.English);
    }

    private void ApplyAudioSettings() {
        AudioListener.volume = mMute ? 0.0f : mVolume;
    }

    private void RelaySettingsChanged() {
        SceneManager.RootBroadcastMessage("UserSettingsChanged", this, SendMessageOptions.DontRequireReceiver);
    }
}
