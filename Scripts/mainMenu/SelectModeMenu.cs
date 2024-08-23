using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class SelectModeMenu : MonoBehaviourPunCallbacks
{

   public GameObject OfflineMenu,RoomMenu,currentWindow,exitWindow;
   public Button backButton;
   void Awake()
   {
       currentWindow = gameObject;
       transform.GetChild(0).GetComponent<Button>().interactable = false;
       PhotonNetwork.OfflineMode = false;
        InvokeTheConnectionTry();
   }
   public override void OnEnable()
   {
       base.OnEnable();
       currentWindow = gameObject;
       backButton.interactable = true;
       InvokeTheConnectionTry();
   } 
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) && backButton.IsInteractable())
            OnBackButton();
    }
   void TryToConnectToServer()
   {  
       print("trying to Connect to servers");
       PhotonNetwork.ConnectUsingSettings();
       //PhotonNetwork.ConnectToRegion("in");
   }
   public override void OnConnectedToMaster()
   {
       CancelInvoke("TryToConnectToServer");
       print("connected to master");
       PhotonNetwork.AutomaticallySyncScene = true;
       transform.GetChild(0).GetComponent<Button>().interactable = true;
   }
   public void OnOfflineModeSelect()
   {
       gameObject.SetActive(false);
       OfflineMenu.SetActive(true);
       currentWindow = OfflineMenu;
   } 

   public void OnBackButton()
   {
       if(currentWindow == gameObject)
       {
            exitWindow.SetActive(true);
            Time.timeScale = 0;
            return;
       }
       else if(currentWindow == RoomMenu)
       {
            if(PhotonNetwork.InRoom)
                {
                    PhotonNetwork.LeaveRoom();
                    backButton.interactable = false;
                    return;
                }
       }
       currentWindow.SetActive(false);
       gameObject.SetActive(true);
       currentWindow = gameObject;       
   }

    public void OnRoomSelected()
    {
        RoomMenu.SetActive(true);
        currentWindow = RoomMenu;
        gameObject.SetActive(false);
    }

    public void InvokeTheConnectionTry()
    {
        if(!IsInvoking("TryToConnectToServer"))
            InvokeRepeating("TryToConnectToServer",0,5);
    }

    public void OnYesClick()
    {
        Application.Quit();
    }

    public void NoClick()
    {
        Time.timeScale = 1;
        exitWindow.SetActive(false);
        gameObject.SetActive(true);
    }
}
