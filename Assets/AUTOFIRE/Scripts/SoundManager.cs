using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    public static SoundManager sharedInstance = null;

    public AudioSource sfxAuidoSource;
    [SerializeField] private AudioSource backgroundAudioSource;
    [SerializeField] private AudioSource InGameBackgroundAudioSource;
    public bool soundOn;
    //=========================================
    public List<AudioClip> shootSFX;
    public List<AudioClip> botKillSFX;
    public AudioClip sniperShotSFX;
    public AudioClip enemyShootSFX;
    public AudioClip enemyCloneSFX;
    public AudioClip coinPickSFX;
    public AudioClip healthPickSFX;
    public AudioClip tripleFirePickSFX;
    public AudioClip portalSFX;
    public AudioClip footstepSFX;
    [Header("LumberCraft")]
    public AudioClip buildProcessSFX;
    public AudioClip buildCompleteSFX;
    public List<AudioClip> treeCutSFX;
    public List<AudioClip> treeCutGruntSFX;
    public List<AudioClip> enemyAttackSFX;
    public AudioClip hitStoneSFX;
    public AudioClip playBtnSFX;
    public AudioClip goldenTreeSpawnSFX;
    [Header("Enemy Audio")]
    public AudioClip followguySFX;
    public AudioClip greenShooterSFX;
    public AudioClip bomberSFX;
    public AudioClip sniperSFX;
    public AudioClip giantSkeletonSFX;
    public AudioClip doubleSkeletonSFX;

    public static SoundManager SharedManager()
    {
        return sharedInstance;
    }

    private void Awake()
    {
        if (sharedInstance == null)
        {
            sharedInstance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

    }
    // Start is called before the first frame update
    void Start()
    {
        soundOn = true;
        sfxAuidoSource = GetComponent<AudioSource>();
        
        //backgroundAudioSource =  GetComponent<AudioSource>();

        //if (backgroundAudioSource != null)
        PlayMainMenuAudio(); 
    }
    public void PlaySFX(AudioClip audioClip)
    {
        sfxAuidoSource.PlayOneShot(audioClip);
    }
    public void PlayMainMenuAudio()
    {
        backgroundAudioSource.gameObject.SetActive(true);
    }
    public void StopMainMenuAudio()
    {
        backgroundAudioSource.gameObject.SetActive(false);
    }
    public void PlayIngameBGAudio()
    {
        InGameBackgroundAudioSource.gameObject.SetActive(true);
    }
    public void StopIngameBGAudio()
    {
        InGameBackgroundAudioSource.gameObject.SetActive(false);
    }
    //=========================================
    public AudioClip GetRandomShootSFX()
    {
        return shootSFX[Random.Range(0, shootSFX.Count)];
    }
    public AudioClip GetRandomBotkillSFX()
    {
        return botKillSFX[Random.Range(0, botKillSFX.Count)];
    }
    public AudioClip GetRandomTreeCutSFX()
    {
        return treeCutSFX[Random.Range(0, treeCutSFX.Count)];
    }
    public AudioClip GetRandomTreeCutGruntSFX()
    {
        return treeCutGruntSFX[Random.Range(0, treeCutGruntSFX.Count)];
    }
    public AudioClip GetRandomEnemyAttackSFX()
    {
        return enemyAttackSFX[Random.Range(0, enemyAttackSFX.Count)];
    }
}
