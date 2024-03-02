using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetSpeed : MonoBehaviour
{
    private Vector3 LastPosition;

    public float[] getSpeed()
    {
        float speed = ((transform.position.x - LastPosition.x) / Time.deltaTime);
        LastPosition = transform.position;

        float po_y = transform.position.y;

        return new float[] { (-speed / 8), po_y };
    }
}