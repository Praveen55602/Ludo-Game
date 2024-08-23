using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class Pointer : MonoBehaviour
{
    public static Pointer indicator;

    AudioSource AS;
    Material mat;

      void Awake()
    {
        if(Pointer.indicator == null)
            Pointer.indicator = this;
        else
            if(Pointer.indicator != this)
                Destroy(this.gameObject);

        DontDestroyOnLoad(this.gameObject);   
    }
   void Start()
   {
       SceneManager.sceneUnloaded += DestroyThis;
       AS = GetComponent<AudioSource>();
       mat =  GetComponent<MeshRenderer>().material;
       GameManagerLudo.GM.AnnounceChanceChange += SetPosition;    
   }

      void DestroyThis(Scene s)
    {
        if(s.buildIndex == NetworkManager.NM.gameSceneIndex)
        {
          Destroy(gameObject);
          SceneManager.sceneUnloaded -= DestroyThis;
        }
    } 

    void SetPosition(int c)
   {
       mat.SetColor("_EmissionColor",GameManagerLudo.GM.AllColors[c]*2.2f);  
       if(!gameObject.activeSelf)
       {
           if(!GameManagerLudo.GM.AS.isPlaying)
            GameManagerLudo.GM.PlayerAudio(5,1);
            transform.position = GameManagerLudo.GM.SpawnPosition[c].position+Vector3.up*2;
            return;
       } 
       transform.DOKill(false);
       if(!GameManagerLudo.GM.AS.isPlaying)
            GameManagerLudo.GM.PlayerAudio(6,1);
       transform.DOMove(GameManagerLudo.GM.SpawnPosition[c].position+Vector3.up*2,0.5f).OnComplete(()=>Bounce());
   }
    public void EnablePointer()
    {
        if(gameObject.activeSelf)
            return;
        gameObject.SetActive(true);
        Bounce();
    }
   void Bounce()
   {
       transform.DOPunchPosition(transform.up,0.5f,1,4).OnComplete(()=> Bounce());
   }
    public  void DisablePointer()
    {
        transform.DOKill(false);
        gameObject.SetActive(false);
        transform.position = GameManagerLudo.GM.SpawnPosition[GameManagerLudo.GM.chance].position+Vector3.up*2;
    }
 
}
