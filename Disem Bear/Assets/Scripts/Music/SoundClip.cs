using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Music
{
    [System.Serializable]
    public class SoundClip
    {
        public AudioClip audioClip;
        public bool isLoop = false;
    }
}
