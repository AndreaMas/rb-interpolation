using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using Photon.Pun;
//using Photon.PunBehaviour;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class MyPhotonView : MonoBehaviourPunCallbacks, IPunObservable
{
    // GameObjs to sync
    [SerializeField] private GameObject playerCube;
    [SerializeField] private GameObject smallCubes;

    // Components
    private Transform playerTransform;
    private Rigidbody[] smallCubesRb;
    private ClientSmoothMovement playerCSM;
    private ClientSmoothMovement[] smallCubesCSM;
    
    // Other
    private bool readyToSync = false; // to avoid sync before Rb set to kinematic

    private void Start()
    {
        Debug.Log("[MY INFO] MyPhotonView Awakened.");

        // check if player and cubes references are missing
        if (smallCubes == null)
            Debug.Log("[MY WARNING] MyPhotonView misses playerCube and smallCubes references!");
        if (playerCube == null)
            Debug.Log("[MY WARNING] MyPhotonView misses playerCube and smallCubes references!");

        // Get Components
        playerTransform = playerCube.GetComponent<Transform>();
        playerCSM = playerCube.GetComponent<ClientSmoothMovement>();
        smallCubesCSM = smallCubes.GetComponentsInChildren<ClientSmoothMovement>();
        smallCubesRb = smallCubes.GetComponentsInChildren<Rigidbody>();

        // Photon jargon
        PhotonPeer.RegisterType(typeof(CubeType), (byte)'C', CubeType.Serialize, CubeType.Deserialize);
        PhotonNetwork.ConnectUsingSettings();
    }

    float timePassedDebugInfo = 0f;
    float timeToPassDebugInfo = 1f; // display debug info every N seconds
    float timeLastDebugInfo = 0f;
    uint bytesSent = 0;
    void Update()
    {
        // display debug info every N seconds
        float currentTime = Time.time; // TODO: test PhotonNetwork.ServerTimestamp for client?
        timePassedDebugInfo = currentTime - timeLastDebugInfo;
        if (photonView.IsMine && timePassedDebugInfo > timeToPassDebugInfo)
        {
            UIManager.Instance.SetBitesSentText(bytesSent);
            Debug.Log("[MY INFO] Sent bytes/sec : " + bytesSent);
            timeLastDebugInfo = currentTime;
            bytesSent = 0; // restart counting
        }
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("[MY INFO] Client connected to Server.");
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsVisible = true;
        roomOptions.IsOpen = true;
        roomOptions.MaxPlayers = 2;
        PhotonNetwork.JoinOrCreateRoom("BruhRoom", roomOptions, TypedLobby.Default);
    }


    public override void OnJoinedRoom()
    {
        Debug.Log("[MY INFO] On Joined room called.");
        base.OnJoinedRoom();

        // enable/disable components wether instance is master client or not

        if (!photonView.IsMine)
        {
            Debug.Log("[MY INFO] PhotonView is NOT mine.");

            // Disable Canvas 
            UIManager.Instance.DisableMasterInfoCanvas();

            // Set components for client's player cube
            playerCube.GetComponent<Rigidbody>().isKinematic = true;
            playerCube.GetComponent<PlayerMovement>().enabled = false;
            playerCube.GetComponent<ClientSmoothMovement>().enabled = true;

            // Set components for client's small cubes
            foreach (Transform smallCubeT in smallCubes.transform)
            {
                smallCubeT.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                smallCubeT.gameObject.GetComponent<ClientSmoothMovement>().enabled = true;
            }
        }
        else
        {
            Debug.Log("[MY INFO] PhotonView is MINE");
        }

        readyToSync = true;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (!readyToSync) return;
        if (stream.IsWriting)
        {
            // Send 1: Player Cube position and rotation

            CubeType playerCompressed = new CubeType();
            short playerId = 0; // whatever number is fine
            playerCompressed.Compress(playerTransform.position, playerTransform.rotation, playerId, false);
            stream.SendNext(playerCompressed);
            bytesSent += CubeType.sizeBytes; // for debug

            // Send 2: Moving small cubes position and rotation

            short numMovingCubes = 0;
            List<CubeType> movingCubes = new List<CubeType>();

            short id = 0;
            foreach (Rigidbody cubeRb in smallCubesRb)
            {
                if (cubeRb.velocity != Vector3.zero) // make list of moving cubes and send only those
                {
                    numMovingCubes++;
                    CubeType cubeSnap = new CubeType();
                    cubeSnap.Compress(cubeRb.transform.position, cubeRb.transform.rotation, id, 
                        cubeRb.GetComponent<SmallCube>().touched);
                    movingCubes.Add(cubeSnap); 
                }
                id++;
            }

            stream.SendNext(numMovingCubes);
            foreach(CubeType cube in movingCubes)
            {
                stream.SendNext(cube);
                bytesSent += CubeType.sizeBytes;
            }
                
        }

        if (stream.IsReading)
        {
            // Receve 1: Player Cube position and rotation

            CubeType playerCompressed = (CubeType)stream.ReceiveNext();
            playerCompressed.Decompress(ref playerCSM.targetPosition, ref playerCSM.targetRotation);

            // Receve 2: moving small cubes

            short numMovingCubes = (short)stream.ReceiveNext();

            for (short i = 0; i < numMovingCubes; i++)
            {
                CubeType movingCube = (CubeType)stream.ReceiveNext();
                ClientSmoothMovement cubeCSM = smallCubesCSM[Mathf.Abs(movingCube.cubeId)];
                cubeCSM.gameObject.GetComponent<SmallCube>().Touched();
                movingCube.Decompress(ref cubeCSM.targetPosition, ref cubeCSM.targetRotation);
            }
        }
    }

    // if I were to use prediction:
    // transform.position = startPosition + transform.forward * speed(receved) * timePassed(PhotonNetwork.ServerTimestamp);

}
