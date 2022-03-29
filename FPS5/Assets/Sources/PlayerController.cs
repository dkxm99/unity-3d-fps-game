using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Input KeyCode")]
    [SerializeField]
    private KeyCode KeyCodeRun = KeyCode.LeftShift;
    private KeyCode KeyCodeJump = KeyCode.Space;
    private KeyCode KeyCodeReload = KeyCode.R;
    private KeyCode KeyCodeKnife = KeyCode.V;
    private KeyCode KeyCodeGrenade = KeyCode.G;
    private KeyCode KeyCodeCrouch = KeyCode.LeftControl;

    [Header("Audio Clips")]
    [SerializeField]
    private AudioClip audioClipWalk;
    [SerializeField]
    private AudioClip audioClipRun;

    private PlayerMovement playerMovement;
    private RotateToMouse rotateToMouse;
    private PlayerStatus playerStatus;
    //private AnimatorController animatorController;
    private AudioSource audioSource;
    //private WeaponAssultRifle weapon;
    private WeaponSystem weapon;
    public GameObject gameOverMenu;

    public int grenadeAmmo = 3;

    private void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        rotateToMouse = GetComponent<RotateToMouse>();
        playerMovement = GetComponent<PlayerMovement>();
        playerStatus = GetComponent<PlayerStatus>();
        //animatorController = GetComponent<AnimatorController>();
        audioSource = GetComponent<AudioSource>();
        //weapon = GetComponentInChildren<WeaponAssultRifle>();
        //weaponSystem = GetComponentInChildren<WeaponSystem>();
    }

    private void Update()
    {
        if (Time.timeScale != 0f)
        {
            UpdateRotate();
            UpdateMove();
            UpdateJump();
            UpdateWeaponAction();
        }
    }

    public void UpdateRotate()
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
            if (v > 0 && playerMovement.MoveSpeed > 1.5f) isRun = Input.GetKey(KeyCodeRun);
            if (isRun == true && weapon.AnimatorController.AimModeIs == true)
            {
                weapon.AnimatorController.AimModeIs = false;
                weapon.mainCamera.fieldOfView = weapon.defaultFOV;
                weapon.crossHairImage.enabled = !weapon.crossHairImage.enabled;
            }

            playerMovement.MoveSpeed = isRun == true ? playerStatus.RunSpeed : playerStatus.WalkSpeed;
            weapon.AnimatorController.MoveSpeed = isRun == true ? 1 : 0.5f;
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
            weapon.AnimatorController.MoveSpeed = 0;

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
            weapon.StartWeaponAction(0);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            weapon.StopWeaponAction(0);
        }
        if (Input.GetMouseButtonDown(1))
        {
            weapon.StartWeaponAction(1);
        }
        else if (Input.GetMouseButtonUp(1))
        {
            weapon.StopWeaponAction(1);
        }
        if (Input.GetKeyDown(KeyCodeReload))
        {
            weapon.StartReload();
        }
        if (Input.GetKeyDown(KeyCodeKnife))
        {
            weapon.StartKnifeAction(0);
        }
        if (Input.GetKeyDown(KeyCodeGrenade))
        {
            weapon.StartGrenadeAction(0);
        }
        if(Input.GetKey(KeyCodeCrouch))
        {
            transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        }
        else if(Input.GetKeyUp(KeyCodeCrouch))
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    public void TakeDamage(int damage)
    {
        bool isDie = playerStatus.DecreaseHP(damage);
        if (isDie == true)
        {
            Debug.Log("GameOver");
            StartCoroutine("GameOver");
        }
    }

    private IEnumerator GameOver()
    {
        for (int i = 1; i < 5; ++i)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
        yield return new WaitForSeconds(1);
        gameOverMenu.SetActive(true);      
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0;
    }

    public void SwitchingWeapon(WeaponSystem newWeapon)
    {
        weapon = newWeapon;
    }
}