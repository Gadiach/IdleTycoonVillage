using UnityEngine;

public class UniversityUI : MonoBehaviour
{
    public static UniversityUI Instance;

    [SerializeField] private GameObject panel;
    //[SerializeField] private BlueprintItemUI[] blueprintItems;

    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    public void OpenUniversityPanel()
    {
        panel.SetActive(true);
        RefreshAll();
    }

    public void CloseUniversityPanel()
    {
        panel.SetActive(false);
    }

    private void RefreshAll()
    {
        //Update info about blueprints for each tab. Make same as in Shop 
    }
}