using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DNA : MonoBehaviour
{
    static int DNAPoints = 0;
    int startPoints = 0;
    public TMP_Text dnaText;

    // Start is called before the first frame update
    void Start()
    {
        startPoints = DNAPoints;
    }

    // Update is called once per frame
    void Update()
    {
        dnaText.text = "" + DNAPoints;
    }

    public static void Addpoints(int points)
    {
        DNAPoints += points;
    }

    public static int GetPoints()
    {
        return DNAPoints;
    }

    public static void SetPoints(int points)
    {
        DNAPoints = points;
    }

    public void ResetPoints()
    {
        DNAPoints = startPoints;
    }
}
