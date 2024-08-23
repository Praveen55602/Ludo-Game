using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
     public static BoardManager board;
    //public list<CharacterManager> players;
    public Transform[] SpawnPosition;

    public int playerCount = 3;

    void Awake()
    {
        if(BoardManager.board == null)
            BoardManager.board = this;
        else
            if(BoardManager.board != this)
                Destroy(this.gameObject);

        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        if(playerCount > 2)
            Swap(SpawnPosition[1], SpawnPosition[2]);
    }
}
