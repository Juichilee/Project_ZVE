using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitDoor : MonoBehaviour
{
    private Animator doorAnimator;
    private int enemiesExisting;
    public GameObject enemiesRemaining;

    // Start is called before the first frame update
    void Start()
    {
        doorAnimator = this.GetComponent<Animator>();
        EnemiesRemaining enemiesRemainingScript = enemiesRemaining.GetComponent<EnemiesRemaining>();
        // Figure out how many enemies exist in the level
        enemiesExisting = enemiesRemainingScript.obtainEnemiesRemaining();
    }

    // Update is called once per frame
    void Update()
    {
        if (enemiesExisting <= 0)
        {
            doorAnimator.SetTrigger("AllEnemiesDefeated");
        }
    }
}
