using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimateHandOnInput : NetworkBehaviour
{
    public InputActionProperty pinchAnimationAction;
    public InputActionProperty fistAnimationAction;
    public Animator handAnimator;

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;

        float triggerValue = pinchAnimationAction.action.ReadValue<float>();
        handAnimator.SetFloat("Trigger", triggerValue);

        float gripValue = fistAnimationAction.action.ReadValue<float>();
        handAnimator.SetFloat("Grip", gripValue);
    }
}
