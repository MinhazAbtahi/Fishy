using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class BuildingsPopPup : MonoBehaviour
{
    public Button btnCross;

    public virtual void Start()
    {
        btnCross.onClick.AddListener(() => ClosePopUpCallBack());
    }

    public void ClosePopUpCallBack()
    {
        GameManager.instance.player.GetComponent<LumberCraft.PlayerInputController>().enabled = true;
        gameObject.SetActive(false);
    }
}

