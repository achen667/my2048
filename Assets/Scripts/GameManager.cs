using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UI;
using System.Linq;
using System;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public AudioSource audioSource;

    [SerializeField] private int _width = 4;
    [SerializeField] private int _height = 4;
    [SerializeField] private Node _nodePrefab;
    [SerializeField] private SpriteRenderer _boardPrefab;
    [SerializeField] private Block _blockPrefab;
    [SerializeField] private List<BlockType> _types;
    [SerializeField] private GameObject _loseText;

    private List<Node> _nodes;
    private GameStates _state;
    private List<Block> _blocks;
    private bool canSpawn  = true;
    private void ChangeGameStates(GameStates state)
    {
        _state = state;

        switch (state)
        {
            case GameStates.GenerateLevel:
                break;
            case GameStates.SpawningBlock:
                _state = GameStates.SpawningBlock;
                break;
            case GameStates.WaitingInput:
                _state = GameStates.WaitingInput;
                break;
            case GameStates.Lose:
                break;
        }
    }
        private BlockType GetBlockTypeByValue(int value)       //【Linq】
    {
       return _types.First(t => t.value == value);
    }
    private void Start()
    {
        _loseText.SetActive(false);
        _nodes = new List<Node>();
        _blocks = new List<Block>();
        _state = new GameStates();
        GenerateGrid();

    }

    //int dirCode;
    private void Update()
    {

        if (_state != GameStates.SpawningBlock)
        {
            int dirCode = GetSlide();
            switch (dirCode)
            {
                case 1:
                    Shift(Vector2.up);
                    SpawnBlocks();
                    break;
                case 2:
                    Shift(Vector2.down);
                    SpawnBlocks();
                    break;
                case 3:
                    Shift(Vector2.right);
                    SpawnBlocks();
                    break;
                case 4:
                    Shift(Vector2.left);
                    SpawnBlocks();
                    break;
                case 0:
                    break;
            }
            
        }
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }

    void GenerateGrid()
    {
        for (int x = 0; x < _width; x++)
            for (int y = 0; y < _height; y++)
            {
                Node node = Instantiate(_nodePrefab, new Vector2(x, y), Quaternion.identity);
                _nodes.Add(node);
            }
        Vector2 center = new Vector2((float)_width / 2 - 0.5f, (float)_height / 2 - 0.5f);
        SpriteRenderer borad = Instantiate(_boardPrefab, center, Quaternion.identity);
        borad.size = new Vector2(_width, _height);

        SpawnBlocks();
    }

    //public void RegistNode(Node newNode)
    //{
    //    nodeList.Add(newNode); 
    //}
    //public void RemoveNode(Node theNode)
    //{
    //    if(nodeList.Contains(theNode))
    //         nodeList.Remove(theNode);
    //}
        
    void SpawnBlocks()
    {
        if (!canSpawn)
            return;
        var freeNode = _nodes.Where(n => n.occupiedBlock == null).OrderBy(b =>UnityEngine.Random.value).ToList();      //【Linq】

        foreach (var node in freeNode.Take(1))
        {
            var block = Instantiate(_blockPrefab, node.Position, Quaternion.identity);
            block.Init(GetBlockTypeByValue(UnityEngine.Random.value > 0.7f ? 4 : 2));
            block.SetBlock(node);
            _blocks.Add(block);
        }
        Debug.Log(freeNode.Count);
        if (_nodes.Count() ==1)
        {
            ChangeGameStates(GameStates.Lose);
            _loseText.SetActive(true);
            Debug.Log("gameover");
            return;
        }
        //Shift(Vector2.left); // 【  debug----------------------------------】
        audioSource.Play();
    }
    void Shift(Vector2 dir)
    {
        bool addOnceFlag;
         canSpawn = false;
         ChangeGameStates(GameStates.SpawningBlock);
         var orderedBlocks = _blocks.OrderBy(b => b.Position.x).ThenBy(b => b.Position.y).ToList();   //【OrderBy】  从小到大  （从左到右 从下到上）
         if (dir == Vector2.right || dir == Vector2.up)
             orderedBlocks.Reverse();

         foreach (var block in orderedBlocks)    //（快慢指针的感觉）： Node 先走，判断下个node是否1.为空 2.没有block，若是，则让block跟着移动
         {
             addOnceFlag = true;
             //Debug.Log(block.Position.ToString());
             var next = block.node;
             do
             {
                 block.SetBlock(next);

                 var possibleNode = GetNodeAtPosition(next.Position + dir);
                 if (possibleNode != null)   //下一个有位置                     
                 {
                    if (addOnceFlag && (possibleNode.occupiedBlock != null && possibleNode.occupiedBlock.value == block.value))
                    {
                        next = possibleNode;   //取下一个
                                               //next.occupiedBlock = null;
                        block.Init(GetBlockTypeByValue(block.value * 2));
                        _blocks.Remove(possibleNode.occupiedBlock);
                        possibleNode.DestroyBlockOnNode();
                        addOnceFlag = false;
                        canSpawn = true;
                    }
                    else if (possibleNode.occupiedBlock == null)
                    {
                        next = possibleNode;
                        canSpawn = true;
                    }
                     
                 }
             } while (next != block.node);    //若下一个还是它自己  说明到头了

            block.transform.DOMove(block.node.Position, 0.14f);
            StartCoroutine(Delay());
            //block.transform.position = block.node.Position;
        }
         //ChangeGameStates(GameStates.WaitingInput);

    }
    Node GetNodeAtPosition(Vector2 pos)
    {
        return _nodes.FirstOrDefault(n => n.Position == pos);    //【.FirstOrDefault】
    }
    IEnumerator Delay()
    {
        ChangeGameStates(GameStates.SpawningBlock);
        yield return new WaitForSeconds(0.15f);
        ChangeGameStates(GameStates.WaitingInput);
    }


    public void Restart()
    {
        //foreach (var block in _blocks)
        //{ 
        //    Destroy(block.gameObject);
        //    _blocks.Remove(block);

        //}
        SceneManager.LoadScene(0);
    }

    private Vector2 startPos;
    private Vector2 endPos;
    private Vector2 dir;
    private Touch touch;
    float dotY;
    float dotX;
    float angleY;
    float angleX;
    private int GetSlide()
    {
        
        if(Input.touchCount > 0 )
        {
            touch = Input.GetTouch(0);   //get the first touch point
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    startPos = touch.position;
                    Debug.Log("BEGIN" + startPos);
                    return 0;
                case TouchPhase.Moved:
                    return 0;

                case TouchPhase.Ended:
                    endPos = touch.position;
                    dir = endPos - startPos;
                    Debug.Log(dir);
                    break;
                default: return 0;
            }
             dotY = Vector2.Dot(Vector2.up, dir.normalized);
             dotX = Vector2.Dot(Vector2.right, dir.normalized);
             angleY = Mathf.Acos(dotY) * Mathf.Rad2Deg;
             angleX = Mathf.Acos(dotX) * Mathf.Rad2Deg;
            if (angleY <= 45)  //up
                return 1;
            if (angleY >= 135)  //down
                return 2;
            if (angleX < 45)  //right
                return 3;
            if (angleX > 135)   // left
                return 4;
        }
        return 0;
    }
}
[Serializable]
public struct BlockType
{
    public int value;
    public Color color;
}

public enum GameStates
{
    GenerateLevel,
    SpawningBlock,
    WaitingInput,
    Lose,
}
