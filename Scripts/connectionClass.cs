using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class connectionClass:MonoBehaviour
{
    public List<Transform> connections=new List<Transform>();
    public List<CharacterMotor> storedPieces = new List<CharacterMotor>();
    
    public void ArrangeStoredCharcters()
    {
            if (storedPieces.Count == 1)
            {
                storedPieces[0].transform.DOMove(transform.position, 1.2f);
                storedPieces[0].transform.DOScale(new Vector3(0.2f,0.2f,0.2f),1.2f);
                return; 
            }
            else if (storedPieces.Count > 1)
            {
                float offset = 0.3f;
                float space = 0.6f / storedPieces.Count;
                offset -= space / 2;
                foreach (CharacterMotor c in storedPieces)
                {
                    c.transform.DOMove(transform.position + transform.right * offset, 1.2f);
                    c.transform.DOScale(new Vector3(0.18f, 0.18f, 0.18f), 1.2f);
                    offset -= space;
                }
            }
           
    }
}
