using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BrushType
{
    normal,
    obstacle,
    accelerate,
    decelerate
}

public class Brush : MonoBehaviour {

    Transform planeTrans;
    float topPos, bottomPos, leftPos, rightPos;
    UnitContainer containerScript;
    public BrushType type = BrushType.normal;
    Dictionary<BrushType, float> brushTypeTable = new Dictionary<BrushType, float>()
    { { BrushType.normal, 0f }, { BrushType.accelerate, -0.5f }, { BrushType.decelerate, 1f }, {BrushType.obstacle, 0f } };
    // Use this for initialization
    void Start () {
        containerScript = GameObject.Find("UnitContainer").GetComponent<UnitContainer>();
        planeTrans = GameObject.Find("Plane").GetComponent<Transform>();
        topPos = planeTrans.position.z + planeTrans.localScale.z * 5;
        bottomPos = planeTrans.position.z - planeTrans.localScale.z * 5;
        rightPos = planeTrans.position.x + planeTrans.localScale.x * 5;
        leftPos = planeTrans.position.x - planeTrans.localScale.x * 5;

        //print(new Vector4(topPos, bottomPos, rightPos, leftPos));
    }
	
	// Update is called once per frame
	void Update () {
        Vector3 mousePos3D = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos = new Vector2(mousePos3D.x, mousePos3D.z);
        if(mousePos.x <= rightPos && mousePos.x >= leftPos && mousePos.y <= topPos && mousePos.y >= bottomPos)
        {
            transform.position = new Vector3(mousePos.x, 2f, mousePos.y);
            if (Input.GetMouseButton(0))
            {
                if(type == BrushType.obstacle)
                {
                    containerScript.SetBlock(mousePos, transform.localScale.x / 2);
                }else if(type == BrushType.accelerate)
                {
                    containerScript.SetAccelerate(mousePos, transform.localScale.x / 2, brushTypeTable[type]);
                }else if(type == BrushType.decelerate)
                {
                    containerScript.SetDecelerate(mousePos, transform.localScale.x / 2, brushTypeTable[type]);
                }
            }
            if (Input.GetMouseButton(1))
            {
                containerScript.SetNormal(mousePos, transform.localScale.x / 2);
            }
        }
	}

    public void SetBrushType(string typeName) {
        print(typeName);
        type = (BrushType)System.Enum.Parse(typeof(BrushType), typeName.ToLower());
    }
}
