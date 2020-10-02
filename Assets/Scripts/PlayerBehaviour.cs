using Mirror;
using UnityEngine;

public class PlayerBehaviour : NetworkBehaviour
{
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
}
