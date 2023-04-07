using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Astrocricket.BeanWars
{
    public class Sway : MonoBehaviour
    {
        public float Intensity;
        public float Smooth;
        public bool IsMine;

        private Transform Player;
        Quaternion OriginRotation;

        void Start()
        {
            Player = transform.root;
            OriginRotation = transform.localRotation;
        }

        void Update()
        {   
            UpdateSway();
        }

        void UpdateSway()
        {
            float t_XMouse = Input.GetAxis("Mouse X");
            float t_YMouse = Input.GetAxis("Mouse Y");

            if (!IsMine)
            {
                t_XMouse = 0;
                t_YMouse = 0;
            }

            Quaternion t_x_adj = Quaternion.AngleAxis(-Intensity * t_XMouse, Vector3.up);
            Quaternion t_y_adj = Quaternion.AngleAxis(Intensity * t_YMouse, Vector3.up);

            Quaternion TargetRotation = OriginRotation * t_x_adj * t_y_adj;

            transform.localRotation = Quaternion.Lerp(transform.localRotation, TargetRotation, Time.deltaTime * Smooth);
        }
    }
}