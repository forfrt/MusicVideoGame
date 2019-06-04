using System;
using UnityEngine;

namespace TutorialInfo.Scripts {
    public class Readme : ScriptableObject {
        public Texture2D icon;
        public bool loadedLayout;
        public Section[] sections;
        public string title;

        [Serializable]
        public class Section {
            public string heading, text, linkText, url;
        }
    }
}