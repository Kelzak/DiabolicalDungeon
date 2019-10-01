using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorials : MonoBehaviour
{
    public Text Tutorial1;
    private bool isInTutorialRange = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void setTutorial1Text()
    {
      if (isInTutorialRange == false)
      {
        Tutorial1.gameObject.SetActive(false);
      }

      if(isInTutorialRange == true)
      {
        Tutorial1.gameObject.SetActive(true);
      }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == ("Player"))
        {
            //Tutorial1.gameObject.SetActive(true);
            //Destroy(Tutorial1, 5f);
            isInTutorialRange = true;
            Invoke("setTutorial1Text", 1);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == ("Player"))
        {
            //Tutorial1.gameObject.SetActive(false);
            isInTutorialRange = false
            Invoke("setTutorial1Text", 1);
            //Destroy(Tutorial1, 5f);
        }
    }

}
