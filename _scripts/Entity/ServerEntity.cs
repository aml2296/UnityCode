using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(PlayerActor))]
public class ServerEntity : NetworkBehaviour
{
    [SerializeField]
    Transform clientOwnedTransform;


    PlayerActor p;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        p = GetComponent<PlayerActor>();

        p.SetAxisClamp(1, 0, 0);
    }

    public void Update()
    {
        if (clientOwnedTransform == null)
            return;


        if(IsOwner)
        {
            SetRotationServerRpc(clientOwnedTransform.GetComponent<PlayerController>().GetRotAxis());
            float distance = Mathf.Abs(Vector3.Distance(transform.position, clientOwnedTransform.position));
        
            if(distance > 0.001f)
            {
                ConsoleHandler.Log(distance + "|" + clientOwnedTransform.position+ " into " + transform.position);
                clientOwnedTransform.position = transform.position;
            }
        }
    }

    [ServerRpc]
    public void SetRotationServerRpc(Vector2 axis)
    {
        Debug.Log("Help");
        ConsoleHandler.Log("Server set axis: " + axis + this.OwnerClientId);
        p.SetRotAxis(axis.x, axis.y);
        SetRotationClientRpc(p.GetRotAxis());
    }
    [ClientRpc]
    public void SetRotationClientRpc(Vector2 axis)
    {
        ConsoleHandler.Log("Client set axis: " + axis + this.OwnerClientId);
            p.SetRotAxis(axis.x, axis.y);
    }


    internal void setClientObject(Transform t)
    {
        clientOwnedTransform = t;
    }
}
