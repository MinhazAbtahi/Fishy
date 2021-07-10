using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPG
{
    public static class TemporaryInitializer
    {
        private static bool initialised = false;
        [RuntimeInitializeOnLoadMethod]
        public static void InitStuff()
        {
            if (initialised) return;

            FirebaseManager.GetInstance().Init();
            VideoAdsManager.GetInstance().Init();

            initialised = true;
        }
    }
}