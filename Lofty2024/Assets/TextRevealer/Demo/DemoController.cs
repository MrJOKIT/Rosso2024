using UnityEngine;
using UnityEngine.UI;

public class DemoController : MonoBehaviour {

    public GameObject DemosParent;
    public Text Demoname;
    private TextRevealer[] DemoObjects;
    private int currentDemoIndex = 0;
    
	void Start () {
        DemoObjects = DemosParent.GetComponentsInChildren<TextRevealer>(true);
        Demoname.text = DemoObjects[currentDemoIndex].name;
    }

    public void NextDemo()
    {
        DemoObjects[currentDemoIndex].gameObject.SetActive(false);
        Transform sliced = DemosParent.transform.Find(DemoObjects[currentDemoIndex].name + "_sliced");
        if (sliced != null)
        {
            GameObject.DestroyImmediate(sliced.gameObject);
        }

        currentDemoIndex++;
        currentDemoIndex %= DemoObjects.Length;
        Demoname.text = DemoObjects[currentDemoIndex].name;
    }

    public void PreviousDemo()
    {
        DemoObjects[currentDemoIndex].gameObject.SetActive(false);

        Transform sliced = DemosParent.transform.Find(DemoObjects[currentDemoIndex].name + "_sliced");
        if (sliced != null)
        {
            GameObject.DestroyImmediate(sliced.gameObject);
        }

        currentDemoIndex--;
        if (currentDemoIndex < 0)
        {
            currentDemoIndex = DemoObjects.Length - 1;
        }
        Demoname.text = DemoObjects[currentDemoIndex].name;
    }

    public void Reveal()
    {
        Transform sliced = DemosParent.transform.Find(DemoObjects[currentDemoIndex].name + "_sliced");
        if (sliced != null)
        {
            GameObject.DestroyImmediate(sliced.gameObject);
        }

        DemoObjects[currentDemoIndex].gameObject.SetActive(true);
        DemoObjects[currentDemoIndex].Reveal();
    }

    public void Unreveal()
    {
        Transform sliced = DemosParent.transform.Find(DemoObjects[currentDemoIndex].name + "_sliced");
        if (sliced != null)
        {
            GameObject.DestroyImmediate(sliced.gameObject);
        }

        DemoObjects[currentDemoIndex].gameObject.SetActive(true);
        DemoObjects[currentDemoIndex].Unreveal();
    }
}
