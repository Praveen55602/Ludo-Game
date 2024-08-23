using Photon.Pun;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BackButton : MonoBehaviourPunCallbacks
{
    public GameObject exitWIndow;
    public TMP_Text Message;

    Button Butt;    

    void Start()
    {
        Butt = GetComponent<Button>();
    }
  void Update()
  {
      if(Input.GetKeyDown(KeyCode.Escape)&& Butt.IsInteractable())
      {
         OnBackButtonClick();
      }
  }

    public void OnBackButtonClick()
    {
        if(PhotonNetwork.OfflineMode)
        {
            Message.text = "Exit To Main Menu?";
            Time.timeScale = 0;
        }
        else
            Message.text = "Leave The Game And Go To \nMain Menu?";
           
        exitWIndow.SetActive(true);
    }

    public override void OnLeftRoom()
    {
        if(!gameObject.activeSelf)
            return;    
        SceneManager.LoadScene(0);
    }

    public void OnNoButtonClick()
    {   
        exitWIndow.SetActive(false);
        Time.timeScale = 1;
    }

    public void OnYesButtonClikc()
    {
        foreach(smallerPointer s in  GameManagerLudo.GM.allPointers)
            Destroy(s.gameObject);
        if(PhotonNetwork.OfflineMode)
        {
           NetworkManager.NM.offline = false;
            Time.timeScale = 1;
            SceneManager.LoadScene(0);
        }
        else
         PhotonNetwork.LeaveRoom();
    }
}
