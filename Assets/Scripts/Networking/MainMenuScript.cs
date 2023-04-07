using Com.Astrocricket.BeanWars;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Com.Astrocricket.BeanWars
{
    public class MainMenuScript : MonoBehaviour
    {
        public Launcher LauncherLauncher;
        public TMP_InputField JoinRoomInput;
        public TMP_InputField CreateRoomInput;

        public void JoinMatch()
        {
            LauncherLauncher.Join();
        }
        public void CreateMatch()
        {
            LauncherLauncher.Create();
        }
    }
}