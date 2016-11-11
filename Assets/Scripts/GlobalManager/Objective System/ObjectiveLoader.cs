using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ObjectiveLoader : MonoBehaviour
{
    [SerializeField]
    ObjectiveData objectiveData;
    [SerializeField]
    GameObject template;
    
    Text[] objectiveTexts;

    void Awake()
    {
        objectiveTexts = GetComponentsInChildren<Text>();
    }

    void Start()
    {
        for(int i = 0; i < objectiveData.objectives.Length; i++)
        {
            GameObject temp = Instantiate(template);
            temp.transform.SetParent(transform);
            temp.transform.localScale = new Vector3(1, 1, 1);
            temp.SetActive(true);
            temp.GetComponentInChildren<Text>().text = objectiveData.objectives[i];
        }
    }
}
