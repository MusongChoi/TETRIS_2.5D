using System.Collections.Generic;
using UnityEditor.XR;
using UnityEngine;

public class Mino : MonoBehaviour {
    [SerializeField] List<Material> material;
    [SerializeField] Material materialGhost;
    public int r;
    public int num;

    int[,,] Shape = {
        { { -1,  0 }, {  0,  0 }, {  1,  0 }, {  2,  0 } }, // I0
        { { -1,  1 }, { -1,  0 }, {  0,  0 }, {  1,  0 } }, // J1
        { {  1,  1 }, { -1,  0 }, {  0,  0 }, {  1,  0 } }, // L2
        { {  0,  0 }, {  0,  1 }, {  1,  0 }, {  1,  1 } }, // O3
        { { -1,  0 }, {  0,  0 }, {  0,  1 }, {  1,  1 } }, // S4
        { {  0,  1 }, { -1,  0 }, {  0,  0 }, {  1,  0 } }, // T5
        { { -1,  1 }, {  0,  1 }, {  0,  0 }, {  1,  0 } }  // Z6
    };

    List<Transform> children = new List<Transform>();


    void Awake() {
        foreach (Transform child in transform)
            children.Add(child);
    }

    public void SetShape(int _num, Vector3 pos, bool isGhost = false) {
        num = _num;
        transform.position = pos;
        for (int i = 0; i < 4; i++) {
            children[i].localPosition = new Vector3(Shape[num, i, 0], Shape[num, i, 1], 0);
            
            if (isGhost)
                children[i].GetComponent<MeshRenderer>().material = materialGhost;

            else
                children[i].GetComponent<MeshRenderer>().material = material[num];
        }
    }
}
