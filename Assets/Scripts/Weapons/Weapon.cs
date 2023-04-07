using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Unity.VisualScripting;

namespace Com.Astrocricket.BeanWars
{
    public class Weapon : MonoBehaviourPunCallbacks
    {
        public Gun[] Loadout;
        public Transform WeaponParent;

        private int CurrentIndex;
        private GameObject CurrentWeapon;
        public GameObject BulletHolePrefab;
        public LayerMask ShootableMask;

        private float CurrentCooldown = 0;

        void Start()
        {
            if (photonView.IsMine)
            {
                photonView.RPC("Equip", RpcTarget.All, 0);
            }

            foreach(Gun a in Loadout) 
            { 
                a.Initialize();
            }
        }


        void Update()
        {
            if (photonView.IsMine && Input.GetKeyDown(KeyCode.Alpha1))
            {
                photonView.RPC("Equip", RpcTarget.All, 0);
            }

            if (CurrentWeapon != null)
            {
                if(photonView.IsMine)
                {
                    photonView.RPC("Aim", RpcTarget.All, Input.GetMouseButton(1));

                    if (Input.GetMouseButtonDown(0) && CurrentCooldown <= 0)
                    {
                        if (Loadout[CurrentIndex].FireBullet())
                        {
                            photonView.RPC("Shoot", RpcTarget.All);
                        }
                        else
                        {
                            Loadout[CurrentIndex].Reload();
                        }
                    }

                    CurrentWeapon.transform.localPosition = Vector3.Lerp(CurrentWeapon.transform.localPosition, Vector3.zero, Time.deltaTime * 4f);

                    if (CurrentCooldown > 0)
                    {
                        CurrentCooldown -= Time.deltaTime;
                    }

                    if (Input.GetKeyDown(KeyCode.R))
                    {
                        Loadout[CurrentIndex].Reload();
                    }
                }
                
                CurrentWeapon.transform.localPosition = Vector3.Lerp(CurrentWeapon.transform.localPosition, Vector3.zero, Time.deltaTime * 4f);
            }
        }

        [PunRPC]
        void Equip(int p_ind)
        {
            if (CurrentWeapon != null)
            {
                Destroy(CurrentWeapon);
            }

            CurrentIndex = p_ind;

            GameObject t_NewEquipment = Instantiate(Loadout[p_ind].Prefab, WeaponParent.position, WeaponParent.rotation, WeaponParent) as GameObject;

            t_NewEquipment.transform.localPosition = Vector3.zero;
            t_NewEquipment.transform.localEulerAngles = Vector3.zero;
            t_NewEquipment.GetComponent<Sway>().IsMine = photonView.IsMine;

            CurrentWeapon = t_NewEquipment;
        }

        [PunRPC]
        void Aim(bool IsAiming)
        {
            Transform t_Anchor = CurrentWeapon.transform.Find("Anchor");
            Transform t_StateADS = CurrentWeapon.transform.Find("States/ADS");
            Transform t_StatesHip = CurrentWeapon.transform.Find("States/Hip");

            if (IsAiming)
            {
                t_Anchor.position = Vector3.Lerp(t_Anchor.position, t_StateADS.position, Time.deltaTime * Loadout[CurrentIndex].AimSpeed);
            }
            else
            {
                t_Anchor.position = Vector3.Lerp(t_Anchor.position, t_StatesHip.position, Time.deltaTime * Loadout[CurrentIndex].AimSpeed);
            }
        }

        [PunRPC]
        void Shoot()
        {
            Transform Spawn = transform.Find("Cameras/Normal Camera");

            Vector3 Spread = Spawn.position + Spawn.forward * 1000f;
            Spread += Random.Range(-Loadout[CurrentIndex].Spread, Loadout[CurrentIndex].Spread) * Spawn.up;
            Spread += Random.Range(-Loadout[CurrentIndex].Spread, Loadout[CurrentIndex].Spread) * Spawn.right;

            Spread -= Spawn.position;
            Spread.Normalize();

            CurrentCooldown = Loadout[CurrentIndex].FireRate;

            RaycastHit Hit = new RaycastHit();

            if (Physics.Raycast(Spawn.position, Spread, out Hit, 100f, ShootableMask))
            {
                GameObject NewHole = Instantiate(BulletHolePrefab, Hit.point + Hit.normal * 0.001f, Quaternion.identity) as GameObject;
                NewHole.transform.LookAt(Hit.point + Hit.normal);

                Destroy(NewHole, 5f);

                if (photonView.IsMine)
                {
                    if (Hit.collider.gameObject.layer == 8)
                    {
                        Hit.collider.gameObject.GetPhotonView().RPC("TakeDamage", RpcTarget.All, Loadout[CurrentIndex].Damage);

                        Debug.Log("Hit Enemy");
                    }
                }
            }

            CurrentWeapon.transform.Rotate(-Loadout[CurrentIndex].Recoil, 0, 0);
            CurrentWeapon.transform.position -= CurrentWeapon.transform.forward * Loadout[CurrentIndex].Kickback;
        }

        [PunRPC]
        void TakeDamage(int p_Damage)
        {
            GetComponent<Player>().TakeDamage(p_Damage);
        }
    }
}