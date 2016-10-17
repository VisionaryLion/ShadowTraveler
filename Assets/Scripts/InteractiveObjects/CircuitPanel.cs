using UnityEngine;
using System.Collections;
using Actors;

[RequireComponent(typeof(BoxCollider2D))]
public class CircuitPanel : MonoBehaviour {

    PlayerActor player;
    [SerializeField]
    GameObject enable;
    [SerializeField]
    GameObject disable;

    void Awake()
    {
        player = ActorDatabase.GetInstance().FindFirst<PlayerActor>();
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if(coll.CompareTag("Player"))
        {
            if (player.EquipmentManager.equipmentSpawnPointLeft.FindChild("Circuit") != null)
            {
                enable.SetActive(true);
                disable.SetActive(false);
                GameObject circuit = player.EquipmentManager.equipmentSpawnPointLeft.FindChild("Circuit").gameObject;

                circuit.transform.SetParent(transform);
                circuit.transform.rotation = Quaternion.identity;

                Vector3 position = new Vector3(0.005f, 0.135f, 0);
                circuit.transform.localPosition = position;


            }
        }
    }	
}
