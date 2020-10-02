using TMPro;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class NetworkGamePlayerLobby : NetworkBehaviour
{

    [SyncVar]
    private string displayName = "Loading...";
    [SyncVar]
    public bool IsScientist;

    private NetworkManagerLobby room;
    private NetworkManagerLobby Room
    {
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as NetworkManagerLobby;
        }
    }

    public override void OnStartClient()
    {
        DontDestroyOnLoad(gameObject);
        Room.GamePlayers.Add(this);
    }

    public override void OnNetworkDestroy()
    {
        Room.GamePlayers.Remove(this);
    }

    [Server]
    public void SetDisplayName(string displayName)
    {
        this.displayName = displayName;
    }

    public void SetIsScientist(bool isScientist)
    {
        this.IsScientist = isScientist;
    }
}
