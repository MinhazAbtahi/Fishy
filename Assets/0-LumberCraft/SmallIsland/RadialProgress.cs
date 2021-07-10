using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
 
public class RadialProgress : MonoBehaviour
{
	private string lastLoginKey = "";
	private OfflineProgress offlineProgress;
	public TextMeshProUGUI ProgressIndicator;
	public Image LoadingBar;
	float elapsedTime;
	float currentTime;
	public float time = 90;

	private bool start;
	public bool done;
	private string uid;

	// Use this for initialization
	void Start()
	{
		uid = Guid.NewGuid().ToString();
		lastLoginKey = "BuildTime" + uid;
		//offlineProgress = GetComponent<OfflineProgress>();
		//if(offlineProgress)
  //      {
		//	float lastLoginS = (float)offlineProgress.lastLoginTime.TotalSeconds;

		//	if (time < lastLoginS) currentTime = time - lastLoginS;
		//	else currentTime = 0f;
  //      }
		//elapsedTime = PlayerPrefs.GetFloat("ElapsedTime");
		//currentTime = PlayerPrefs.GetFloat("CurrentTime");
		//if (currentTime <= 0.0f) currentTime = time;

		currentTime = time;
	}

	// Update is called once per frame
	void Update()
	{
		if (!start) return;

		//transform.LookAt(Camera.main.transform);
		if (currentTime > 0)
		{
			currentTime -= Time.deltaTime;
			elapsedTime += Time.deltaTime;

			int seconds = (int)(currentTime % 60);
			int minutes = (int)(currentTime / 60);
			int hours = (int)(currentTime / 3600 % 24);

			string timeString = string.Format("{0:0}:{1:00}:{2:00}", hours, minutes, seconds);
			ProgressIndicator.text = timeString;
		}
		else
		{
            if (!done)
            {
				done = true;
				StopFill();
			}
			
			ProgressIndicator.text = "Done";
		}

		LoadingBar.fillAmount = elapsedTime / time;
	}

	public void StartFill()
    {
		start = true;
    }

	public void StopFill()
    {
		start = false;
		GameManager.instance.playerCon.FinishSmallIslandBuild();
		elapsedTime = 0f;
		currentTime = time;
		this.enabled = false;
		gameObject.SetActive(false);
		PlayerPrefs.SetFloat("CurrentTime", currentTime);
		PlayerPrefs.SetFloat("ElapsedTime", elapsedTime);
	}

    private void OnApplicationQuit()
    { 
		PlayerPrefs.SetFloat("BuildTime"+uid, currentTime);
		//PlayerPrefs.SetFloat("ElapsedTime", elapsedTime);
	}
}