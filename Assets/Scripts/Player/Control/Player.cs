using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UIElements;

namespace Com.Astrocricket.BeanWars
{

    public class Player : MonoBehaviourPunCallbacks
    {
        [Header("Stats")]
        public float Speed;
        public float SprintModifier;
        public float JumpForce;
        public int MaxHealth;
        public float LengthOfSlide;
        public float SlideSpeed;
        public float SlideModifier;

        [Header("References")]
        public Camera NormalCam;
        public LayerMask Ground;
        public Transform GroundDetector;
        public Transform WeaponParent;
        public GameObject CameraParent;

        private Transform UI_Healthbar;

        private Manager PlayerManager;

        private Rigidbody Rig;

        private float MovementCounter;
        private float IdleCounter;
        private Vector3 TargetWeaponBobPosition;

        private float BaseFOV = 80f;
        private float SprintFOVModifier = 1.2f;

        private Vector3 WeaponParentOrigin;

        private int CurrentHealth;

        private Vector3 SlideDirection;
        private bool Sliding;
        private float SlideTime;

        void Start()
        {
            CurrentHealth = MaxHealth;

            CameraParent.SetActive(photonView.IsMine);

            PlayerManager = GameObject.Find("Manager").GetComponent<Manager>();

            if (!photonView.IsMine)
            {
                gameObject.layer = 8;
            }


            BaseFOV = NormalCam.fieldOfView;

            if (Camera.main)
            {
                Camera.main.enabled = false;
            }

            Rig = GetComponent<Rigidbody>();
            WeaponParentOrigin = WeaponParent.localPosition;

            if (photonView.IsMine)
            {

                UI_Healthbar = GameObject.Find("HUD/Health/Bar").transform;
                RefreshHealthBar();
            }
        }

        void Update()
        {
            if (!photonView.IsMine)
            {
                return;
            }

            float t_Hmove = Input.GetAxisRaw("Horizontal");
            float t_Vmove = Input.GetAxisRaw("Vertical");

            bool Sprint = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            bool Jump = Input.GetKey(KeyCode.Space);

            bool IsGrounded = Physics.Raycast(GroundDetector.position, Vector3.down, 0.1f, Ground);
            bool IsJumping = Jump && IsGrounded;
            bool IsSprinting = Sprint && t_Vmove > 0 && !IsJumping && IsGrounded;

            if (IsJumping)
            {
                Rig.AddForce(Vector3.up * JumpForce);
            }

            if (t_Hmove == 0 && t_Vmove == 0)
            {
                HeadBob(IdleCounter, .025f, .025f);
                IdleCounter += Time.deltaTime;

                WeaponParent.localPosition = Vector3.Lerp(WeaponParent.localPosition, TargetWeaponBobPosition, Time.deltaTime * 2f);
            }
            else if (!IsSprinting)
            {
                HeadBob(MovementCounter, .035f, .035f);
                MovementCounter += Time.deltaTime * 3;

                WeaponParent.localPosition = Vector3.Lerp(WeaponParent.localPosition, TargetWeaponBobPosition, Time.deltaTime * 6f);
            }
            else
            {
                HeadBob(MovementCounter, .15f, .075f);
                MovementCounter += Time.deltaTime * 7;

                WeaponParent.localPosition = Vector3.Lerp(WeaponParent.localPosition, TargetWeaponBobPosition, Time.deltaTime * 10f);
            }

            if (Input.GetKeyDown(KeyCode.U))
            {
                TakeDamage(10);
            }

            RefreshHealthBar();

        }

        void FixedUpdate()
        {
            if (!photonView.IsMine)
            {
                return;
            }

            float t_Hmove = Input.GetAxisRaw("Horizontal");
            float t_Vmove = Input.GetAxisRaw("Vertical");

            bool Sprint = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            bool Jump = Input.GetKey(KeyCode.Space);
            bool Slide = Input.GetKey(KeyCode.C);


            bool IsGrounded = Physics.Raycast(GroundDetector.position, Vector3.down, 0.1f, Ground);
            bool IsJumping = Jump && IsGrounded;
            bool IsSprinting = Sprint && t_Vmove > 0 && !IsJumping && IsGrounded;
            bool IsSliding = IsSprinting && Slide;

            Vector3 t_Direction = Vector3.zero;
            float t_AdjustedSpeed = Speed;

            if (!Sliding)
            {
                t_Direction = new Vector3(t_Hmove, 0, t_Vmove);
                t_Direction.Normalize();


                


                Vector3 t_TargetVelocity = transform.TransformDirection(t_Direction) * t_AdjustedSpeed * Time.deltaTime;
                t_TargetVelocity.y = Rig.velocity.y;

                Rig.velocity = t_TargetVelocity;

                if (IsSliding)
                {
                    Sliding = true;
                    SlideDirection = transform.TransformDirection(t_Direction);

                    SlideTime = LengthOfSlide;
                }

                if (IsSprinting)
                {
                    NormalCam.fieldOfView = Mathf.Lerp(NormalCam.fieldOfView, BaseFOV * SprintFOVModifier, Time.deltaTime * 8f);
                }
                else
                {
                    NormalCam.fieldOfView = Mathf.Lerp(NormalCam.fieldOfView, BaseFOV, Time.deltaTime * 8f);
                }
            }        
            
        }

        void HeadBob(float z, float Xintensity, float Yintensity)
        {
            TargetWeaponBobPosition = WeaponParentOrigin + new Vector3(Mathf.Cos(z) * Xintensity, Mathf.Sin(z * 2) * Yintensity, 0);
        }

        void RefreshHealthBar()
        {
            float t_health_ratio = (float)CurrentHealth / (float)MaxHealth;

            UI_Healthbar.localScale = Vector3.Lerp(UI_Healthbar.localScale, new Vector3(t_health_ratio, 1, 1), Time.deltaTime * 8f);
        }

        public void TakeDamage(int p_Damage)
        {
            if (photonView.IsMine)
            {
                CurrentHealth -= p_Damage;

                RefreshHealthBar();

                Debug.Log(CurrentHealth);
            }

            if (CurrentHealth <= 0)
            {
                PlayerManager.Spawn();
                PhotonNetwork.Destroy(gameObject);

                Debug.Log("You Died! :(");
            }
        }
    }
}