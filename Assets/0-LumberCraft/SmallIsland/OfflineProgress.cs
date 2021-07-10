using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OfflineProgress : MonoBehaviour
{
    private string lastLoginKey = "LastLogin";
    public TimeSpan lastLoginTime;

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey(lastLoginKey))
        {
            DateTime lastLogin = DateTime.Parse(PlayerPrefs.GetString(lastLoginKey));
            lastLoginTime = DateTime.Now - lastLogin;

            string formattedTime = string.Format("{0} Hours, {1} Minutes, {2} Seconds", lastLoginTime.Hours, lastLoginTime.Minutes, lastLoginTime.Seconds);
            Debug.Log(formattedTime);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnApplicationQuit()
    {
       
        PlayerPrefs.SetString(lastLoginKey, DateTime.Now.ToString());
    }
}
