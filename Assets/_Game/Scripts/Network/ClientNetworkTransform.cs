using Unity.Netcode.Components;
using Unity.Netcode;

public class ClientNetworkTransform : NetworkTransform
{
    /// <summary>
    /// Used to determine who can write to this transform. Owner client only.
    /// This imposes state to the server. This is putting trust on your clients. Make sure no security-sensitive features use this transform.
    /// </summary>
    protected override bool OnIsServerAuthoritative()
    {
        return false;
    }

    /*public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        CanCommitToTransform = IsOwner;
    }

    protected override void Update()
    {
        base.Update();

        if (NetworkManager.Singleton != null && (NetworkManager.Singleton.IsConnectedClient || NetworkManager.Singleton.IsHost))
        {
            CanCommitToTransform = IsOwner;

            if (CanCommitToTransform)
            {
                TryCommitTransformToServer(transform, NetworkManager.LocalTime.Time);
            }
        }
    }*/
}

