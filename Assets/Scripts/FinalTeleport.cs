using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalTeleport : MonoBehaviour
{
    private MeshRenderer meshRenderer;
    private PlayerStatus playerStatus;
    public int maxMonsterLevel = 5;
    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = this.GetComponent<MeshRenderer>();
        playerStatus = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<PlayerStatus>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (playerStatus.monsterPoints <= maxMonsterLevel)
        {
            meshRenderer.enabled = false;
        }
    }

}
