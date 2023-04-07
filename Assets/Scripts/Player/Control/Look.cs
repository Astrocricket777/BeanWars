using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Astrocricket.BeanWars
{
    public class Look : MonoBehaviourPunCallbacks
    {
        #region Variables;

        public static bool CursorLock = true;

        public Transform Player;
        public Transform Cams;
        public Transform Weapon;

        public float XSensitivity;
        public float YSensitivity;
        public float MaxAngle;

        private Quaternion CamCenter;

        #endregion

        void Start()
        {

            CamCenter = Cams.localRotation;
        }

        void Update()
        {
            if (!photonView.IsMine)
            {
                return;
            }

            SetY();
            SetX();
            UpdateCursorLock();
        }

        void SetY()
        {
            float t_Input = Input.GetAxis("Mouse Y") * YSensitivity * Time.deltaTime;

            Quaternion t_adj = Quaternion.AngleAxis(t_Input, -Vector3.right);
            Quaternion t_Delta = Cams.localRotation * t_adj;

            if (Quaternion.Angle(CamCenter, t_Delta) < MaxAngle)
            {
                Cams.localRotation = t_Delta;
            }

            Weapon.rotation = Cams.rotation;
        }

        void SetX()
        {
            float t_Input = Input.GetAxis("Mouse X") * XSensitivity * Time.deltaTime;

            Quaternion t_adj = Quaternion.AngleAxis(t_Input, Vector3.up);
            Quaternion t_Delta = Player.localRotation * t_adj;

            Player.localRotation = t_Delta;
        }

        void UpdateCursorLock()
        {
            if (CursorLock)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    CursorLock = false;
                }
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    CursorLock = true;
                }
            }
        }
    }
}

