using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class Block : MonoBehaviour
{
    public int value;
    public Vector2 Position => transform.position;
    public Node node;

    [SerializeField ]private SpriteRenderer _renderer;
    [SerializeField] private TextMeshPro _text;

    //private void Awake()
    //{
    //    _renderer = GetComponent<SpriteRenderer>();
    //}
    public void Init(BlockType type )
    {
        value = type.value;
        _renderer.color = type.color;
       

        _text.text = type.value.ToString();
    }

    public void SetBlock(Node theNode)
    {
        if (node != null) node.occupiedBlock = null;
        node = theNode;
        node.occupiedBlock = this;
    }

    public void ChangeBlockType(int value   )
    {

    }

}
