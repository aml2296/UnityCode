using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ServerTick : NetworkBehaviour
{
    [SerializeField] GameObject client;
    [SerializeField] GameObject server;

    private float timer;
    private int currentTick;
    private float minTimeBetweenTicks;
    private const float SERVER_TICK_RATE = 60f;
    private const int BUFFER_SIZE = 1024;

    
    void Start()
    {
        minTimeBetweenTicks = 1f / SERVER_TICK_RATE;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

    }

    void Update()
    {
        
    }
}
