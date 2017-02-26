using UnityEngine;
using System.Collections;

public class Altimeter : MonoBehaviour
{

    #region Inspector Vars

    [SerializeField]
    private float startingHeight;
    [SerializeField]
    private float interval;
    [SerializeField]
    private float refreshTime = .1f;
    #endregion

    [SerializeField]
    private float heightInUnits;
    
    public static float heightInMeters;    
    private float lastHeight;

    void Start()
    {
        lastHeight = heightInUnits = transform.position.y;

        heightInMeters = startingHeight;

        StartCoroutine(CheckHeight());
    }

    IEnumerator CheckHeight()
    {
        heightInUnits = transform.position.y;
        heightInMeters -= ((lastHeight - heightInUnits) * interval);

        lastHeight = heightInUnits;

        Debug.Log(heightInMeters);
        yield return new WaitForSeconds(refreshTime);
        StartCoroutine(CheckHeight());
    }

}
