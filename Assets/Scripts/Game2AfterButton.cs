using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game2AfterButton : MonoBehaviour
{
    public DiaLogmanager DM;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            DM.Game2AfterButton();
            transform.position += new Vector3(20, 20, 0);
        }
    }
}
