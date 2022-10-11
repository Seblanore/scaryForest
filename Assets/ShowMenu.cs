using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowMenu : MonoBehaviour
{
    [SerializeField] GameObject gameMenu;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player")) {
            gameMenu.SetActive(true);
            gameMenu.GetComponent<GameMenuManager>().head = other.transform;
        }
    }
}
