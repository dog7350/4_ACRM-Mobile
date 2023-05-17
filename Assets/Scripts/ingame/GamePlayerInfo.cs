using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Fusion;

public class GamePlayerInfo : NetworkBehaviour
{
    public static GamePlayerInfo instance = null;
    public NetworkRunner runner;

    private void Awake()
    {
        instance = this;
        runner = GameObject.Find("NetworkRunner").GetComponent<NetworkRunner>();
    }

    [Networked] public string createId { get; set; }
    public int spNum;
}
