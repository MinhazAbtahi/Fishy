using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace FPG
{
    public class PanelHapticTester : MonoBehaviour
    {
        [SerializeField] List<ClipNPattern> cNps; 
        [SerializeField] GameObject btnPrefab;
        [SerializeField] TMP_InputField inputPattern;
        private List<Button> btns;

        AudioSource audiosrc;

        private void Awake()
        {
            audiosrc = FindObjectOfType<AudioSource>();

            if (cNps != null)
            {
                btns = new List<Button>();
                for (int i = 0; i < cNps.Count; i++)
                {
                    GameObject btnGo = Instantiate(btnPrefab, gameObject.transform);
                    Button btn = btnGo.GetComponent<Button>();

                    ClipNPattern cnp = cNps[i];
                    btn.onClick.AddListener(() => { PleasePlay(cnp); } );

                    btnGo.GetComponentInChildren<TextMeshProUGUI>().text = cNps[i].audioClip.name;

                    btns.Add(btn);
                }
            }

            btnPrefab.SetActive(false);
        }

        void PleasePlay(ClipNPattern cnp)
        {

            Debug.Log("name: " + cnp.audioClip.name);
            Debug.Log("length: " + cnp.audioClip.length);
            DisableBtns();
            StartCoroutine(PlayClip(cnp.audioClip));

            var pattern = string.IsNullOrEmpty(inputPattern.text) ? cnp.hapticPattern : inputPattern.text;
            Debug.Log("pattern: " + pattern);
            HapticController.GenerateHapticFromPattern(pattern);

            StartCoroutine(EnableBtns(cnp.audioClip.length));
        }

        IEnumerator PlayClip(AudioClip clip)
        {
            audiosrc.clip = clip;
            audiosrc.Play(0);
            yield return null;
        }


        IEnumerator EnableBtns(float delay)
        {
            if (btns == null) yield return null;
            yield return new WaitForSeconds(delay + 0.1f);

            for (int i = 0; i < btns.Count; i++)
            {
                btns[i].interactable = true;
            }

        }

        void DisableBtns()
        {
            if (btns == null) return;

            for (int i = 0; i < btns.Count; i++)
            {
                btns[i].interactable = false;
            }
        }

        public void AddCharacterToPattern(string character)
        {
            inputPattern.text += character;
        }
        public void EraseCharacterFromPattern()
        {
            inputPattern.text = inputPattern.text.Substring(0, inputPattern.text.Length - 1);
        }
    }
}