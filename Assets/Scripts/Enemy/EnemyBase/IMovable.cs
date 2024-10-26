using UnityEngine;

public interface IMovable
{
    bool GoTo(Vector3 position, float speed = 0f);
    bool ReachedTarget();
    void Stop();
}