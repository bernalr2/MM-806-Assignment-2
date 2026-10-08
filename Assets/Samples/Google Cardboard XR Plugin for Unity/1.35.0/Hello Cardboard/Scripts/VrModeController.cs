//-----------------------------------------------------------------------
// <copyright file="VrModeController.cs" company="Google LLC">
// Copyright 2020 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections;
using Google.XR.Cardboard;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.XR;
using UnityEngine.XR.Management;

using InputSystemTouchPhase = UnityEngine.InputSystem.TouchPhase;

/// <summary>
/// Turns VR mode on and off.
/// </summary>
public class VrModeController : MonoBehaviour
{
    // Field of view value to be used when the scene is not in VR mode. In case
    // XR isn't initialized on startup, this value could be taken from the main
    // camera and stored.
    private const float _defaultFieldOfView = 60.0f;

    // Main camera from the scene.
    private Camera _mainCamera;

    private int fingerID;
    
    // Player Movement
    private Vector2 moveTouchStartPosition;
    private Vector2 moveInput;
    
    [SerializeField] private CharacterController characterController;
    [SerializeField] private float moveSpeed;

    /// <summary>
    /// Gets a value indicating whether the screen has been touched this frame.
    /// </summary>
    private bool _isScreenTouched
    {
        get
        {
            TouchControl touch = GetFirstTouchIfExists();
            return touch != null && touch.phase.ReadValue() == InputSystemTouchPhase.Began;
        }
    }

    /// <summary>
    /// Gets a value indicating whether the VR mode is enabled.
    /// </summary>
    private bool _isVrModeEnabled
    {
        get
        {
            return XRGeneralSettings.Instance.Manager.isInitializationComplete;
        }
    }

    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    public void Start()
    {
        // Saves the main camera from the scene.
        _mainCamera = Camera.main;

        // Configures the app to not shut down the screen and sets the brightness to maximum.
        // Brightness control is expected to work only in iOS, see:
        // https://docs.unity3d.com/ScriptReference/Screen-brightness.html.
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Screen.brightness = 1.0f;

        // Checks if the device parameters are stored and scans them if not.
        // This is only required if the XR plugin is initialized on startup,
        // otherwise these API calls can be removed and just be used when the XR
        // plugin is started.
        if (!Api.HasDeviceParams())
        {
            Api.ScanDeviceParams();
        }

        fingerID = -1;
    }

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    public void Update()
    {
        GetTouchInput();
        
        // Left Finger controls movement
        if (fingerID != -1)
        {
            Move();
        }
        
        if (_isVrModeEnabled)
        {
            if (Api.IsCloseButtonPressed)
            {
                ExitVR();
            }

            if (Api.IsGearButtonPressed)
            {
                Api.ScanDeviceParams();
            }

            Api.UpdateScreenParams();
        }
        else
        {
            // TODO(b/171727815): Add a button to switch to VR mode.
            // TODO: Create menus that allow for switching between VR and Normal
            if (_isScreenTouched)
            {
                //EnterVR();
            }
        }
    }
    
    void GetTouchInput()
    {
        // Find a touchscreen
        if (Touchscreen.current == null)
        {
            return;
        }

        // Store a bool for the current status of each finger input
        bool fingerSeen = false;

        // Check through each input found
        foreach (var touch in Touchscreen.current.touches)
        {
            var phase = touch.phase.ReadValue();

            if (phase == UnityEngine.InputSystem.TouchPhase.None)
            {
                continue;
            }

            int newFingerID = touch.touchId.ReadValue();
            Vector2 position = touch.position.ReadValue();
            Vector2 delta = touch.delta.ReadValue();
            
            if (newFingerID == fingerID)
            {
                fingerSeen = true;

                if (phase == UnityEngine.InputSystem.TouchPhase.Moved ||
                    phase == UnityEngine.InputSystem.TouchPhase.Stationary)
                {
                    moveInput = position - moveTouchStartPosition;
                }
            }
            
            if (phase == UnityEngine.InputSystem.TouchPhase.Began)
            {
                if (fingerID == -1)
                {
                    fingerID = newFingerID;
                    fingerSeen = true;
                    moveTouchStartPosition = position;
                    moveInput = Vector2.zero;
                }
            }
            
            if (phase == UnityEngine.InputSystem.TouchPhase.Ended ||
            phase == UnityEngine.InputSystem.TouchPhase.Canceled)
            {
                if (newFingerID == fingerID)
                {
                    fingerID = -1;
                    moveInput = Vector2.zero; 
                    fingerSeen = false;
                }
            }
        }
        
        if (fingerID != -1 && !fingerSeen)
        {
            fingerID = -1;
            moveInput = Vector2.zero;
        }
    }
    
    void Move() {
        // Multiply the normalized direction by the speed
        Vector2 movementDirection = moveInput.normalized * moveSpeed;
        
        // Move relatively to the local transform's direction
        characterController.Move(transform.right * movementDirection.x + transform.forward * movementDirection.y);
    }

    /// <summary>
    /// Checks if the screen has been touched during the current frame.
    /// </summary>
    ///
    /// <returns>
    /// The first touch of the screen during the current frame. If the screen hasn't been touched,
    /// returns null.
    /// </returns>
    private static TouchControl GetFirstTouchIfExists()
    {
        Touchscreen touchScreen = Touchscreen.current;
        if (touchScreen == null)
        {
            return null;
        }

        if (!touchScreen.enabled)
        {
            InputSystem.EnableDevice(touchScreen);
        }

        ReadOnlyArray<TouchControl> touches = touchScreen.touches;
        if (touches.Count == 0)
        {
            return null;
        }

        return touches[0];
    }

    /// <summary>
    /// Enters VR mode.
    /// </summary>
    private void EnterVR()
    {
        StartCoroutine(StartXR());
        if (Api.HasNewDeviceParams())
        {
            Api.ReloadDeviceParams();
        }
    }

    /// <summary>
    /// Exits VR mode.
    /// </summary>
    private void ExitVR()
    {
        StopXR();
    }

    /// <summary>
    /// Initializes and starts the Cardboard XR plugin.
    /// See https://docs.unity3d.com/Packages/com.unity.xr.management@3.2/manual/index.html.
    /// </summary>
    ///
    /// <returns>
    /// Returns result value of <c>InitializeLoader</c> method from the XR General Settings Manager.
    /// </returns>
    private IEnumerator StartXR()
    {
        Debug.Log("Initializing XR...");
        yield return XRGeneralSettings.Instance.Manager.InitializeLoader();

        if (XRGeneralSettings.Instance.Manager.activeLoader == null)
        {
            Debug.LogError("Initializing XR Failed.");
        }
        else
        {
            Debug.Log("XR initialized.");

            Debug.Log("Starting XR...");
            XRGeneralSettings.Instance.Manager.StartSubsystems();
            Debug.Log("XR started.");
        }
    }

    /// <summary>
    /// Stops and deinitializes the Cardboard XR plugin.
    /// See https://docs.unity3d.com/Packages/com.unity.xr.management@3.2/manual/index.html.
    /// </summary>
    private void StopXR()
    {
        Debug.Log("Stopping XR...");
        XRGeneralSettings.Instance.Manager.StopSubsystems();
        Debug.Log("XR stopped.");

        Debug.Log("Deinitializing XR...");
        XRGeneralSettings.Instance.Manager.DeinitializeLoader();
        Debug.Log("XR deinitialized.");

        _mainCamera.ResetAspect();
        _mainCamera.fieldOfView = _defaultFieldOfView;
    }
}
