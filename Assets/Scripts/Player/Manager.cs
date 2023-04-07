using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

namespace Com.Astrocricket.BeanWars
{
    public class Manager : MonoBehaviourPunCallbacks
    {
        public TMP_Text RoomName;

        public string PlayerPrefab;
        public Transform[] SpawnPoint;

        private void Update()
        {
            RoomName.text = PhotonNetwork.CurrentRoom.Name;
        }

        private void Start()
        {

            Spawn();
        }

        public void Spawn()
        {
            Transform t_Spawn = SpawnPoint[Random.Range(0,SpawnPoint.Length)];
            PhotonNetwork.Instantiate(PlayerPrefab, t_Spawn.position, t_Spawn.rotation);
        }
    }
}