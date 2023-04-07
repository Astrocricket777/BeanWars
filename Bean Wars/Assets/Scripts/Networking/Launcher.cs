using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Com.Astrocricket.BeanWars
{
    public class Launcher : MonoBehaviourPunCallbacks
    {
        public MainMenuScript MM;

        public void Awake()
        {
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log("Connected!");

            base.OnConnectedToMaster();
        }

        public override void OnJoinedRoom()
        {
            StartGame();
        }

        public void Join()
        {
            PhotonNetwork.JoinRoom(MM.JoinRoomInput.text);
        }

        public void Create()
        {
            PhotonNetwork.CreateRoom(MM.CreateRoomInput.text);
        }

        public void StartGame()
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                PhotonNetwork.LoadLevel(1);
            }
        }
    }

}