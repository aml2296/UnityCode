using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetworkController : NetworkBehaviour
{
    [SerializeField] GameObject serverObjectPrefab;
    [SerializeField] GameObject playerObjectPrefab;

    [SerializeField] private CharacterController controller;
    NetworkVariable<ulong> serverObjNet_ID = new NetworkVariable<ulong>(default(ulong));
    NetworkVariable<bool> serverLoaded = new NetworkVariable<bool>(false);
    
    PlayerActor actorData;
    PlayerController clientData;


    public override void OnNetworkSpawn()
    {
        string name = "id#" + this.NetworkObjectId.ToString();
        Transform spawn = NetworkManager.gameObject.GetComponent<NetworkSpawnPoint>().spawnPoints[0];
        gameObject.name = name;
        if (IsServer)
        {
            GameObject player = Instantiate(serverObjectPrefab, spawn.position, spawn.rotation);
            player.name = IsClient == true ? "Client: " + name : "Server: " + name;

            controller = player.GetComponent<CharacterController>();
            actorData = player.GetComponent<PlayerActor>();
            actorData.Start();
            actorData.ListenToInput = false;
            player.layer = LayerMask.NameToLayer("Server");
            NetworkObject serverObj = player.GetComponent<NetworkObject>();
            serverObj.Spawn(true);
            serverObj.ChangeOwnership(OwnerClientId);
            serverObjNet_ID.Value = serverObj.NetworkObjectId;
            serverLoaded.Value = true;
        }
        StartCoroutine(HandleOwner(spawn));
    }
    IEnumerator HandleOwner(Transform spawn)
    {
        if (IsOwner)
        {
            while (serverLoaded.Value == false)
            { yield return new WaitForEndOfFrame(); }
            GameObject playerOwnerObj = Instantiate(playerObjectPrefab, spawn.position, spawn.rotation);
            playerOwnerObj.layer = LayerMask.NameToLayer("Owner");
            playerOwnerObj.name = "PlayerController: " + name;
            clientData = playerOwnerObj.GetComponent<PlayerController>();
            NetworkObject serverObj = NetworkManager.SpawnManager.SpawnedObjects[serverObjNet_ID.Value];
            serverObj.GetComponent<ServerEntity>().setClientObject(playerOwnerObj.transform);
            actorData = serverObj.GetComponent<PlayerActor>();
        }
    }
    public void Update()
    {
        if (IsOwner)
        {
            SendRotationPackageServerRpc(Time.frameCount, clientData.GetRotAxis());
            SendInputPackageServerRpc(Time.frameCount, clientData.getMoveInput(), null);
            
            float distance = Mathf.Abs(Vector3.Distance(clientData.transform.position, actorData.transform.position));

            if (distance > 0.001f)
            {
                ConsoleHandler.Log(distance + "|" + clientData.transform.position + " into " + actorData.transform.position);
                clientData.transform.position = actorData.transform.position;
            }
        }
    }

    [ServerRpc]
    public void SendRotationPackageServerRpc(int _frame, Vector2 _axisData)
    {
        actorData.SetRotAxis(_axisData.x, _axisData.y);
    }
    [ServerRpc]
    public void SendInputPackageServerRpc(int _frame, Vector2 _inputDirectionData, bool[] _inputActionData)
    {
        actorData.SetInput(_inputDirectionData);
        actorData.setInputActions(_inputActionData);
    }

}
