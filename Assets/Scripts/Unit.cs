using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Unit : MonoBehaviour {


    public float x { get; private set; }
    public float y { get; private set; }
    public int rowIndex { get; private set; }
    public int colIndex { get; private set; }

    public int type = 0;
    public float moveCost = 0;

    public void SetPos(float x, float y) {
        this.x = x;
        this.y = y;
    }

    public void SetIndex(int row, int col) {
        this.rowIndex = row;
        this.colIndex = col;
    }

    public void SetMaterial(Material material) {
        GetComponent<MeshRenderer>().material = material;
    }

    public void DestroySelf() {
        Destroy(gameObject);
    }
}
