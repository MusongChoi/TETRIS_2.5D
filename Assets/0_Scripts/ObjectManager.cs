using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour {
    [SerializeField] GameObject minoPrefab;
    [SerializeField] List<Transform> nextTransforms;
    List<GameObject> minoBag = new List<GameObject>();

    void Awake() {
        //InitMinoBag();
    }

    public void InitMinoBag() {
        Generate7Mino();
        Generate7Mino();
    }

    void Generate7Mino() {
        int[] bag = {0, 1, 2, 3, 4, 5, 6};
        for (int i = 0; i < 7; i++) {
            int j = Random.Range(i, 7);
            int t = bag[i];
            bag[i] = bag[j];
            bag[j] = t;
        }

        for (int i = 0; i < 7; i++) {
            GameObject inst = Instantiate(minoPrefab, transform);
            inst.GetComponent<Mino>().SetShape(bag[i], new Vector3(4f, 100f, 0));
            inst.name = "Mino_"+"IJLOSTZ"[bag[i]];
            minoBag.Add(inst);
        }
    }

    void SetNextPos() {
        for (int i = 0; i < nextTransforms.Count; i++) {
            minoBag[i].transform.position = nextTransforms[i].position;
        }
        if (minoBag.Count == 7) {
            Generate7Mino();
        }
    }

    public GameObject GetMino() {
        GameObject re = minoBag[0];
        re.transform.localPosition = new Vector3(4f, 20f, 0);
        minoBag.Remove(re);
        SetNextPos();
        return re;
    }
}
