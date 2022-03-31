using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private KeyCode KeyCodeRun = KeyCode.LeftShift;
    private KeyCode KeyCodeJump = KeyCode.Space;
    private KeyCode KeyCodeReload = KeyCode.R;
    private KeyCode KeyCodeKnife = KeyCode.V;
    private KeyCode KeyCodeGrenade = KeyCode.G;
    private KeyCode KeyCodeCrouch = KeyCode.LeftControl;
    private KeyCode KeyCodeLeanLeft = KeyCode.Q;
    private KeyCode KeyCodeLeanRight = KeyCode.E;


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

    private float speed = 0.1f;
    public int grenadeAmmo = 3;

    [Header("Leaning Objects")]
    [SerializeField]
    private Transform playerCamera;
    [SerializeField]
    private Transform LeanLeft;
    [SerializeField]
    private Transform LeanRight;
    [SerializeField]
    private Transform idle;

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
                rotateToMouse.rotCamXAxisSpeed = 4;
                rotateToMouse.rotCamYAxisSpeed = 2;
                weapon.AnimatorController.AimModeIs = false;
                weapon.mainCamera.fieldOfView = weapon.defaultFOV;
                weapon.crossHairImage.enabled = !weapon.crossHairImage.enabled;
            }

            if ((h == 0) && (rotateToMouse.eulerAngleX >= 75 && rotateToMouse.eulerAngleX <= 85) || 
                (rotateToMouse.eulerAngleX >= -85 && rotateToMouse.eulerAngleX <= -75))
            {
                playerStatus.walkSpeed = 15;
                playerStatus.runSpeed = 30;
            }
            else if (Input.GetKey(KeyCode.LeftControl))
            {
                playerStatus.walkSpeed = 1;
            }
            else
            {
                playerStatus.walkSpeed = 3;
                playerStatus.runSpeed = 5;
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
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(0.5f, 0.5f, 0.5f), 5f * Time.deltaTime);
            weapon.isCrouch = true;
        }
        else if(Input.GetKeyUp(KeyCodeCrouch))
        {
            transform.localScale = new Vector3(1, 1, 1);
            weapon.isCrouch = false;
        }
        if (Input.GetKey(KeyCodeLeanLeft))
        {
            playerCamera.position = Vector3.Lerp(playerCamera.position, LeanLeft.position, speed);
            playerCamera.rotation = Quaternion.Lerp(playerCamera.rotation, LeanLeft.rotation, speed);          
        }
        else if (Input.GetKey(KeyCodeLeanRight))
        {
            playerCamera.position = Vector3.Lerp(playerCamera.position, LeanRight.position, speed);
            playerCamera.rotation = Quaternion.Lerp(playerCamera.rotation, LeanRight.rotation, speed);                     
        }
        else
        {
            playerCamera.position = Vector3.Lerp(playerCamera.position, idle.position, speed);
            playerCamera.rotation = Quaternion.Lerp(playerCamera.rotation, idle.rotation, speed);
        }
    }

    private void hitByEnemy()
    {
        int a = Random.Range(0, 3);
        if (a == 0)
        {
            rotateToMouse.eulerAngleX += Random.Range(1, 3);
            rotateToMouse.eulerAngleY += Random.Range(1, 3);
        }
        else if (a == 1)
        {
            rotateToMouse.eulerAngleX -= Random.Range(1, 3);
            rotateToMouse.eulerAngleY -= Random.Range(1, 3);
        }
        else if (a == 2)
        {
            rotateToMouse.eulerAngleX += Random.Range(1, 3);
            rotateToMouse.eulerAngleY -= Random.Range(1, 3);
        }
        else if (a == 3)
        {
            rotateToMouse.eulerAngleX -= Random.Range(1, 3);
            rotateToMouse.eulerAngleY += Random.Range(1, 3);
        }
    }

    public void TakeDamage(int damage)
    {
        hitByEnemy();
        bool isDie = playerStatus.DecreaseHP(damage);
        if (isDie == true)
        {
            Debug.Log("GameOver");
            StartCoroutine("GameOver");
        }
    }

    private IEnumerator GameOver()
    {     
        for (int i = 0; i < 3; ++i)
        {
            transform.GetChild(0).GetChild(i).gameObject.SetActive(false);
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