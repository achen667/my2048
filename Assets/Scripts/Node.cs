using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public Block occupiedBlock;


    public Vector2 Position => transform.position;

    //private Vector2 _position;
    //private int _value;

    //private void Awake()
    //{
    //    _position = transform.position;
    //}

    //public Vector2 Postition { get { return _position; } set { _position = value; } }
    //public int Value { get { return _value; } set { _value = value; } }

    public void DestroyBlockOnNode()
    {
        Destroy(occupiedBlock.gameObject);
    }
}
