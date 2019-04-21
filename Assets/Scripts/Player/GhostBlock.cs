using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostBlock : MonoBehaviour {


    // Use this for initialization
    void Start () {
        gameObject.name = "GhostBlock";
        tag = "CloneBlock";
        foreach(Transform block in transform){
            block.GetComponent<SpriteRenderer> ().color = new Color (1f, 1f, 1f, 0.2f);
        }
    }
    
    // Update is called once per frame
    void Update () {
        FollowActiveBlock ();
        MoveDown ();
    }

    void FollowActiveBlock(){
        Transform currentBlock = GameObject.FindGameObjectWithTag ("CurrentBlock").transform;

        transform.position = currentBlock.position;
        transform.rotation = currentBlock.rotation;


    }

    void MoveDown(){
        while(CheckIsValidPosition()){
            transform.position += new Vector3 (0, -1, 0);
        }

        if(!CheckIsValidPosition()){
            transform.position += new Vector3 (0, 1, 0);
        }

    }

    private bool CheckIsValidPosition(){
        foreach (Transform mino in transform)
        {
            Vector2 position = GameplayController.instance.Estimate(mino.position);
            if (!GameplayController.instance.CheckBlockInside(position))
                return false;
        }

        foreach (Transform mino in transform)
        {
            Vector2 position = GameplayController.instance.Estimate(mino.position);

            Transform trans = GameplayController.instance.GetGridPosition(position);

            if (trans != null && trans.parent.tag == "CurrentBlock")
                return true;
            
            if (trans != null && trans.parent != transform)
                return false;
        }
        return true;


    }

}
