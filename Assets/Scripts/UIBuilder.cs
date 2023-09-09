using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBuilder : MonoBehaviour
{
    [SerializeField] private MainDataHandler MDH;

    [SerializeField] private GameObject BuildingsList;

    [SerializeField] private GameObject BuildingLotPrefab;
    // Start is called before the first frame update
    void Start()
    {

    }

    public void Init()
    {
        int index = 0;
        foreach (var IdBuildingPair in MDH.myDB.buildings)
        {
            var obj = Instantiate(BuildingLotPrefab, BuildingsList.transform);
            var script = obj.GetComponent<BuildingLotClass>();
            script.building = IdBuildingPair.Value;
            obj.transform.localPosition = new Vector3(0, -75 - 100 * index, 0);
            index++;
        }
        BuildingsList.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 100 * index + 50);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
