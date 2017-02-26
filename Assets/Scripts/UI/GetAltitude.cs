using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GetAltitude : MonoBehaviour
{
    [SerializeField]
    Text altitudeText;
    
    void Awake()
    {
        altitudeText = GetComponentInChildren<Text>();
    }


    void OnEnable()
    {
        altitudeText.text = "Altitude : " + ((int)Altimeter.heightInMeters).ToString() + " meters";
    }


}
