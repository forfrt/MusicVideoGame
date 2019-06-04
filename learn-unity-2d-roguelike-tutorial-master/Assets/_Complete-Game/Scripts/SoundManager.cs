using UnityEngine;

namespace Scripts {
    public class SoundManager : MonoBehaviour {
        private const float HighPitchRange = 1.05f; //The highest a sound effect will be randomly pitched.
        private const float LowPitchRange = .95f; //The lowest a sound effect will be randomly pitched.
        public static SoundManager Instance; //Allows other scripts to call functions from SoundManager.				

        private AudioSource _efxSource;

        private AudioSource _musicSource;

        public void StopBackgroundMusic() {
            _musicSource.Stop();
        }

        private void Awake() {
            //Check if there is already an instance of SoundManager
            if (Instance == null)
                //if not, set it to this.
                Instance = this;
            //If instance already exists:
            else if (Instance != this)
                //Destroy this, this enforces our singleton pattern so there can only be one instance of SoundManager.
                Destroy(gameObject);

            //Set SoundManager to DontDestroyOnLoad so that it won't be destroyed when reloading our scene.
            DontDestroyOnLoad(gameObject);

            _efxSource = GetComponent<AudioSource>();
            _musicSource = GetComponent<AudioSource>();
        }

        //Used to play single sound clips.
        public void PlaySingle(AudioClip clip) {
            //Set the clip of our efxSource audio source to the clip passed in as a parameter.
            _efxSource.clip = clip;

            //Play the clip.
            _efxSource.Play();
        }


        //RandomizeSfx chooses randomly between various audio clips and slightly changes their pitch.
        public void RandomizeSfx(params AudioClip[] clips) {
            //Generate a random number between 0 and the length of our array of clips passed in.
            var randomIndex = Random.Range(0, clips.Length);

            //Choose a random pitch to play back our clip at between our high and low pitch ranges.
            var randomPitch = Random.Range(LowPitchRange, HighPitchRange);

            //Set the pitch of the audio source to the randomly chosen pitch.
            _efxSource.pitch = randomPitch; // 调整音调, 避免听觉疲劳

            //Set the clip to the clip at our randomly chosen index.
            _efxSource.clip = clips[randomIndex];

            //Play the clip.
            _efxSource.Play();
        }
    }
}