using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager _instance {
        get; private set;
    }

    GameObject unitContainer;
    GameObject brush;

	void Awake() {
        _instance = this;
    }

	void Start () {
        unitContainer = GameObject.Find("UnitContainer");
        brush = GameObject.Find("BrushCylinder");
        brush.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    public void FillContainer(int rowNum) {
        unitContainer.GetComponent<UnitContainer>().Fill(rowNum);
        print("rowNum: " + rowNum);
    }

    public void SetBrush(float brushSize) {
        if (brush.activeSelf)
        {
            brush.SetActive(false);
            unitContainer.GetComponent<UnitContainer>().mouse = MouseMode.none;
            return;
        }
        brush.SetActive(true);
        brush.transform.localScale = new Vector3(brushSize, 0.1f, brushSize);
        unitContainer.GetComponent<UnitContainer>().mouse = MouseMode.brush;
    }

    public void SetBrushType(string brushType) {
        brush.GetComponent<Brush>().SetBrushType(brushType);
    }

    public void SetStart() {
        if (brush.activeSelf)
            brush.SetActive(false);
        unitContainer.GetComponent<UnitContainer>().mouse = MouseMode.start;
    }

    public void SetDest() {
        if (brush.activeSelf)
            brush.SetActive(false);
        unitContainer.GetComponent<UnitContainer>().mouse = MouseMode.dest;
    }

    public void SearchRoute() {
        if (brush.activeSelf) brush.SetActive(false);
        unitContainer.GetComponent<UnitContainer>().mouse = MouseMode.none;
        unitContainer.GetComponent<UnitContainer>().ShowRoute();
    }

    public void ResetMap() {
        if (brush.activeSelf) brush.SetActive(false);
        unitContainer.GetComponent<UnitContainer>().mouse = MouseMode.none;
        unitContainer.GetComponent<UnitContainer>().ResetMap();
    }
}
