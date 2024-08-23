using UnityEngine;
using DG.Tweening;
using Photon.Pun;

public class CharacterMotor : MonoBehaviour
{
    #region private Variables
    int stepsMoved = 0;
    CharacterManager CM;
    Collider col;
    Animator animator;
    Material playerMat;
    #endregion
    public Transform targetSpot,currentSpot,homeSpot;
    
    
    void Start()
    {
        
        CM = GetComponentInParent<CharacterManager>();
        col = GetComponent<Collider>();
        animator = GetComponent<Animator>();
        playerMat = transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().material;
        playerMat.SetColor("Color_94B9E947",CM.myColor);        
    }
    public int  CheckForMovement(int number)
    {
        if((number != 6 && stepsMoved == 0) || stepsMoved+number > 57)
            return 0;
        else
            return 1;
    }
    void Unlock()
    {
        GameManagerLudo.GM.PlayerAudio(0,1);
        CM.progress ++;
        targetSpot = currentSpot.GetComponent<connectionClass>().connections[0];
        targetSpot.GetComponent<connectionClass>().storedPieces.Add(this);
        unlockEffect.UE.transform.position = transform.position;
        unlockEffect.UE.StartTheEffect(CM.myColor);
        DOVirtual.Float(0, 1, 0.9f, p =>{}).OnComplete(()=>{unlockEffect.UE.e1.transform.position = transform.position + Vector3.up*0.5f; });
        DOVirtual.Float(0, 1, 0.4f, p =>{ 
            playerMat.SetFloat("Vector1_828C4EF1", p); 
        }).OnComplete(()=> { SetMyTransform(targetSpot);
           CM.ExtraChance(true); 
           currentSpot = targetSpot; 
           currentSpot.GetComponent<connectionClass>().ArrangeStoredCharcters();
           DOVirtual.Float(1, 0, 4f, p =>{ playerMat.SetFloat("Vector1_828C4EF1", p);});
        });
        stepsMoved += 1;
        
    }
    public void SwitchCollider(bool state)
    {
        col.enabled = state;
    }
    void OnMouseDown()
    {
        if(DiceManager.dice.isMoving)
            return;
        CM.CloseAllCollider(false);
        MoveToDesiredSpot(GameManagerLudo.GM.numberOnDice);
        if(!PhotonNetwork.OfflineMode)
            CM.OnClickedPlayer(this); 
    }
    public void MoveToDesiredSpot(int steps)
    {
         if(steps == 0)
        {
            animator.SetBool("walk",false);
            if(currentSpot.CompareTag("safeHouse"))
                GameManagerLudo.GM.PlayerAudio(2,1);
            else if(stepsMoved == 57)
            {
                GameManagerLudo.GM.PlayerAudio(4,1);
                animator.SetTrigger("win");   
            }
            CM.progress += GameManagerLudo.GM.numberOnDice;   
            connectionClass temp = currentSpot.GetComponent<connectionClass>();
            temp.storedPieces.Add(this);
            temp.ArrangeStoredCharcters();
            
            return;
        }
        else if(steps == 6 && stepsMoved == 0)
        {
            Unlock();
            return;
        }
        if(steps == GameManagerLudo.GM.numberOnDice)
        {
            connectionClass c = currentSpot.GetComponent<connectionClass>();
            c.storedPieces.Remove(this);
            c.ArrangeStoredCharcters();
            transform.DOScale(new Vector3(0.20f,0.20f,0.20f),2);
            animator.SetBool("walk",true);
        }
        if(stepsMoved==51)
            targetSpot = currentSpot.GetComponent<connectionClass>().connections[1];
        else
            targetSpot = currentSpot.GetComponent<connectionClass>().connections[0];
        stepsMoved++;

          if(steps == 1)
        {
            if(!CheckForKill())
                {
                    if(GameManagerLudo.GM.numberOnDice == 6)
                        CM.ExtraChance(true);
                    else if(stepsMoved == 57)
                        CM.ExtraChance(true);
                    else    
                        CM.ExtraChance(false);        
                }
        }
        transform.DOMove(targetSpot.position,0.3f,false).SetEase(Ease.Linear).OnComplete((TweenCallback)(()=>{
            currentSpot = targetSpot;
            GameManagerLudo.GM.PlayerAudio(1,(float)1);
            MoveToDesiredSpot(steps - 1);}));
        if(steps == 1 && stepsMoved == 51)
            transform.DORotateQuaternion(targetSpot.GetComponent<connectionClass>().connections[1].rotation,0.28f);
        else
            transform.DORotateQuaternion(targetSpot.rotation,0.28f);
    }
    bool CheckForKill()
    {
        print("checking for kills");
        connectionClass temp = targetSpot.GetComponent<connectionClass>();
        if(temp.CompareTag("safeHouse"))
            return false;
        else if(temp.storedPieces.Count == 1)
        {
            if(CompareTag(temp.storedPieces[0].tag) )
                return false;
            temp.storedPieces[0].GotKilled();
            CM.ExtraChance(true);
            return true;
        }
            return false; 
    }
    public void GotKilled()
    {
        GameManagerLudo.GM.PlayerAudio(3,1);
        currentSpot.GetComponent<connectionClass>().storedPieces.Remove(this);
        deathEffect.DE.StartEffect(CM.myColor,transform.position);
        DOVirtual.Float(0, 1, 1f, p =>{ playerMat.SetFloat("Vector1_828C4EF1", p);}).OnComplete(()=> {
                SetMyTransform(homeSpot);
                DOVirtual.Float(1, 0,3, p =>{playerMat.SetFloat("Vector1_828C4EF1", p);});
                });
        currentSpot = homeSpot;
        CM.progress -= stepsMoved;
        CM.UpdateMyProgress();
        stepsMoved = 0;
    }
    void SetMyTransform(Transform t)
    {
        transform.position = t.position;
        transform.rotation = t.rotation;
    }

}
