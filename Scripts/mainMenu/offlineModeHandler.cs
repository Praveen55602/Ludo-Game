using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class offlineModeHandler : MonoBehaviourPunCallbacks
{   
    public List<GameObject> players;
    public GameObject modeSelectMenu;
    public Button add,sub,start;
    bool flag;

    void Start()
    {
       players[0].transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().material.SetColor("Color_94B9E947",Color.blue);
       players[1].transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().material.SetColor("Color_94B9E947",Color.yellow);
    }
    public override void OnEnable()
   {
       base.OnEnable();
       NetworkManager.NM.numberOfPlayers = 2;
        if(players.Count > 2)
        {
            if(players.Count == 3)
            {
                Destroy(players[2]);
                players.Remove(players[2]);
            }
            else if(players.Count == 4)
            {
                Destroy(players[2]);
                Destroy(players[3]);
                players.Remove(players[3]);
                players.Remove(players[2]);
            }
        }
        Arrange(false);
        sub.interactable = false;
        add.interactable = true;   
   }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) && modeSelectMenu.GetComponent<SelectModeMenu>().backButton.IsInteractable())
        {
            gameObject.SetActive(false);
            modeSelectMenu.SetActive(true);
        }
    }

    public void OnAddClicked()
    {
        float offset = 30f;
        float space = 60 / players.Count;
        offset -= space / players.Count;
        Vector3 pos = transform.position;
        pos.z = players[0].transform.position.z;
        pos.y = players[0].transform.position.y;
        for(int i = 0;i<=players.Count;i++)
            offset -= space;
        Vector3 p =  pos + transform.right * offset;   
        GameObject g = Instantiate(players[0],p,players[0].transform.rotation,transform);
        Material  playerMat =  g.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().material;
        playerMat.SetFloat("Vector1_828C4EF1", 1);
         DOVirtual.Float(1, 0, 1.5f, pe =>{ 
            playerMat.SetFloat("Vector1_828C4EF1", pe);});
        players.Add(g); 
        if(players.Count == 3)
            g.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().material.SetColor("Color_94B9E947",Color.red);
        else
            g.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().material.SetColor("Color_94B9E947",Color.green);    
        Arrange(true);
        NetworkManager.NM.numberOfPlayers++;
        if(players.Count == 4)
            add.interactable = false;
        if(players.Count > 2)
            sub.interactable = true;
    }
    public void OnSubtractClick()
    {
        Destroy(players[players.Count-1]);
        players.RemoveAt(players.Count -1);
        Arrange(false); 
        NetworkManager.NM.numberOfPlayers--;
        if(players.Count == 2)
            sub.interactable = false;
        if(players.Count <= 3)
            add.interactable = true;
    }
    void Arrange(bool b)
    {
         float offset = 30f;
                float space = 60 / players.Count;
                offset -= space / players.Count;
                Vector3 pos = transform.position;
                for(int i = 0 ;i<players.Count;i++)
                {
                    pos.z = players[i].transform.position.z;
                    pos.y = players[i].transform.position.y;
                    if(i == players.Count - 1 && b)
                       players[i].transform.position =  pos + transform.right * offset;
                    else   
                        players[i].transform.DOMove(pos + transform.right * offset, 1.2f);
                    offset -= space;
                }
    }

    public void OnStartOffline()
    {
        if(modeSelectMenu.GetComponent<SelectModeMenu>().IsInvoking())
        {
            modeSelectMenu.GetComponent<SelectModeMenu>().CancelInvoke();
            modeSelectMenu.GetComponent<SelectModeMenu>().backButton.interactable = false;
            start.interactable = false;
            flag = true;
            if(PhotonNetwork.IsConnected)
            {
                PhotonNetwork.Disconnect();
                return;
            }
            OnDisconnected(new Photon.Realtime.DisconnectCause());
            print("justStartedOfflineMode");
            return;
        }
        start.interactable = false;
        modeSelectMenu.GetComponent<SelectModeMenu>().backButton.interactable = false;
        PhotonNetwork.AutomaticallySyncScene = false;
        flag = true;
        PhotonNetwork.Disconnect();
    }
    public override void OnDisconnected(Photon.Realtime.DisconnectCause cause)
    {
        print("disconnected");
        if(!flag)
            return;
        PhotonNetwork.OfflineMode = true;    
        SceneManager.LoadScene(NetworkManager.NM.gameSceneIndex);
        flag = false;
    }
}
