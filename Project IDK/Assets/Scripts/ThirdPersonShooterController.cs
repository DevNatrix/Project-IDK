using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using StarterAssets;
using UnityEngine.InputSystem;

public class ThirdPersonShooterController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera aimvirtualcam;
    [SerializeField] private float normalSensitivity;
    [SerializeField] private float aimSensitivity;
    [SerializeField] private LayerMask aimColliderLayermask = new LayerMask();
    [SerializeField] private Transform debugTransform;

    private StarterAssetsInputs starterassetsInputs;
    private ThirdPersonController thirdpersonController;
    private bool hasSwitchedCameraSide = false;

    private void Awake()
    {
        thirdpersonController = GetComponent<ThirdPersonController>();
        starterassetsInputs = GetComponent<StarterAssetsInputs>();
    }

    private void Update()
    {
        Vector3 mouseWorldPosition = Vector3.zero;

        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderLayermask))
        {
            debugTransform.position = raycastHit.point;
            mouseWorldPosition = raycastHit.point;
        }

        if(starterassetsInputs.jump)
        {
            Debug.Log("Ok");
        }

        if (starterassetsInputs.aim)
        {
            aimvirtualcam.gameObject.SetActive(true);
            thirdpersonController.SetSensitivity(aimSensitivity);
            thirdpersonController.SetRotateOnMove(false);

            Vector3 worldAimTarget = mouseWorldPosition;
            worldAimTarget.y = transform.position.y;
            Vector3 aimDirection = (worldAimTarget - transform.position).normalized;

            transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);

            if(starterassetsInputs.switchcameraside)
            {
                Debug.Log("Hello");
                aimvirtualcam.GetCinemachineComponent<Cinemachine3rdPersonFollow>().CameraSide = -0.8f;
                hasSwitchedCameraSide = true;
            }

            if (starterassetsInputs.switchcameraside && hasSwitchedCameraSide)
            {
                aimvirtualcam.GetCinemachineComponent<Cinemachine3rdPersonFollow>().CameraSide = 0.8f;
                hasSwitchedCameraSide = false;
            }
        }
        else
        {
            aimvirtualcam.gameObject.SetActive(false);
            thirdpersonController.SetSensitivity(normalSensitivity);
            thirdpersonController.SetRotateOnMove(true);
        }
    }
}
