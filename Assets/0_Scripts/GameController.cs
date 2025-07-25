using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Collections;
using Unity.VisualScripting;

public class GameController : MonoBehaviour {

    [SerializeField] TextMeshProUGUI txt;
    [SerializeField] GameObject ghostMino;

    [SerializeField] float maxDownTime;
    [SerializeField] float maxDas;
    [SerializeField] float maxArr;
    [SerializeField] float maxLockDelay;

    const int fieldWidth = 10;
    const int fieldHeight = 30;

    int[,,] srsTable = { // JLTSZ
        { { -1,  0}, { -1,  1}, {  0, -2}, { -1, -2} }, // 0 > 1
        { {  1,  0}, {  1,  1}, {  0, -2}, {  1, -2} }, // 0 > 3

        { {  1,  0}, {  1, -1}, {  0,  2}, {  1,  2} }, // 1 > 2
        { {  1,  0}, {  1, -1}, {  0,  2}, {  1,  2} }, // 1 > 0
        
        { {  1,  0}, {  1,  1}, {  0, -2}, {  1, -2} }, // 2 > 3
        { { -1,  0}, { -1,  1}, {  0, -2}, { -1, -2} }, // 2 > 1

        { { -1,  0}, { -1, -1}, {  0,  2}, { -1,  2} }, // 3 > 0
        { { -1,  0}, { -1, -1}, {  0,  2}, { -1,  2} }  // 3 > 2
    };
    int[,,] srsTable2 = { // I
        { { -2,  0}, {  1,  0}, { -2, -1}, {  1,  2} }, // 0 > 1
        { { -1,  0}, {  2,  0}, { -1,  2}, {  2, -1} }, // 0 > 3

        { { -1,  0}, {  2,  0}, { -1,  2}, {  2, -1} }, // 1 > 2
        { {  2,  0}, { -1,  0}, {  2,  1}, { -1, -2} }, // 1 > 0
        
        { {  2,  0}, { -1,  0}, {  2,  1}, { -1, -2} }, // 2 > 3
        { {  1,  0}, { -2,  0}, {  1, -2}, { -2,  1} }, // 2 > 1

        { {  1,  0}, { -2,  0}, {  1, -2}, { -2,  1} }, // 3 > 0
        { { -2,  0}, {  1,  0}, { -2, -1}, {  1,  2} }  // 3 > 2
    };

    GameObject curMino;
    GameObject[,] field = new GameObject[fieldHeight, fieldWidth];
    Camera cam;

    float maxDownArr;
    float maxDownDas;

    float curDownTime;
    float curArr;
    float curDas;
    float curLockDelay;
    float curDownArr;
    float curDownDas;

    Vector3 camPos, camDPos, camShake;

    ObjectManager objectManager;



    void Start() {
        maxDownArr = maxArr;
        maxDownDas = maxDas;
        cam = Camera.main;
        camPos = cam.transform.position;
        objectManager = GameObject.Find("Object Manager").GetComponent<ObjectManager>();
        objectManager.InitMinoBag();
        GetNewBlock();
    }

    void Update() {
        if (curMino == null) {
            return;
        }

        if (curDownTime < maxDownTime)
            curDownTime += Time.deltaTime;
        else if (CheckMove(curMino, 0, -1, 0)) {
            if (CheckMove(curMino, 0, -1, 0, false))
                curDownTime = 0f;
        }
        else if (curLockDelay < maxLockDelay) {
            curLockDelay += Time.deltaTime;
        }
        else {
            LockBlock();
        }


        // 스페이스키 하드드롭
        if (Input.GetKeyDown(KeyCode.Space)) {
            while (CheckMove(curMino, 0, -1, 0)) ;
            camShake = Vector3.down * 0.5f;
            LockBlock();
        }


        // 왼쪽 키
        if (Input.GetKeyDown(KeyCode.LeftArrow) && CheckMove(curMino, -1, 0, 0)) {
            curArr = 0f;
            curDas = 0f;
        }
        else if (Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow)) {
            if (curDas < maxDas)
                curDas += Time.deltaTime;

            else if (curArr < maxArr)
                curArr += Time.deltaTime;

            else if (CheckMove(curMino, -1, 0, 0))
                curArr = 0f;

            else
                camDPos.x -= Time.deltaTime * 2;
        }


        // 오른쪽 키 
        if (Input.GetKeyDown(KeyCode.RightArrow) && CheckMove(curMino, 1, 0, 0)) {
            curArr = 0f;
            curDas = 0f;
        }
        else if (Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.LeftArrow)) {
            if (curDas < maxDas)
                curDas += Time.deltaTime;

            else if (curArr < maxArr)
                curArr += Time.deltaTime;

            else if (CheckMove(curMino, 1, 0, 0))
                curArr = 0f;

            else
                camDPos.x += Time.deltaTime * 2;
        }


        // 아래쪽 키 
        if (Input.GetKeyDown(KeyCode.DownArrow) && CheckMove(curMino, 0, -1, 0)) {
            curDownArr = 0f;
            curDownDas = 0f;
        }
        else if (Input.GetKey(KeyCode.DownArrow)) {
            if (curDownDas < maxDownDas)
                curDownDas += Time.deltaTime;

            else if (curDownArr < maxDownArr)
                curDownArr += Time.deltaTime;

            else if (CheckMove(curMino, 0, -1, 0))
                curDownArr = 0f;

        }


        // x 키 (시계방향회전) 
        if (Input.GetKeyDown(KeyCode.X)) {
            CheckRotate(1);
            ghostMino.GetComponent<Mino>().r = curMino.GetComponent<Mino>().r;
            ghostMino.transform.eulerAngles = curMino.transform.eulerAngles;
        }

        // z 키 (반시계방향회전)
        if (Input.GetKeyDown(KeyCode.Z)) {
            CheckRotate(-1);
            ghostMino.GetComponent<Mino>().r = curMino.GetComponent<Mino>().r;
            ghostMino.transform.eulerAngles = curMino.transform.eulerAngles;
        }

        camDPos *= 0.95f;
        camShake *= -0.9f;
        cam.transform.position = camPos - camDPos - camShake;

        MoveGhostMino();
    }

    void CheckRotate(int dr) {
        if (curMino.name == "Mino_O") return;

        int r = curMino.GetComponent<Mino>().r;
        int nr = (r + dr + 4) % 4;
        int tmp = dr > 0 ? 0 : 1;

        if (CheckMove(curMino, 0, 0, dr)) {
            curMino.GetComponent<Mino>().r = nr;
            return;
        }

        if (curMino.name != "Mino_I") {
            for (int i = 0; i < 4; i++) {
                if (CheckMove(curMino, srsTable[r * 2 + tmp, i, 0], srsTable[r * 2 + tmp, i, 1], dr)) {
                    curMino.GetComponent<Mino>().r = nr;
                    return;
                }
            }
        }
        else {
            for (int i = 0; i < 4; i++) {
                if (CheckMove(curMino, srsTable2[r * 2 + tmp, i, 0], srsTable2[r * 2 + tmp, i, 1], dr)) {
                    curMino.GetComponent<Mino>().r = nr;
                    return;
                }
            }
        }
    }

    bool CheckMove(GameObject mino, int dx, int dy, int dr, bool move = true) {
        mino.transform.position += Vector3.right * dx + Vector3.up * dy;
        mino.transform.eulerAngles += Vector3.forward * -90 * dr;

        foreach (Transform child in mino.transform) {
            int x = Mathf.RoundToInt(child.position.x);
            int y = Mathf.RoundToInt(child.position.y);

            if (x < 0 || x >= 10 || y < 0 || y >= 25 || field[y, x]) {
                mino.transform.position -= Vector3.right * dx + Vector3.up * dy;
                mino.transform.eulerAngles -= Vector3.forward * -90 * dr;
                return false;
            }
        }

        if (!move) {
            mino.transform.position -= Vector3.right * dx + Vector3.up * dy;
            mino.transform.eulerAngles -= Vector3.forward * -90 * dr;
        }
        return true;
    }


    void GetNewBlock() {
        curMino = objectManager.GetMino();
        ghostMino.GetComponent<Mino>().SetShape(curMino.GetComponent<Mino>().num, new Vector3(4f, 20f, 0), true);
        ghostMino.transform.eulerAngles = Vector3.zero;
    }

    void LockBlock() {
        camShake += Vector3.down * 0.5f;
        curLockDelay = 0f;
        curDownArr = 0f;
        curDownTime = 0f;
        curArr = 0f;
        curDas = 0f;



        List<Transform> children = new List<Transform>();
        foreach (Transform child in curMino.transform)
            children.Add(child);

        foreach (Transform child in children) {
            int x = Mathf.RoundToInt(child.position.x);
            int y = Mathf.RoundToInt(child.position.y);
            field[y, x] = child.gameObject;
            child.SetParent(transform);
        }
        Destroy(curMino);
        CheckLine();
        GetNewBlock();
    }

    void CheckLine() {
        int[] lines = new int[4];
        int k = 0;

        for (int i = 0; i < fieldHeight - 1; i++) {
            int cnt = 0;
            for (int j = 0; j < fieldWidth; j++) {
                if (field[i, j]) cnt++;
            }
            if (cnt == fieldWidth)
                lines[k++] = i;
        }

        for (; k-- > 0;) {
            for (int j = 0; j < fieldWidth; j++)
                Destroy(field[lines[k], j]);

            for (int i = lines[k]; i < fieldHeight - 1; i++) {
                for (int j = 0; j < fieldWidth; j++) {
                    field[i, j] = field[i + 1, j];
                    if (field[i, j])
                        field[i, j].transform.position += Vector3.down;
                }
            }
        }
    }

    void MoveGhostMino() {
        ghostMino.transform.localPosition = curMino.transform.localPosition;
        while (CheckMove(ghostMino, 0, -1, 0));
    }
}
