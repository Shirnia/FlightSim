using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {
    [SerializeField]
    new Camera camera;
    [SerializeField]
    Plane plane;
    [SerializeField]
    PlaneHUD planeHUD;

    Vector3 controlInput;
    PlaneCamera planeCamera;
    AIController aiController;

    void Start() {
        planeCamera = GetComponent<PlaneCamera>();
        SetPlane(plane);    //SetPlane if var is set in inspector
    }

    void SetPlane(Plane plane) {
        this.plane = plane;
        aiController = plane.GetComponent<AIController>();

        if (planeHUD != null) {
            planeHUD.SetPlane(plane);
            planeHUD.SetCamera(camera);
        }

        planeCamera.SetPlane(plane);
    }


    public void OnToggleHelp(InputAction.CallbackContext context) {
        ToggleHelp(context.performed);
    }

    public void ToggleHelp(bool context)
    {
        if (plane == null) return;
        if (context) {
            planeHUD.ToggleHelpDialogs();
        }
    }

    public void SetThrottleInput(InputAction.CallbackContext context) {

        SetThrottleInput(context.ReadValue<float>());
    }
    public void SetThrottleInput(float context) 
    {
        if (plane == null) return;
        if (aiController.enabled) return;
        plane.SetThrottleInput(context);
    }


    public void OnRollPitchInput(InputAction.CallbackContext context) {
        OnRollPitchInput(context.ReadValue<Vector2>());
    }
    public void OnRollPitchInput(Vector2 context) {
        if (plane == null) return;
        controlInput = new Vector3(context.y, controlInput.y, -context.x);
    }


    public void OnYawInput(InputAction.CallbackContext context) {        
        OnYawInput(context.ReadValue<float>());
    }
    public void OnYawInput(float context) {
        if (plane == null) return;
        controlInput = new Vector3(controlInput.x, context, controlInput.z);
    }


    public void OnFlapsInput(InputAction.CallbackContext context) {
        OnFlapsInput(context.phase == InputActionPhase.Performed);
    }
    public void OnFlapsInput(bool context) {
        if (plane == null) return;

        if (context) {
            plane.ToggleFlaps();
        }
    }


    public void OnFireMissile(InputAction.CallbackContext context) {
        OnFireMissile(context.phase == InputActionPhase.Performed);
    }
    public void OnFireMissile(bool context) {
        if (plane == null) return;

        if (context) {
            plane.TryFireMissile();
        }
    }

    public void OnFireCannon(InputAction.CallbackContext context) {
        if (plane == null) return;

        if (context.phase == InputActionPhase.Started) {
            OnFireCannon(true);
        } else if (context.phase == InputActionPhase.Canceled) {
            OnFireCannon(false);
        }
    }
    public void OnFireCannon(bool context) {
        if (plane == null) return;

        if (context) {
            plane.SetCannonInput(true);
        } else {
            plane.SetCannonInput(false);
        }
    }

    public void OnToggleAI(InputAction.CallbackContext context) {
        if (plane == null) return;

        if (aiController != null) {
            aiController.enabled = !aiController.enabled;
        }
    }
    public void OnCameraInput(InputAction.CallbackContext context) {
        if (plane == null) return;

        var input = context.ReadValue<Vector2>();
        planeCamera.SetInput(input);
    }

    void Update() {
        if (plane == null) return;
        if (aiController.enabled) return;

        plane.SetControlInput(controlInput);
    }
}
