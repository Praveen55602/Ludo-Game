using UnityEngine;
using UnityEngine.SceneManagement;

public class deathEffect : MonoBehaviour
{
    public static deathEffect DE;
    ParticleSystem e1;
    Material material;

    void Awake()
    {
        if(deathEffect.DE == null)
            deathEffect.DE = this;
        else
            if(deathEffect.DE != this)
                Destroy(this.gameObject);

        DontDestroyOnLoad(this.gameObject); 
    }
    void Start()
    {
        SceneManager.sceneUnloaded += DestroyThis;
        e1 = transform.GetChild(0).GetComponent<ParticleSystem>();
        material = e1.GetComponent<Renderer>().material;
    }

    void DestroyThis(Scene s)
    {
        if(s.buildIndex == NetworkManager.NM.gameSceneIndex)
        {
          Destroy(gameObject);
          SceneManager.sceneUnloaded -= DestroyThis;
        }
    }
    public void StartEffect(Color col,Vector3 position)
    {
        transform.position = position;
        material.SetColor("_EmissionColor",col*3f);
        e1.Play();   
    }
}
