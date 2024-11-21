using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalDoor : MonoBehaviour
{
    private Animator doorAnimator;
    private PlayerStatus playerStatus;
    public int maxMonsterLevel = 5;
    // Start is called before the first frame update
    void Start()
    {
        doorAnimator = this.GetComponent<Animator>();
        playerStatus = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<PlayerStatus>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (playerStatus.monsterPoints <= maxMonsterLevel)
        {
            doorAnimator.SetTrigger("AllEnemiesDefeated");
        }
    }

}
