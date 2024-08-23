using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;
using TMPro;

public class RoomMenuHandler : MonoBehaviourPunCallbacks
{
    public Button joinTabButton,createTabButton,joinRoomButton,createRoomButton,roomModeButton,backButton,startForCreate,joinForJoin;
    public GameObject selectModeMenu;

    public TMP_Text NumberOfPlayersText,RoomCodeText,joinMessage;

    public TMP_InputField inputRoomCode;
    public override void OnEnable()
    {
        base.OnEnable();
        joinTabButton.interactable = false;
        createTabButton.interactable = true;
        OnJoinTabButtonClick();
        NumberOfPlayersText.text = "";
        RoomCodeText.text ="";
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) && backButton.IsInteractable())
            {
                if(PhotonNetwork.InRoom)
                {
                    PhotonNetwork.LeaveRoom();
                    backButton.interactable = false;
                    return;
                }
                selectModeMenu.SetActive(true);
                gameObject.SetActive(false);
            }
    }

    public void OnRoomCreateButtonClick()
    {
        int randomRoomName = Random.Range(0, 10000);
        RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = (byte)4};
        PhotonNetwork.CreateRoom(randomRoomName.ToString(), roomOps);
        NetworkManager.NM.roomId = randomRoomName;
        print("room ID = "+randomRoomName);
        createRoomButton.interactable = false;
        joinTabButton.interactable = false;
        backButton.interactable = false;
    }
    
     void SetplayerCount()
    {
        if(PhotonNetwork.PlayerList.Length > 1)
           startForCreate.interactable = true;
        NumberOfPlayersText.text ="Number Of Players In Room- "+PhotonNetwork.CurrentRoom.PlayerCount +"\nwaiting for Other Players To Join The Room";
        NetworkManager.NM.numberOfPlayers = PhotonNetwork.PlayerList.Length;
    }
    public override void OnCreatedRoom()
    {
        RoomCodeText.text = "Room Code is - "+NetworkManager.NM.roomId;
        NumberOfPlayersText.text ="Number Of Players In Room- "+PhotonNetwork.CurrentRoom.PlayerCount +"\nwaiting for Other Players To Join The Room";
        InvokeRepeating("SetplayerCount",0,1);
        NetworkManager.NM.numberOfPlayers = 1;
        backButton.interactable = true;
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        print("failed To Create Room ");
        RoomCodeText.text = "failed To Create Room";
        createRoomButton.interactable = true;
        joinTabButton.interactable = true;
        backButton.interactable = true;
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        print("failedToConnect");
        joinMessage.text = "Failed To Join the Room";
        joinRoomButton.interactable = true;
    }
    public void OnJoinTabButtonClick()
    {
        createTabButton.transform.GetChild(0).gameObject.SetActive(false);
        createTabButton.interactable = true;
        joinTabButton.interactable = false;
        joinTabButton.transform.GetChild(0).gameObject.SetActive(true);
        createRoomButton.gameObject.SetActive(false);
    }

    public void OnCreateTabButtonCLick()
    {
        joinTabButton.transform.GetChild(0).gameObject.SetActive(false);
        joinTabButton.interactable = true;
        createTabButton.interactable = false;
        createTabButton.transform.GetChild(0).gameObject.SetActive(true);
        createRoomButton.gameObject.SetActive(true);
        createRoomButton.interactable = true;
    }

    public void OnStartButtonClick()
    {
        PhotonNetwork.LoadLevel(NetworkManager.NM.gameSceneIndex);
    }
    public override void OnDisconnected(Photon.Realtime.DisconnectCause cause)
    {
        if(!gameObject.activeSelf)
            return;
        print("connection Lost");
        selectModeMenu.SetActive(true);
        gameObject.SetActive(false);
        roomModeButton.interactable = false;
        selectModeMenu.GetComponent<SelectModeMenu>().InvokeTheConnectionTry();
    }

    public override void OnLeftRoom()
    {
        print("room left");
        if(!gameObject.activeSelf)
            return;
        joinMessage.text = "Enter The \n Room Code";
        gameObject.SetActive(false);
        CancelInvoke("SetplayerCount");
        selectModeMenu.SetActive(true);
    }

    public void OnJoinButtonClicked()
    {
        PhotonNetwork.JoinRoom(inputRoomCode.text);
        joinRoomButton.interactable = false;
    }

    public override void OnJoinedRoom()
    {
        joinMessage.text = "Room Joined Successfully \n Waiting for the Host To Start The Game";
    }

    public void OnInputFieldSelected(TMP_InputField input)
    {
        if(input.text =="")
            joinRoomButton.interactable = false;
        else
            joinRoomButton.interactable = true;
    }
}
