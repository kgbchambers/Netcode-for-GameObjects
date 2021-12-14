using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

public class PlayerController : NetworkBehaviour
{
    public NetworkVariable<float> forwardBackPosition = new NetworkVariable<float>();
    public NetworkVariable<float> leftRightPosition = new NetworkVariable<float>();


    private PlayerInput playerInputActions;
    private float speed = .15f;
    public Vector3 moveVec;
    
    private float oldForwardBack;
    private float oldLeftRight;


    private void Awake()
    {
        playerInputActions = new PlayerInput();
        playerInputActions.Enable();
    }


    void Update()
    {
        if (IsServer)
        {
            UpdateServer();
        }
        if (IsClient && IsOwner)
        {
            UpdateClient();
        }
    }


    private void UpdateServer()
    {
        transform.position = new Vector3(Mathf.Clamp(transform.position.x + leftRightPosition.Value,-25f,25f), transform.position.y, Mathf.Clamp(transform.position.z + forwardBackPosition.Value,-25,25));
    }


    private void UpdateClient()
    {
        float forwardBack = 0f;
        float leftRight = 0f;
        moveVec = playerInputActions.Player.Movement.ReadValue<Vector2>();
        if (moveVec.x < 0f)
        {
            leftRight -= speed;
        }
        if (moveVec.x > 0f)
        {
            leftRight += speed;
        }
        if (moveVec.y < 0f)
        {
            forwardBack -= speed;
        }
        if (moveVec.y > 0f)
        {
            forwardBack += speed;
        }

        if (oldForwardBack != forwardBack || oldLeftRight != leftRight)
        {
            oldForwardBack = forwardBack;
            oldLeftRight = leftRight;

            UpdateClientPositionServerRpc(forwardBack, leftRight);
        }
    }


    /*
     * Client can invoke a server RPC on a Network Object. 
     * The RPC will be placed in the local queue and then sent to the server,
     * where it will be executed on the server version of the same Network Object.
     * 
     * Developers can declare a ServerRpc by marking a method with [ServerRpc] attribute
     * and making sure to have ServerRpc suffix in the method name.
     * 
     */



    //  The below code doesn't work because it is missing "ServerRpc" on function/method name
    //    void RequestMove(ServerRpcParams rpcParams = default) 


    [ServerRpc]
    public void UpdateClientPositionServerRpc(float forwardBack, float leftRight)
    {
        forwardBackPosition.Value = forwardBack;
        leftRightPosition.Value = leftRight;
    }

}
