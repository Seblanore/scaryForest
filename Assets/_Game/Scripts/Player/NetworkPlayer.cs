using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class NetworkPlayer : NetworkBehaviour
{
    [SerializeField] private Vector2 placementArea = new Vector2(-10.0f, 10.0f);

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        // Hack pour éviter les messages d'erreurs dans la console lié à l'abscense d'audioListener avant l'apparition du joueur
        Destroy(GameObject.Find("AudioListener"));

        DisableClientInput();
    }

    // Désactive les input des autres joueurs
    public void DisableClientInput()
    {
        if(IsClient && !IsOwner)
        {
            var clientMoveProvider = GetComponent<NetworkMoveProvider>();
            var clientControllers = GetComponentsInChildren<ActionBasedController>();
            var clientTurnProvider = GetComponent<ActionBasedSnapTurnProvider>();
            var clientHead = GetComponentInChildren<TrackedPoseDriver>();
            var clientCamera = GetComponentInChildren<Camera>();
            var clientAudioListener = GetComponentInChildren<AudioListener>();


            clientCamera.enabled = false;
            clientMoveProvider.enableInputActions = false;
            clientTurnProvider.enableTurnLeftRight = false;
            clientTurnProvider.enableTurnAround = false;
            clientHead.enabled = false;
            DestroyImmediate(clientAudioListener);

            foreach (var controller in clientControllers)
            {
                controller.enableInputActions = false;
                controller.enableInputTracking = false;
            }

        }
    }

    private void Start()
    {
        // Placement du joueur dans la zone de Spawn
        if (IsClient && IsOwner)
        {
            transform.position = new Vector3(Random.Range(placementArea.x, placementArea.y), transform.position.y, Random.Range(placementArea.x, placementArea.y));
        }
    }

    public void OnSelectGrabbable(SelectEnterEventArgs eventArgs)
    {
        if(IsClient && IsOwner)
        {
            NetworkObject networkObjectSelected = eventArgs.interactableObject.transform.GetComponent<NetworkObject>();
            if(networkObjectSelected != null)
            {
                // request ownership from the server
                RequestGrabbableOwnershipServerRpc(OwnerClientId, networkObjectSelected);
            }
        }
    }

    [ServerRpc]
    public void RequestGrabbableOwnershipServerRpc(ulong newOwnerClientId, NetworkObjectReference networkObjectReference)
    {
        if(networkObjectReference.TryGet(out NetworkObject networkObject))
        {
            Debug.Log("changing ownership : " + newOwnerClientId);
            networkObject.ChangeOwnership(newOwnerClientId);
        }
        else
        {
            Debug.Log("Unable to change ownership" + newOwnerClientId);
        }
    }
}
