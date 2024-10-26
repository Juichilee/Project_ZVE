using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesRemaining : MonoBehaviour
{
    public int enemiesRemaining = 0;

    public int obtainEnemiesRemaining()
    {
        return enemiesRemaining;
    }

    public void oneEnemyCreated()
    {
        enemiesRemaining = enemiesRemaining + 1;
    }
    
    public void oneEnemyDefeated()
    {
        enemiesRemaining = enemiesRemaining - 1;
        if (enemiesRemaining < 0)
        {
            Debug.Log("Enemies Remaining has become negative");
        }
    }
}
