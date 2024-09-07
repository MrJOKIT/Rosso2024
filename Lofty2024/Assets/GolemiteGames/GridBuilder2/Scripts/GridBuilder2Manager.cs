using UnityEngine;

public class GridBuilder2Manager : MonoBehaviour
{
    public static GridBuilder2Manager Instance { get; private set; }


    [SerializeField] GridSelector gridSelector;
    [SerializeField] ObjectPlacer objectPlacer;
    [SerializeField] ObjectRemover objectRemover;
    [SerializeField] ObjectSelector objectSelector;
    [SerializeField] GridObjectOptions gridObjectOptions;

    public GridSelector GridSelector { get => gridSelector; set => gridSelector = value; }
    public ObjectPlacer ObjectPlacer { get => objectPlacer; set => objectPlacer = value; }
    public ObjectRemover ObjectRemover { get => objectRemover; set => objectRemover = value; }
    public ObjectSelector ObjectSelector { get => objectSelector; set => objectSelector = value; }
    public GridObjectOptions GridObjectOptions { get => gridObjectOptions; set => gridObjectOptions = value; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;    
        }
    }
}

