using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class CheckPoint : MonoBehaviour
{
    public GameObject SpawnPoint;
    public int CkNum;
    public static bool isReSpawn = false;
    public GameObject myInfo;

    private void Start()
    {
        myInfo = GameDirector.instance.myInfoObj;
        myInfo.GetComponent<PlayerInfo>().MyChackPointCount = 1;
    }

    public void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("PlayerCar")  && col.gameObject.GetComponent<NetworkObject>().HasStateAuthority)
        {
            myInfo.GetComponent<PlayerInfo>().MyChackPointCount = CkNum ;
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (myInfo == null) myInfo = GameDirector.instance.myInfoObj;
        if (PlayerController.isReSpawnRq == true)
        {
            isReSpawn = true;
            PlayerController.isReSpawnRq = false;
        }
    }
}
