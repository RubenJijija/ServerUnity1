using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class NetworkUI : MonoBehaviour
{
    public Button hostButton;
    public Button serverButton;
    public Button clientButton;

    void Start()
    {
        hostButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost(); // Host juega
        });

        serverButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartServer(); // Solo servidor
        });

        clientButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient(); // Cliente remoto
        });


    }
}
