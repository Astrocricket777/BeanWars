using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Astrocricket.BeanWars
{
    [CreateAssetMenu(fileName = "New Gun", menuName = "Gun")]
    public class Gun : ScriptableObject
    {
        public string Name;
        public GameObject Prefab;
        public float Spread;
        public float Recoil;
        public float Kickback;
        public float AimSpeed;
        public float FireRate;
        public int Damage;
        public int Ammo;
        public int ClipSize;

        private int Clip; // Current Clip
        private int Stash; //Current Ammo

        public void Initialize()
        {
            Stash = Ammo;
            Clip = ClipSize;
        }

        public bool FireBullet()
        {
            if (Clip > 0)
            {
                Clip -= 1;
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Reload()
        {
            Stash += Clip;
            Clip = Mathf.Min(ClipSize, Stash);

            Stash -= Clip;
        }

        public int GetStash()
        {
            return Stash;
        }
        public int GetClip()
        {
            return Clip;
        }
    }
}