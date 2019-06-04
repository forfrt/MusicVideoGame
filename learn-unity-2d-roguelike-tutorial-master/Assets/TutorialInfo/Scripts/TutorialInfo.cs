using UnityEngine;
using UnityEngine.UI;


// Hi! This script presents the overlay info for our tutorial content, linking you back to the relevant page.
namespace TutorialInfo.Scripts {
    public class TutorialInfo : MonoBehaviour {
        // string to store Prefs Key with name of preference for showing the overlay info
        public static string ShowAtStartPrefsKey = "showLaunchScreen";

        // used to ensure that the launch screen isn't more than once per play session if the project reloads the main scene
        private static bool _alreadyShownThisSession;

        // store a reference to the audio listener in the scene, allowing for muting of the scene during the overlay
        public AudioListener mainListener;

        // store the GameObject which renders the overlay info
        public GameObject overlay;

        // allow user to choose whether to show this menu 
        public bool showAtStart = true;

        // store a reference to the UI toggle which allows users to switch it off for future plays
        public Toggle showAtStartToggle;

        // location that Visit Tutorial button sends the user
        public string url;

        private void Awake() {
            // have we already shown this once?
            if (_alreadyShownThisSession) {
                StartGame();
            }
            else {
                _alreadyShownThisSession = true;

                // Check player prefs for show at start preference
                if (PlayerPrefs.HasKey(ShowAtStartPrefsKey)) showAtStart = PlayerPrefs.GetInt(ShowAtStartPrefsKey) == 1;

                // set UI toggle to match the existing UI preference
                showAtStartToggle.isOn = showAtStart;

                // show the overlay info or continue to play the game
                if (showAtStart)
                    ShowLaunchScreen();
                else
                    StartGame();
            }
        }

        // show overlay info, pausing game time, disabling the audio listener 
        // and enabling the overlay info parent game object
        public void ShowLaunchScreen() {
            Time.timeScale = 0f;
            mainListener.enabled = false;
            overlay.SetActive(true);
        }

        // open the stored URL for this content in a web browser
        public void LaunchTutorial() {
            Application.OpenURL(url);
        }

        // continue to play, by ensuring the preference is set correctly, the overlay is not active, 
        // and that the audio listener is enabled, and time scale is 1 (normal)
        public void StartGame() {
            overlay.SetActive(false);
            mainListener.enabled = true;
            Time.timeScale = 1f;
        }

        // set the boolean storing show at start status to equal the UI toggle's status
        public void ToggleShowAtLaunch() {
            showAtStart = showAtStartToggle.isOn;
            PlayerPrefs.SetInt(ShowAtStartPrefsKey, showAtStart ? 1 : 0);
        }
    }
}