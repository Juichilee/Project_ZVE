using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemiesRemaining : MonoBehaviour
{
    public int enemiesRemaining = 2;
    private Scene level;
    private string levelTitle;

    // Start is called before the first frame update
    void Start()
    {
        level = SceneManager.GetActiveScene();
        levelTitle = level.name;

        if (levelTitle == "Level1Scene")
        {
            enemiesRemaining = 2;
        }
        if (levelTitle == "Level2Scene")
        {
            enemiesRemaining = 3;
        }
        if (levelTitle == "Level3Scene")
        {
            enemiesRemaining = 4;
        }
    }

    // Obtain how many remaining enemies to defeat before the exit door will open
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
