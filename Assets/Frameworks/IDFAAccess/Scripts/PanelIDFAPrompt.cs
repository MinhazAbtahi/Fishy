using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

namespace FPG {
    public class PanelIDFAPrompt : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI txtTitle, txtMsg;
        [SerializeField] Button btnAllow, btnNotNow;
        [SerializeField] bool destroyParent = false;

        private void Start()
        {
            btnAllow.onClick.AddListener(BtnAllowCallback);
            btnNotNow.onClick.AddListener(BtnNotNowCallback);
        }

        private void OnEnable()
        {
            txtMsg?.SetText(TagManager.GetIDFAPromptMessage());
        }

        private void BtnAllowCallback()
        {
            ClosePanel();
            IDFAAccessController.UserAllowedIDFAPopUp();
        }

        private void BtnNotNowCallback()
        {
            ClosePanel();
            IDFAAccessController.UserPostponedIDFAPopUp();
        }

        private void ClosePanel()
        {
            if (destroyParent) Destroy(gameObject.transform.parent.gameObject);
            else Destroy(this.gameObject);
        }

        private static GameObject promptObject;
        public static void ShowUnityIDFAPrompt()
        {
            if (promptObject != null) Destroy(promptObject);

            var idfaPromptCamvas = Resources.Load("CanvasIDFAPrompt") as GameObject;
            promptObject = Instantiate(idfaPromptCamvas);
        }
    }
}