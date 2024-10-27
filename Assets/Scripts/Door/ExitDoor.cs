using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitDoor : MonoBehaviour
{
    private Animator doorAnimator;
    private int enemiesRemainingNumber;
    public GameObject enemiesRemaining;

    // Start is called before the first frame update
    void Start()
    {
        doorAnimator = this.GetComponent<Animator>();
        EnemiesRemaining enemiesRemainingScript = enemiesRemaining.GetComponent<EnemiesRemaining>();
        // Figure out how many remaining enemies to defeat in the level
        enemiesRemainingNumber = enemiesRemainingScript.obtainEnemiesRemaining();
    }

    // Update is called once per frame
    void Update()
    {
        EnemiesRemaining enemiesRemainingScript = enemiesRemaining.GetComponent<EnemiesRemaining>();
        enemiesRemainingNumber = enemiesRemainingScript.obtainEnemiesRemaining();
        if (enemiesRemainingNumber <= 0)
        {
            doorAnimator.SetTrigger("AllEnemiesDefeated");
        }
    }
}
