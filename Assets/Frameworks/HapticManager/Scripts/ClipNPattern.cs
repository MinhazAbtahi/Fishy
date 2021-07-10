using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPG
{
    [CreateAssetMenu(fileName = "ClipNPattern_", menuName = "HapticScriptables/ClipNPattern")]
    public class ClipNPattern : ScriptableObject
    {
        [SerializeField] public AudioClip audioClip;
        [SerializeField] public string hapticPattern;
    }
}