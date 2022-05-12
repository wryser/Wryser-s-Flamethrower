using BepInEx;
using System;
using UnityEngine;
using Utilla;
using System.Reflection;
using System.IO;
using UnityEngine.XR;
using UnityEngine.InputSystem;


namespace Flamethrower
{
    /// <summary>
    /// This is your mod's main class.
    /// </summary>

    /* This attribute tells Utilla to look for [ModdedGameJoin] and [ModdedGameLeave] */
    [ModdedGamemode]
    [BepInDependency("org.legoandmars.gorillatag.utilla", "1.5.0")]
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {
        bool inRoom;
        public static readonly string assemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        GameObject handl;
        bool rightTrigger;
        bool primButton;
        bool isflight = true;
        private readonly XRNode rNode = XRNode.RightHand;
        GameObject flame;
        float ftogglecooldown = 0.5f;
        float flighttoggle;
#pragma warning disable IDE0051 // IDE0051: Remove unused member

        void OnEnable()
        {
            /* Set up your mod here */
            /* Code here runs at the start and whenever your mod is enabled*/

            HarmonyPatches.ApplyHarmonyPatches();
            Utilla.Events.GameInitialized += OnGameInitialized;
        }

        void OnDisable()
        {
            /* Undo mod setup here */
            /* This provides support for toggling mods with ComputerInterface, please implement it :) */
            /* Code here runs whenever your mod is disabled (including if it disabled on startup)*/

            HarmonyPatches.RemoveHarmonyPatches();
            Utilla.Events.GameInitialized -= OnGameInitialized;
        }
        public void OnGameInitialized(object sender, EventArgs e)
        {
            Stream str = Assembly.GetExecutingAssembly().GetManifestResourceStream("Flamethrower.Assets.flamethrower");
            AssetBundle bundle = AssetBundle.LoadFromStream(str);
            GameObject flamethrower = bundle.LoadAsset<GameObject>("flamethrower");
            GameObject realflamethrower = Instantiate(flamethrower);
            flame = realflamethrower.transform.GetChild(0).gameObject;

            handl = GameObject.Find("OfflineVRRig/Actual Gorilla/rig/body/shoulder.R/upper_arm.R/forearm.R/hand.R/palm.01.R/");
            realflamethrower.transform.SetParent(handl.transform, false);
            realflamethrower.transform.localScale = new Vector3(4.5f, 4.5f, 4.5f);
            realflamethrower.transform.localRotation = Quaternion.Euler(0f, 270f, 90f);
            realflamethrower.transform.localPosition = new Vector3(-0.03f, 0.13f, - 0.075f);
            flame.SetActive(false);
        }

        void FixedUpdate()
        {
            /* Code here runs every frame when the mod is enabled */
            InputDevices.GetDeviceAtXRNode(rNode).TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out rightTrigger);
            InputDevices.GetDeviceAtXRNode(rNode).TryGetFeatureValue(UnityEngine.XR.CommonUsages.primaryButton, out primButton);

            if (rightTrigger)
            {
                flame.SetActive(true);
                if (inRoom && isflight)
                {
                    GorillaLocomotion.Player.Instance.GetComponent<Rigidbody>().AddForce(new Vector3(0f, 50f, 0f), ForceMode.Acceleration);
                }
            }
            else
            {
                flame.SetActive(false);
            }

            if (Time.time > flighttoggle)
            {
                if (primButton)
                {
                    if (!isflight)
                    {
                        isflight = true;
                    }
                    else
                    {
                        isflight = false;
                    }
                    flighttoggle = Time.time + ftogglecooldown;
                }
            }
        }
        /* This attribute tells Utilla to call this method when a modded room is joined */
        [ModdedGamemodeJoin]
        public void OnJoin(string gamemode)
        {
            /* Activate your mod here */
            /* This code will run regardless of if the mod is enabled*/

            inRoom = true;
        }

        /* This attribute tells Utilla to call this method when a modded room is left */
        [ModdedGamemodeLeave]
        public void OnLeave(string gamemode)
        {
            /* Deactivate your mod here */
            /* This code will run regardless of if the mod is enabled*/

            inRoom = false;
        }
    }
}
