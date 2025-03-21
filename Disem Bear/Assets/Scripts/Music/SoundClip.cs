using System;
using UnityEngine;

namespace Game.Music
{
    [Serializable]
    public class SoundClip
    {
        public AudioClip audioClip;
        public bool isLoop = false;
    }
}
