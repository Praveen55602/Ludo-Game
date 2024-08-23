using UnityEngine;
using UnityEngine.SceneManagement;

public class unlockEffect : MonoBehaviour
{
    public static unlockEffect UE;
    public ParticleSystem e1;
    public ParticleSystem e2;
    public Material mat;
    public Material mat2;

    void Awake()
    {
         if(unlockEffect.UE == null)
            unlockEffect.UE = this;
        else
            if(unlockEffect.UE != this)
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
        e1 = transform.GetChild(0).GetComponent<ParticleSystem>();
        e2 = transform.GetChild(1).GetComponent<ParticleSystem>();
       
    }

    public void StartTheEffect(Color c)
    {
        e1.transform.localPosition = Vector3.zero + Vector3.up*1.5f;
        mat2.SetColor("_EmissionColor",c*12);
        mat.SetColor("_EmissionColor",c*2.5f);
        e1.Play();
        e2.Play();
    }
    
}
