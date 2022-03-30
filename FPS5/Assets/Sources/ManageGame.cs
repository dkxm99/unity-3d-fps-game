using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyCountEvent : UnityEngine.Events.UnityEvent<int> { }
public class ManageGame : MonoBehaviour
{
    [SerializeField]
    private GameObject clearMenu;

    [SerializeField]
    public GameObject enemies;

    [HideInInspector]
    public EnemyCountEvent enemyCountEvent = new EnemyCountEvent();

    [SerializeField]
    private PlayerHUD playerHUD;

    private void Awake()
    {
        playerHUD.SetupEnemyCount();
    }
    private void Update()
    {
        DefeatAllEnemy();
        enemyCountEvent.Invoke(enemies.transform.childCount);
    }

    private void DefeatAllEnemy()
    {
        if(enemies.transform.childCount == 0)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0;
            clearMenu.SetActive(true);
        }
    }
}
