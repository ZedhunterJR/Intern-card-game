using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    private bool hasBegunSwitchScene = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && !hasBegunSwitchScene)
        {
            SceneManager.LoadScene(1);
            hasBegunSwitchScene = true;
        }
    }
}
