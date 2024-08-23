using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class CharacterManager : MonoBehaviour
{
    public int playerNumber;
    public Color myColor; 
    PhotonView PV;
    CharacterMotor[] MyPlayers;

    public int progress = 0;
    void Awake()
    {
         PV = GetComponent<PhotonView>();
    }
    void Start()
    {   
        SceneManager.sceneUnloaded += DestroyThis;
        DiceManager.dice.diceThrown += CloseAllCollider;
        MyPlayers = new CharacterMotor[4];
        InitaializePlayers();       
    }

    void DestroyThis(Scene s)
    {
        if(s.buildIndex == NetworkManager.NM.gameSceneIndex)
        {
          Destroy(gameObject);
          SceneManager.sceneUnloaded -= DestroyThis;
        }
    }
    void InitaializePlayers()
    {
        GameObject g = Resources.Load("player") as GameObject;
        for(int i = 0;i<4;i++)
        {
            Transform temp = transform.parent.GetChild(i);
            GameObject player = Instantiate(g,temp.position,temp.rotation,transform);
            CharacterMotor temp1 = player.GetComponent<CharacterMotor>();
            MyPlayers[i] = temp1;
            temp1.tag = tag;
            temp1.currentSpot = temp;
            temp1.homeSpot = temp;
        }
        transform.SetParent(null);
    }
    public void CheckForTurn(int c)
    {
        if(c  == playerNumber && PV.IsMine)    
        {
            if(progress == 228)
            {
                GiveupTurn();
                return;
            }
            DiceManager.dice.diceThrowResult += FindMovableCharacters;
            GameManagerLudo.GM.BoardTouched += CallForDiceThrow;
             if(!PhotonNetwork.OfflineMode)
            {
                Player p = PhotonNetwork.CurrentRoom.GetPlayer(PV.ControllerActorNr);
                DiceManager.dice.SetOwnerForDice(p);
            }
            print(tag);
        }
    }
    
    //Subscribed to board Touch
    void CallForDiceThrow()
    {
        GameManagerLudo.GM.BoardTouched -= CallForDiceThrow;
        GuiNumberOnDice.numberGiu.gameObject.SetActive(false);
        if(progress == 0 || progress == 114)
            DiceManager.dice.ThrowDice(0);
        else if(progress == 57 || progress == 114 || progress == 171)
            DiceManager.dice.ThrowDice(0);
        else
            DiceManager.dice.ThrowDice(progress);        
    }

    [PunRPC]//this function is called eveywhere 
    public void FindMovableCharacters(int number)
    {
        
        GuiNumberOnDice.numberGiu.Setup(number);
        GameManagerLudo.GM.numberOnDice = number;
        if(PV.IsMine && !PhotonNetwork.OfflineMode)
            PV.RPC("FindMovableCharacters",RpcTarget.Others,number);
        int movableCharacters = 0;
        int autoMovableCharacterIndex = 0;
        for(int i = 0 ;i< MyPlayers.Length;i++)
        {
            int t = MyPlayers[i].CheckForMovement(number);
            movableCharacters += t;
            if(t == 1)
            {
                autoMovableCharacterIndex = i;
                GameManagerLudo.GM.allPointers[i].MoveOverPlayerHead(MyPlayers[i].transform);
                if(PV.IsMine) 
                    MyPlayers[i].SwitchCollider(true);
            }
        }
        if(movableCharacters > 1)
        {
            Pointer.indicator.DisablePointer();
            GameManagerLudo.GM.PlayerAudio(5,1);
        }
        else if(movableCharacters == 1)
        {
            GameManagerLudo.GM.allPointers[autoMovableCharacterIndex].Disable();
            MyPlayers[autoMovableCharacterIndex].MoveToDesiredSpot(number);
            MyPlayers[autoMovableCharacterIndex].SwitchCollider(false);
        }
        else
            GiveupTurn();
        DiceManager.dice.ChangeState(false);
    } 
    //this function gets called on every players board;
    void GiveupTurn()
    {
        UpdateMyProgress();
        if(progress == 228)
        {
            GameManagerLudo.GM.FireWorks.Play();
            if(!GameManagerLudo.GM.Rank.Contains(playerNumber))
                GameManagerLudo.GM.Rank.Add(playerNumber);
        }
        if(PV.IsMine)
        {
            DiceManager.dice.diceThrowResult -= FindMovableCharacters;
            GameManagerLudo.GM.ChangeChance();
        }
    }
    public void OnClickedPlayer(CharacterMotor c)
    {
        for(int i = 0 ;i<4;i++)
            if(MyPlayers[i] == c)
            {
                PV.RPC("RPC_MoveClickedPlayer",RpcTarget.Others,i);
                break;
            }   
    }
    [PunRPC]
    public void RPC_MoveClickedPlayer(int index)
    {
        MyPlayers[index].MoveToDesiredSpot(GameManagerLudo.GM.numberOnDice);
    }
    public void ExtraChance(bool state)
    {  
        if(state)
        {
            UpdateMyProgress();
            GameManagerLudo.GM.ResetPointers();
             if(progress == 227)
            {
                GameManagerLudo.GM.Rank.Add(playerNumber);
                GiveupTurn();
                return;
            }    
            //GameManagerLudo.GM.ResetTimer();
            if(PV.IsMine)
                GameManagerLudo.GM.BoardTouched += CallForDiceThrow;  
            return;    
        }
        GiveupTurn();
    }

    public void UpdateMyProgress()
    {
        GameManagerLudo.GM.PlayerProgresses[playerNumber].text =(100*progress/228).ToString() + "%";        
    }
    public void CloseAllCollider(bool state)
    {
        foreach(CharacterMotor c in MyPlayers)
            c.SwitchCollider(state);
    }
}
