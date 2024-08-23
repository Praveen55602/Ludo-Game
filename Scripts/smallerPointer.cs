using UnityEngine;
using DG.Tweening;
public class smallerPointer : MonoBehaviour
{
  Material material;

    void Start()
  {
      material = GetComponent<MeshRenderer>().material;
      GameManagerLudo.GM.AnnounceChanceChange += SetPosition;
      gameObject.SetActive(false);  
  }
    public void MoveOverPlayerHead(Transform t)
   {
       if(gameObject.activeSelf)
            return;
       transform.position = Pointer.indicator.transform.position;
       gameObject.SetActive(true);
       transform.SetParent(t);
       transform.rotation = Quaternion.identity;
       transform.DOMove(t.position + Vector3.up,0.4f).OnComplete(()=>Bounce());
   }
    void Bounce()
   {
       transform.DOPunchPosition(Vector3.up,0.5f,1,4).OnComplete(()=> Bounce());
   }
    void SetPosition(int c)
   {
       material.SetColor("_EmissionColor",GameManagerLudo.GM.AllColors[c]*2.2f);
       if(!gameObject.activeSelf)
       {
            transform.position = GameManagerLudo.GM.SpawnPosition[c].position+Vector3.up*2;
            return;
       }
       transform.DOKill(false);
       transform.SetParent(null);
       transform.DOMove(GameManagerLudo.GM.SpawnPosition[c].position+Vector3.up*2,0.5f).OnComplete(()=>{gameObject.SetActive(false);Pointer.indicator.EnablePointer();});
   }
    public void Disable()
    {
       transform.DOKill(false);
       transform.SetParent(null);
       gameObject.SetActive(false);
    }

    public void CombineOnReturn()
    {
        if(!gameObject.activeSelf)
            return;
        transform.DOKill(false);
        transform.SetParent(null);
        transform.DOMove(Pointer.indicator.transform.position,0.5f).OnComplete(() =>{gameObject.SetActive(false);Pointer.indicator.EnablePointer();} );
    }
}
