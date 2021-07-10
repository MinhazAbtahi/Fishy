using System;
using UnityEngine;
#if UNITY_IOS
using System.Runtime.InteropServices;
using UnityEngine.iOS;
#endif

namespace FPG
{
    public static class HapticController
    {
        public static bool HapticEnabled = true;
#if UNITY_IOS
        [DllImport("__Internal")]
        private static extern void _GenerateHapticFromPattern(string pattern);
        [DllImport("__Internal")]
        private static extern void _GenerateLightImpact();
        [DllImport("__Internal")]
        private static extern void _GenerateMediumImpact();
        [DllImport("__Internal")]
        private static extern void _GenerateHeavyImpact();
        [DllImport("__Internal")]
        private static extern void _GenerateSoftImpact();
        [DllImport("__Internal")]
        private static extern void _GenerateRigidImpact();
        
        [DllImport("__Internal")]
        private static extern void _StartHapticEngine();
        [DllImport("__Internal")]
        private static extern void _PlayCustomHaptic(float intensity, float sharpness, double duration, double startDelay);

        [DllImport("__Internal")]
        private static extern void _PlayWalkHaptic(double duration);

        [DllImport("__Internal")]
        private static extern void _PlayTreeChopHaptic(double duration);
        [DllImport("__Internal")]
        private static extern void _PlayTreeChopCompletedHaptic(double duration);

        [DllImport("__Internal")]
        private static extern void _PlayBuildHaptic(double duration);
        [DllImport("__Internal")]
        private static extern void _PlayBuildCompletedHaptic(double duration);

        [DllImport("__Internal")]
        private static extern void _PlayShootEnemyHaptic(double duration);
        [DllImport("__Internal")]
        private static extern void _PlayEnemyDestroyedHaptic(double duration);

#endif

        [RuntimeInitializeOnLoadMethod]
        public static void Init()
        {
            StartHapticEngine();
            Vibration.Init();
        }


        #region iOS General Haptics
        public static void GenerateHapticFromPattern(string pattern)
        {
#if UNITY_IOS && !UNITY_EDITOR
            if(HapticEnabled)_GenerateHapticFromPattern(pattern);
#else
            Debug.Log("AshLog: Only Works on iOS.");
#endif
        }

        internal static void StartHapticEngine()
        {
#if UNITY_IOS && !UNITY_EDITOR
            if(HapticEnabled)_StartHapticEngine();
#else
            Debug.Log("AshLog: StartHapticEngine Only Works on iOS.");
#endif
        }

        internal static void PlayCustomHaptic(float intensity, float sharpness, double duration, double startDelay)
        {
#if UNITY_IOS && !UNITY_EDITOR
            if(HapticEnabled)_PlayCustomHaptic(intensity, sharpness, duration, startDelay);
#else
            Debug.Log($"AshLog: PlayCustomHaptic Only Works on iOS. {intensity}-{sharpness}-{duration}-{startDelay}");
#endif
        }
        internal static void PlayDiscreteHaptic(float intensity, float sharpness, double startDelay = 0)
        {
            Debug.Log($"AshLog: PlayDiscreteHaptic Only Works on iOS. {intensity}-{sharpness}-{startDelay}");
            PlayCustomHaptic(intensity, sharpness, 0, startDelay);
        }
        internal static void PlayContinuousHaptic(float intensity, float sharpness, double duration, double startDelay = 0)
        {
            Debug.Log($"AshLog: PlayContinuousHaptic Only Works on iOS. {intensity}-{sharpness}-{duration}-{startDelay}");
            PlayCustomHaptic(intensity, sharpness, duration, startDelay);
        }

        #endregion iOS General Haptics

        #region Game Specific Haptics

        public static void PlayWalkHaptic(float length)
        {
#if UNITY_IOS && !UNITY_EDITOR
            if(HapticEnabled)_PlayWalkHaptic(length);
#elif UNITY_ANDROID && !UNITY_EDITOR
            if(HapticEnabled)Vibration.Vibrate(15);
#else
            //Debug.Log("AshLog: WalkHaptic Only Works on iOS/Android.");
#endif
        }

        internal static void PlayTreeChopHaptic(float length)
        {
#if UNITY_IOS && !UNITY_EDITOR
            //if(HapticEnabled)_PlayTreeChopHaptic(length);
            if(HapticEnabled)Vibration.VibratePop();
#elif UNITY_ANDROID && !UNITY_EDITOR
            if(HapticEnabled)Vibration.Vibrate(35);
#else
            //Debug.Log("AshLog: TreeChopHaptic Only Works on iOS/Android.");
#endif
        }
        internal static void PlayTreeChopCompletedHaptic(float length)
        {
#if UNITY_IOS && !UNITY_EDITOR
            if(HapticEnabled)_PlayTreeChopCompletedHaptic(length);
#elif UNITY_ANDROID && !UNITY_EDITOR
            
#else
            //Debug.Log("AshLog: TreeChopCompletedHaptic Only Works on iOS/Android.");
#endif
        }

        internal static void PlayBuildHaptic(float length)
        {
#if UNITY_IOS && !UNITY_EDITOR
            if(HapticEnabled)_PlayBuildHaptic(length);
#elif UNITY_ANDROID && !UNITY_EDITOR
            if(HapticEnabled)Vibration.VibratePop();
#else
            //Debug.Log("AshLog: BuildHaptic Only Works on iOS/Android.");
#endif
        }
        internal static void PlayBuildCompletedHaptic(float length)
        {
#if UNITY_IOS && !UNITY_EDITOR
            if(HapticEnabled)_PlayBuildCompletedHaptic(length);
#elif UNITY_ANDROID && !UNITY_EDITOR
            if(HapticEnabled)Vibration.VibratePeek();
#else
            //Debug.Log("AshLog: BuildCompleted Only Works on iOS/Android.");
#endif
        }

        internal static void PlayShootEnemyHaptic(float length)
        {
#if UNITY_IOS && !UNITY_EDITOR
            if(HapticEnabled)_PlayShootEnemyHaptic(length);
#elif UNITY_ANDROID && !UNITY_EDITOR
            if(HapticEnabled)Vibration.VibratePop();
#else
            //Debug.Log("AshLog: ShootEnemyHaptic Only Works on iOS/Android.");
#endif
        }
        internal static void PlayEnemyDestroyedHaptic(float length)
        {
#if UNITY_IOS && !UNITY_EDITOR
            if(HapticEnabled)_PlayEnemyDestroyedHaptic(length);
#elif UNITY_ANDROID && !UNITY_EDITOR
            if(HapticEnabled)Vibration.VibratePeek();
#else
            //Debug.Log("AshLog: EnemyDestroyedHaptic Only Works on iOS/Android.");
#endif
        }
        #endregion Game Specific Haptics
    }
}
