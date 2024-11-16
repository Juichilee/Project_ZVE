using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerControlScript))]
public class PlayerVelocityTracker : MonoBehaviour
{
    public PlayerControlScript player;
    private float HistoricalPositionDurantion = 1f;
    private float HistoricalPositionInterval = 0.1f;

    private Vector3 TotalSum;
    public Vector3 AverageVelocity;

    public Queue<Vector3> HistoricalVelocities;
    private float LastPositionTime;
    private int MaxQueueSize;
 
    private void Awake()
    {
        MaxQueueSize = Mathf.CeilToInt(1f / HistoricalPositionInterval * HistoricalPositionDurantion);
        HistoricalVelocities = new Queue<Vector3>(MaxQueueSize);
        TotalSum = Vector3.zero;
        AverageVelocity = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= LastPositionTime + HistoricalPositionInterval)
        {
            if (HistoricalVelocities.Count == MaxQueueSize)
            {
                Vector3 dequeVal = HistoricalVelocities.Dequeue();   
                TotalSum -= dequeVal;
            }

            Vector3 playerVelocity = player.WorldVelocity;
            playerVelocity.y = 0;
            TotalSum += playerVelocity;
            HistoricalVelocities.Enqueue(playerVelocity);
            LastPositionTime = Time.time;
        }

        AverageVelocity = TotalSum / HistoricalVelocities.Count;
    }
}
