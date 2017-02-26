using UnityEngine;
using System.Collections;

public class DeleteMeOnTouch : MonoBehaviour {

    public string attacker;
    public GameObject guard01;
    public GameObject guard02;
    public GameObject Monster;
    
       
    public void DeleteGuard01()
    {
        guard01.SetActive(false);
    }

    public void DeleteGuard02()
    {
        guard02.SetActive(false);
    }
}
