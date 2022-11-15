using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField]
    Button serverButt;
    [SerializeField]
    Button hostButt;
    [SerializeField]
    Button clientButt;

    private void Awake()
    {
        serverButt.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartServer();
        });
        hostButt.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();
        });
        clientButt.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient();
        });
    }
}
