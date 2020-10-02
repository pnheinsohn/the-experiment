using Mirror;
using Cinemachine;
using UnityEngine;

public class PlayerCameraController : NetworkBehaviour
{
    [Header("Camera")]
    [SerializeField] private CinemachineVirtualCamera virtualCamera = null;

    private CinemachineTransposer transposer;

    public override void OnStartAuthority()
    {
        transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        virtualCamera.gameObject.SetActive(true);
        enabled = true;

        virtualCamera.m_Lens.OrthographicSize = 150f;
    }
}
