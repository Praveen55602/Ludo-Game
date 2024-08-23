#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class pieceConnection : MonoBehaviour
{
    public Transform[] allPositions;
    public bool ResetConnection;
    void Start()
    {
        allPositions = GetComponentsInChildren<Transform>();
    }
    void Update()
    {
        if (ResetConnection)
        {
            ResetConnection = false;
            allPositions = GetComponentsInChildren<Transform>();
            for (int x = 1; x < allPositions.Length-1; x++)
            {
                if (allPositions[x].GetComponent<connectionClass>() != null)
                    DestroyImmediate(allPositions[x].GetComponent<connectionClass>());
                allPositions[x].gameObject.AddComponent<connectionClass>();
                connectionClass c = allPositions[x].gameObject.GetComponent<connectionClass>();
                c.connections.Add(allPositions[x + 1]);
            }
        }
    }
}
#endif