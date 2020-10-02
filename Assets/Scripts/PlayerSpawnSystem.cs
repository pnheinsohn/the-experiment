using Mirror;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerSpawnSystem : NetworkBehaviour
{
    [SerializeField] private Sprite scientistSprite = null;
    [SerializeField] private GameObject playerPrefab = null;

    private static List<Transform> spawnPoints = new List<Transform>();

    private int nextIndex = 0;

    private NetworkManagerLobby room;
    private NetworkManagerLobby Room
    {
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as NetworkManagerLobby;
        }
    }

    public static void AddSpawnPoint(Transform transform)
    {
        spawnPoints.Add(transform);

        spawnPoints = spawnPoints.OrderBy(x => x.GetSiblingIndex()).ToList();
    }

    public static void RemoveSpawnPoint(Transform transform) => spawnPoints.Remove(transform);

    public override void OnStartServer() => NetworkManagerLobby.OnServerReadied += SpawnPlayer;

    [ServerCallback]
    private void OnDestroy() => NetworkManagerLobby.OnServerReadied -= SpawnPlayer;

    [Server]
    public void SpawnPlayer(NetworkConnection conn)
    {
        Transform spawnPoint = spawnPoints.ElementAtOrDefault(nextIndex);

        if (spawnPoint == null)
        {
            Debug.LogError($"Missing spawn point for player {nextIndex}");
            return;
        }
        GameObject playerInstance = Instantiate(playerPrefab, spawnPoints[nextIndex].position, spawnPoints[nextIndex].rotation);
        PlayerBehaviour playerBehaviour = playerInstance.GetComponent<PlayerBehaviour>();
        playerBehaviour.IsScientist = conn.identity.gameObject.GetComponent<NetworkGamePlayerLobby>().IsScientist;
        NetworkServer.Spawn(playerInstance, conn);

        nextIndex++;
        if (nextIndex == Room.GamePlayers.Count)
        {
            RpcSetSpritesAfterSpawn();
        }
    }

    [ClientRpc]
    private void RpcSetSpritesAfterSpawn()
    {
        var players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            PlayerBehaviour playerBehaviour = player.GetComponent<PlayerBehaviour>();
            NetworkBehaviour playerNetwork= player.GetComponent<NetworkBehaviour>();
            if (playerNetwork.hasAuthority && playerBehaviour.IsScientist)
            {
                foreach (GameObject otherPlayer in players)
                {
                    PlayerBehaviour otherPlayerBehaviour = otherPlayer.GetComponent<PlayerBehaviour>();
                    if (otherPlayerBehaviour.IsScientist)
                    {
                        SpriteRenderer playerSprite = otherPlayer.GetComponentInChildren<SpriteRenderer>();
                        playerSprite.sprite = scientistSprite;
                    }
                }
            }
        }
    }
}
