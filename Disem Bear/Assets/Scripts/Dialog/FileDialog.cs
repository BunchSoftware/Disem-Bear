using System.Collections.Generic;
using UnityEngine;

namespace Game.LDialog
{
    [CreateAssetMenu(fileName = "New Dialog", menuName = "Dialog")]
    public class FileDialog : ScriptableObject
    {
        public List<DialogPoint> dialogPoints = new List<DialogPoint>();
    }
}
