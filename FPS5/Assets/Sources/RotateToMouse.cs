using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToMouse : MonoBehaviour
{
    [SerializeField]
    public float rotCamXAxisSpeed = 5;
    [SerializeField]
    public float rotCamYAxisSpeed = 3;

    private PlayerStatus playerStatus;

    private float limitMinX = -80;
    private float limitMaxX = 80;
    private float eulerAngleX;
    private float eulerAngleY;

    private void Awake()
    {
        playerStatus = GetComponent<PlayerStatus>();
    }
    public void updateRotate(float mouseX, float mouseY)
    {
        eulerAngleY += mouseX * rotCamYAxisSpeed;
        eulerAngleX -= mouseY * rotCamXAxisSpeed;

        eulerAngleX = ClampAngle(eulerAngleX, limitMinX, limitMaxX);

        transform.rotation = Quaternion.Euler(eulerAngleX, eulerAngleY, 0);

    }

    private float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360) angle += 360;
        if (angle > 360) angle -= 360;

        Debug.Log(angle);
        if((angle >= 75 && angle <= 85) || (angle >= -85 && angle <= -75))
        {
            playerStatus.walkSpeed = 15;
            playerStatus.runSpeed = 25;
        }
        else
        {
            playerStatus.walkSpeed = 3;
            playerStatus.runSpeed = 5;
        }
        return Mathf.Clamp(angle, min, max);
    }
}
