using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainUI : MonoBehaviour {
    

    Button setSizeButton;
    int maxRowNum = 50, minRowNum = 10;
    InputField setSizeInputField;

    Button selectBrushButton;
    ToggleGroup brushSizeGroup;
    Dictionary<string, float> brushSizeTable = new Dictionary<string, float>() { { "Small", 3f }, { "Mid", 7f }, { "Big", 13f } };

    Button setStartButton, setDestButton, searchButton;

    Button resetButton;
	
	void Start () {
        setSizeButton = transform.Find("RightUI/SetSize/SetSizeButton").GetComponent<Button>();
        setSizeButton.onClick.AddListener(delegate () {
            FillContainer();
        });
        setSizeInputField = transform.Find("RightUI/SetSize/InputField").GetComponent<InputField>();

        selectBrushButton = transform.Find("RightUI/Brush/SelectBrushButton").GetComponent<Button>();
        selectBrushButton.onClick.AddListener(delegate() {
            SetBrush();
        });
        brushSizeGroup = transform.Find("RightUI/Brush/BrushSize").GetComponent<ToggleGroup>();

        setStartButton = transform.Find("RightUI/StartDest/SetStartButton").GetComponent<Button>();
        setStartButton.onClick.AddListener(new UnityEngine.Events.UnityAction(SetStart));
        setDestButton = transform.Find("RightUI/StartDest/SetDestButton").GetComponent<Button>();
        setDestButton.onClick.AddListener(new UnityEngine.Events.UnityAction(SetDest));
        searchButton = transform.Find("RightUI/StartDest/SearchButton").GetComponent<Button>();
        searchButton.onClick.AddListener(new UnityEngine.Events.UnityAction(SearchRoute));

        resetButton = transform.Find("RightUI/ResetButton").GetComponent<Button>();
        resetButton.onClick.AddListener(new UnityEngine.Events.UnityAction(ResetMap));
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void FillContainer() {
        int rowNum = int.Parse(setSizeInputField.text);
        rowNum = Mathf.Clamp(rowNum, minRowNum, maxRowNum);
        setSizeInputField.text = rowNum.ToString();
        GameManager._instance.FillContainer(rowNum);
        print("rowNum: " + rowNum);
    }

    void SetBrush() {
        IEnumerable<Toggle> activeToggle = brushSizeGroup.ActiveToggles();
        string sizeName;
        float brushSize;
        foreach (Toggle t in activeToggle)
        {
            sizeName = t.transform.GetComponentInChildren<Text>().text;
            brushSize = brushSizeTable[sizeName];
            GameManager._instance.SetBrush(brushSize);
        }
        
    }

    void SetStart() {
        GameManager._instance.SetStart();
    }

    void SetDest() {
        GameManager._instance.SetDest();
    }

    void SearchRoute() {
        GameManager._instance.SearchRoute();
    }

    void ResetMap() {
        GameManager._instance.ResetMap();
    }
}
