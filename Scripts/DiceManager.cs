using UnityEngine;
using System;
using Photon.Pun;
using Photon.Realtime;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class DiceManager : MonoBehaviour
{
    public int sixesPerTurn;
    public static DiceManager dice;
    public float RotationSpeed=800;
    public event Action<int> diceThrowResult;
    public event Action<bool> diceThrown;
    PhotonView PV;
    AudioSource AS;
    MeshRenderer mr;

    #region PrivateVariable
    bool Landed;
    Rigidbody rb;
    Vector3[] allSides;
    Vector3 randomRot;
    public bool isMoving;
     
    #endregion;
   
   void Awake()
   {
        if(DiceManager.dice == null)
            DiceManager.dice = this;
        else
            if(DiceManager.dice != this)
                Destroy(this.gameObject);

        DontDestroyOnLoad(this.gameObject); 
   }
   void Start()
   {
        SceneManager.sceneUnloaded += DestroyThis;
       GameManagerLudo.GM.AnnounceChanceChange += ResetSixesPerTurn;
       AS = GetComponent<AudioSource>();
      allSides = new Vector3[6];
      rb = GetComponent<Rigidbody>();
      PV = GetComponent<PhotonView>();
      isMoving = false;
      mr= GetComponentInChildren<MeshRenderer>();
      if(!PhotonNetwork.IsMasterClient)
        Landed = true;
        gameObject.SetActive(false);  
   }
   
    void DestroyThis(Scene s)
    {
        if(s.buildIndex == NetworkManager.NM.gameSceneIndex)
        {
          Destroy(gameObject);
          SceneManager.sceneUnloaded -= DestroyThis;
        }
    }
   void Update()
   {
       if(!Landed)
        transform.Rotate(Vector3.right * RotationSpeed*Time.deltaTime ,Space.World); 
   }
   void FixedUpdate()
   {
       if(rb.velocity == Vector3.zero && Landed)
            FindNumberOnDice();
   }
   void FindNumberOnDice()
   {
       if(!isMoving)
        return;
       GetOrientation();
       isMoving = false;
        float a = Mathf.Infinity;
        int s = 0;
        for (int i = 0; i < 6; i++)
            {
                float temp = Vector3.Angle(allSides[i],Vector3.up);
                if (a > temp)
                {
                    s = i + 1;
                    a = temp;
                }
            } 
        print(s);
        if(s==6)
            sixesPerTurn++;

        FinishThrow(s); 
        
   }

   void FinishThrow(int s)
   {
       GameManagerLudo.GM.numberOnDice = s;
        if(diceThrown!=null)
            diceThrown.Invoke(false);
        if(diceThrowResult!=null)
            diceThrowResult.Invoke(s);
        isMoving = false;   
   }
   public void ThrowDice(int progress)
    {
        rb.isKinematic = false;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        gameObject.SetActive(true);
        if(sixesPerTurn >= 2)
        {
                randomRot.x = 0;
                randomRot.y =0;
                randomRot.z = 90;
        }
        else if(progress == 0)
            {
                randomRot.x = GetRandomRot();
                randomRot.y =0;
                randomRot.z = 0;
            }
        else 
        {
            randomRot.x = GetRandomRot();
            randomRot.y = GetRandomRot();
            randomRot.z = GetRandomRot();
        }
        transform.position = Camera.main.ScreenPointToRay(Input.mousePosition).origin;
        transform.rotation = Quaternion.Euler(randomRot);
        rb.AddForce(new Vector3(0,-6,8),ForceMode.Impulse);
        isMoving = true;
        Landed = false;
        diceThrown.Invoke(true);
        ChangeState(true);
    }
   int GetRandomRot()
    {
        int r = UnityEngine.Random.Range(1,4);
        if(r==1)
            return 90;
        else if(r==2)
            return 180;
        else if(r==3)
            return 270;
        else
            return 0;
    }
   void GetOrientation()
    {
        allSides = new Vector3[6];
            allSides[0] = transform.up;
            allSides[1] = -transform.right;
            allSides[2] = transform.forward;
            allSides[3] = -transform.forward;
            allSides[4] = transform.right;
            allSides[5] = -transform.up;
    }
   void OnCollisionEnter()
   {
       Landed = true;
       AS.Play();
       if(!PhotonNetwork.OfflineMode)
           PV.RPC("RPC_DiceRollSound",RpcTarget.Others); 
   }


    void ResetSixesPerTurn(int n)
    {
        sixesPerTurn = 0;
    }
   public void ChangeColor(int c)
   {
       mr.material.color = GameManagerLudo.GM.AllColors[c];
   } 
    public void SetOwnerForDice(Player player)
    {
        PV.RPC("RPC_CallToBecomeOwner",RpcTarget.All,player);
    }

    public void ChangeState(bool state)
    {
        gameObject.SetActive(state);
        if(!PhotonNetwork.OfflineMode)
            PV.RPC("RPC_ChangeState",RpcTarget.Others,state,transform.position);
    }

    [PunRPC]
    void RPC_ChangeState(bool state,Vector3 position)
    {
         transform.position = position;
         gameObject.SetActive(state);
    }
    [PunRPC]
    void RPC_CallToBecomeOwner(Player player)
    {
        PV.SetOwnerInternal(player,player.ActorNumber);
        GuiNumberOnDice.numberGiu.gameObject.SetActive(false);
        if(PV.IsMine)
            return;
       
        rb.isKinematic = true;
        Landed = true;
        
    }

    [PunRPC]
    void RPC_DiceRollSound()
    {
        AS.Play();
    }
}
