using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
public class GuiNumberOnDice : MonoBehaviour
{
    TMP_Text number;
    public static GuiNumberOnDice numberGiu;
    void Awake()
    {
         if(GuiNumberOnDice.numberGiu == null)
            GuiNumberOnDice.numberGiu = this;
        else
            if(GuiNumberOnDice.numberGiu != this)
                Destroy(this.gameObject);

        DontDestroyOnLoad(this.gameObject);   
    }
    

    void Start()
    {
         SceneManager.sceneUnloaded += DestroyThis;
        number = GetComponent<TMP_Text>();
    }

     void DestroyThis(Scene s)
    {
        if(s.buildIndex == NetworkManager.NM.gameSceneIndex)
        {
          Destroy(gameObject);
          SceneManager.sceneUnloaded -= DestroyThis;
        }
    }
    public void Setup(int n)
    {
        number.text = n.ToString();
        gameObject.SetActive(true);
    }
}
