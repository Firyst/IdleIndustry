using System.Collections;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class BuildingLotClass : MonoBehaviour
{

    public Building building;
    public Button mainButton;
    [SerializeField] private TextMeshProUGUI LotTitle;
    [SerializeField] private Image thumbnail;
    

    // Start is called before the first frame update
    void Start()
    {
        LotTitle.text = building.id + ".name";
    }

}
