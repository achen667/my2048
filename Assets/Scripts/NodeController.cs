using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeController : MonoBehaviour
{
    private Node _node;


    private void Awake()
    {
        _node = GetComponent<Node>();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.UpArrow))
            MoveUp();
    }


    private void MoveUp()
    {
        //line 2

    }
}
