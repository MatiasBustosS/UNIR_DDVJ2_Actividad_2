using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    public enum Direction
    { Up, Down, Left, Right }

    [SerializeField] private Direction direction;
    [SerializeField] private float distance;
    [SerializeField] private float speed = 2f;
    
    
    public void Open()
    {
        StartCoroutine(MoveTo(transform.position + DirectionToVector() * distance));
    }

    IEnumerator MoveTo(Vector3 target)
    {
        Vector3 start = transform.position;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * speed;
            transform.position = Vector3.Lerp(start, target, t);
            yield return null;
        }

        transform.position = target;
    }

    Vector3 DirectionToVector()
    {
        switch (direction)
        {
            case Direction.Up:    return Vector3.up;
            case Direction.Down:  return Vector3.down;
            case Direction.Left:  return Vector3.left;
            case Direction.Right: return Vector3.right;
        }
        return Vector3.zero;
    }
}
