using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Input KeyCode")]
    [SerializeField]
    private KeyCode keyCodeRun = KeyCode.LeftShift;
    private KeyCode KeyCodeJump = KeyCode.Space;
    private KeyCode KeyCodeReload = KeyCode.R;

    [Header("Audio Clips")]
    [SerializeField]
    private AudioClip audioClipWalk;
    [SerializeField]
    private AudioClip audioClipRun;

    private PlayerMovement playerMovement;
    private RotateToMouse rotateToMouse;
    private PlayerStatus playerStatus;
    private AnimatorController animatorController;
    private AudioSource audioSource;
    private WeaponAssultRifle weaponAssultRifle;

    private void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        rotateToMouse = GetComponent<RotateToMouse>();
        playerMovement = GetComponent<PlayerMovement>();
        playerStatus = GetComponent<PlayerStatus>();
        animatorController = GetComponent<AnimatorController>();
        audioSource = GetComponent<AudioSource>();
        weaponAssultRifle = GetComponentInChildren<WeaponAssultRifle>();
    }

    private void Update()
    {
        UpdateRotate();
        UpdateMove();
        UpdateJump();
        UpdateWeaponAction();
    }

    private void UpdateRotate()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        rotateToMouse.updateRotate(mouseX, mouseY);
    }

    private void UpdateMove()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        if (h != 0 || v != 0)
        {
            bool isRun = false;
            if (v > 0) isRun = Input.GetKey(keyCodeRun);
            if(isRun == true && animatorController.AimModeIs == true)
            {
                animatorController.AimModeIs = false;
                weaponAssultRifle.mainCamera.fieldOfView = weaponAssultRifle.defaultFOV;       
                weaponAssultRifle.aimImage.enabled = !weaponAssultRifle.aimImage.enabled;
            }

            playerMovement.MoveSpeed = isRun == true ? playerStatus.RunSpeed : playerStatus.WalkSpeed;
            animatorController.MoveSpeed = isRun == true ? 1 : 0.5f;
            audioSource.clip = isRun == true ? audioClipRun : audioClipWalk;

            if (audioSource.isPlaying == false)
            {
                audioSource.loop = true;
                audioSource.Play();
            }
        }
        else
        {
            playerMovement.MoveSpeed = 0;
            animatorController.MoveSpeed = 0;

            if (audioSource.isPlaying == true)
            {
                audioSource.Stop();
            }
        }

        playerMovement.MoveTo(new Vector3(h, 0, v));
    }

    private void UpdateJump()
    {
        if (Input.GetKeyDown(KeyCodeJump))
        {
            playerMovement.Jump();
        }
    }

    private void UpdateWeaponAction()
    {
        if (Input.GetMouseButtonDown(0))
        {
            weaponAssultRifle.StartWeaponAction(0);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            weaponAssultRifle.StopWeaponAction(0);
        }
        if (Input.GetMouseButtonDown(1))
        {
            /*rotateToMouse.rotCamXAxisSpeed = 2;
            rotateToMouse.rotCamYAxisSpeed = 1;*/
            weaponAssultRifle.StartWeaponAction(1);
            
        }
        else if (Input.GetMouseButtonUp(1))
        {  
            weaponAssultRifle.StopWeaponAction(1);
           /* rotateToMouse.rotCamXAxisSpeed = 5;
            rotateToMouse.rotCamYAxisSpeed = 3;*/
        }
        if (Input.GetKeyDown(KeyCodeReload))
        {
            weaponAssultRifle.StartReload();
        }
    }

    public void TakeDamage(int damage)
    {
        bool isDie = playerStatus.DecreaseHP(damage);
        if(isDie == true)
        {
            Debug.Log("GameOver");
        }
    }
}