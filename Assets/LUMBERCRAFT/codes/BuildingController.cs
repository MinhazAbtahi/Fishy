using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum BuildingType
{
    house,
    bridge,
    lumberMill,
    shop,
    housecm,
    Small_Island
}
public class BuildingController : MonoBehaviour
{
    public BuildingType buildingType;
    public int requiredWood;
    public int hasWood;
    public bool isPending;
    public bool isComplete;
    public GameObject[] buildStages;
    public TextMeshPro txtReqWood;
    public GameObject buildCanvas;
    public RadialProgress radialProgress;
    public int bridgeID;
    public int homeID;
    public GameObject bridgeCollider;
    public GameObject tileToActivate;


    [Header("Info")]
    private Vector3 _startPos;
    private float _timer;
    private Vector3 _randomPos;

    [Header("Settings")]
    [Range(0f, 2f)]
    private float _time = 1f;
    [Range(0f, 2f)]
    private float _distance = 0.25f;
    [Range(0f, 0.1f)]
    private float _delayBetweenShakes = .005f;

    // Start is called before the first frame update
    void Start()
    {
        if(!isComplete)
            txtReqWood.text = requiredWood.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void shakeBuilding()
    {
        _startPos = transform.position;
        StartCoroutine(Shake());
    }

    private IEnumerator Shake()
    {
        _timer = 0f;

        while (_timer < _time)
        {
            _timer += Time.deltaTime;

            _randomPos = _startPos + (Random.insideUnitSphere * _distance);

            transform.position = _randomPos;

            if (_delayBetweenShakes > 0f)
            {
                yield return new WaitForSeconds(_delayBetweenShakes);
            }
            else
            {
                yield return null;
            }


#if !UNITY_EDITOR && UNITY_ANDROID

            Vibration.Vibrate(30);
#endif
#if !UNITY_EDITOR && UNITY_IPHONE && UNITY_IOS
 
            Vibration.VibratePop();
#endif
        }

        transform.position = _startPos;
    }

}
