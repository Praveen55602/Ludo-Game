using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManagerLudo : MonoBehaviour
{
     #region publicVariables

     public Button Butt;
    public static GameManagerLudo GM;
    public Transform[] SpawnPosition;
    public List<int> Rank;
    public ParticleSystem FireWorks;
    public int playerFinishedGame = 0;
    public event Action<int> AnnounceChanceChange;
    public int numberOnDice;
    public event Action BoardTouched;
    public Color[] AllColors;

    public TMP_Text[] PlayerProgresses;
    public List<smallerPointer> allPointers;

    public float ChanceTime = 10;
    public float currentTime = 0;

    public Image[] AllTimerBars;

    public bool StartTimer;

    public FinishMenu Fin;

    #endregion

    #region privateVariables
    PhotonView PV;
    public int chance = -1;

    [SerializeField]
    AudioClip[] AllSounds;

    public AudioSource AS;
    #endregion
    
    void Awake()
    {
        if(GameManagerLudo.GM == null)
            GameManagerLudo.GM = this;
        else
            if(GameManagerLudo.GM != this)
                Destroy(this.gameObject);

        DontDestroyOnLoad(this.gameObject);
    }

    void DestroyThis(Scene s)
    {
        if(s.buildIndex == NetworkManager.NM.gameSceneIndex)
        {
          Destroy(gameObject);
          SceneManager.sceneUnloaded -= DestroyThis;
        }
    }
    void Start()
    {
        SceneManager.sceneUnloaded += DestroyThis;
        Rank = new List<int>();
        AS = GetComponent<AudioSource>();
        PV = GetComponent<PhotonView>();
        InitializeCharacterHolders();
        if(PhotonNetwork.OfflineMode)
        {
            ChangeChance();
            return;
        }
        if(PhotonNetwork.IsMasterClient)
        {
            DiceManager.dice.SetOwnerForDice(PhotonNetwork.CurrentRoom.GetPlayer(PV.ControllerActorNr));
            ChangeChance();
        }   
    }

    void Update()
    {
        if(StartTimer)
        {
            AllTimerBars[chance].fillAmount = currentTime/ChanceTime;
            currentTime-= Time.deltaTime;            
            if(currentTime<=0)
            {
                print("ChanceOver");
                StartTimer = false;
                AllTimerBars[chance].gameObject.SetActive(false);
                currentTime = ChanceTime;
            }  
        }
    }

    [PunRPC]
    void InitializeCharacterHolders()
    {
        GameObject g =  Resources.Load("playerHolder",typeof(GameObject)) as GameObject;
        int t = 0;
        for(int i=0 ;i<NetworkManager.NM.numberOfPlayers; i++)
        {
            t = i;
            GameObject holder = new GameObject();
            if(NetworkManager.NM.numberOfPlayers == 2 && i==1)           
                   t=2;
        
            holder = Instantiate(g,SpawnPosition[t].position,SpawnPosition[t].rotation,SpawnPosition[t]);
            holder.GetComponent<CharacterManager>().myColor = AllColors[t];
            holder.tag = SpawnPosition[t].tag;
            holder.name = SpawnPosition[t].tag +" holder";
            holder.GetComponent<CharacterManager>().playerNumber = t;
            AnnounceChanceChange += holder.GetComponent<CharacterManager>().CheckForTurn;
            if(PhotonNetwork.OfflineMode)
                continue;
            PhotonView pv = holder.GetComponent<PhotonView>();
            Player p = PhotonNetwork.CurrentRoom.GetPlayer(i + 1);
            pv.ViewID = 10 * (i+1);
            pv.SetOwnerInternal(p, p.ActorNumber);
        }       
    }
    [PunRPC]
    public void RPC_ChangeChance(int c)
    {
        if(Rank.Count == NetworkManager.NM.numberOfPlayers-1)
        {
            Butt.interactable =false;
            Invoke("GameFinished",1);
            return;
        }
        chance = c;
        DiceManager.dice.ChangeColor(c);
        AnnounceChanceChange.Invoke(c);
    }

    public void ResetTimer()
    {
        StartTimer = true;
        currentTime = ChanceTime;
        AllTimerBars[chance].gameObject.SetActive(true);
    } 
    public void ResetPointers()
    {
      foreach(smallerPointer s in allPointers)
          s.CombineOnReturn();
    }
    public void ClearExtraIndicator()
    {
        foreach(smallerPointer g in allPointers)
        {
            g.transform.SetParent(null);
            g.gameObject.SetActive(false);
        }
    }
    public void ChangeChance()
    {   
        chance++;
        if(NetworkManager.NM.numberOfPlayers == 2 && chance == 1)
            chance = 2;
        else if(chance > NetworkManager.NM.numberOfPlayers-1)
            chance = 0;

        if(PhotonNetwork.OfflineMode)
            RPC_ChangeChance(chance);
        else        
            PV.RPC("RPC_ChangeChance",RpcTarget.All,chance);
    }
    void OnMouseDown()
    {
        if(BoardTouched!=null)
            BoardTouched.Invoke();
    }

    void GameFinished()
    {
        print("gameFinished");
        Fin.gameObject.SetActive(true);
        Fin.SetUpPosition();
    }

    public void PlayerAudio(int n,float volume)
    {
        AS.clip = AllSounds[n];
        AS.Play();
    }
}
