using UnityEngine; 
using TMPro;

public class progress : MonoBehaviour
{
  CharacterManager characterManager;
  bool updateProgress;
  void Start()
  {
   CharacterManager[] ch = FindObjectsOfType<CharacterManager>();
   foreach(CharacterManager c in ch)
        if(CompareTag(c.tag))
        {
            characterManager = c;
            break;
        }
    GameManagerLudo.GM.AnnounceChanceChange += CheckForChance;       
  }

  void CheckForChance(int n)
  {
     if(CompareTag(GameManagerLudo.GM.SpawnPosition[n].tag))
     {
        updateProgress = true;
        print("updateing progress");
     }
     else 
        updateProgress = false;   
  }
}
