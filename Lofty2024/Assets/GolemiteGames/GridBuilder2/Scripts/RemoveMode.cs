using UnityEngine;
using UnityEngine.UI;

/*************This class should be placed on a button to enable the object remover class**************/
public class RemoveMode : MonoBehaviour
{
    //Cached
    ObjectRemover objectRemover;
    GridSelector gridSelector;
    ObjectSelector objectSelector;

    //Private
    Button btn;
    Text removeText;

    private void Awake()
    {
        objectRemover = FindObjectOfType<ObjectRemover>();
        gridSelector = FindObjectOfType<GridSelector>();
        objectSelector = FindObjectOfType<ObjectSelector>();
        btn = GetComponent<Button>();
        removeText = transform.GetChild(0).GetComponent<Text>();
    }

    private void Start()
    {
        //Entering object removal mode
        btn.onClick.AddListener(() => {
            if (gridSelector)
            {
                gridSelector.DeselectPreview();
            }
            if(objectSelector)
            {
                objectSelector.DeselectObject();
            }


            EnableRemoveMode();
            ChangeText();
        });
    }
    private void Update()
    {
        //For exiting object removal mode
        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
        {
            if(objectRemover) 
            { 
                objectRemover.CancelRemove();
            }
            DisableRemoveMode();
            ChangeText();
        }
        
    }
    public void DisableRemoveMode()
    {
        if(objectRemover)
        objectRemover.enabled = false;
    }
    public void EnableRemoveMode()
    {
        if(objectRemover)
        objectRemover.enabled = true;
    }
    public void ChangeText()
    {
        if(objectRemover)
        {
            if (objectRemover.enabled)
            {
                if(removeText)
                removeText.text = "Remove Mode Active";
            }
            else
            {
                if(removeText)
                removeText.text = "Remove Mode";
            }
        }
    }
}
