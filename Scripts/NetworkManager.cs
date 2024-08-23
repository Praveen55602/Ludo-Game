using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager NM;
    public int numberOfPlayers = 2;
    public int gameSceneIndex=1;
    public int roomId;
    public bool offline = true;

    void Awake()
    {
        if(NetworkManager.NM == null)
            NetworkManager.NM = this;
        else
            if(NetworkManager.NM != this)
                Destroy(this.gameObject);

        DontDestroyOnLoad(this.gameObject);
        offline = true;        
    }
 
}
