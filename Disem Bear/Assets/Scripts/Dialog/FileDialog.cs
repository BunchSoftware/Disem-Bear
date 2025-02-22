using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game.LDialog
{
    [CreateAssetMenu(fileName = "New Dialog", menuName = "Dialog")]
    public class FileDialog : ScriptableObject
    {
        public List<DialogPoint> dialogPoints = new List<DialogPoint>();
    }
}
