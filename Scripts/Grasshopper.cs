using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Grasshopper : MonoBehaviour
{
    
    public GameObject[] targets;
    public float timeBuffer = 10f;
    public float launchForce = 5f;
    public GameManager gameManager;
    Rigidbody rb;
    Vector3 startingPosition;
    Vector3 targetStartingPosition;
    List<Vector3> listOfPositions;
    
    int fallenBlocks = 0;
    int lastCount = 0;
    bool canLaunch = true;
    // Start is called before the first frame update

    void Start()
    {
        startingPosition = transform.position;

        var totalX = 0f;
        var totalY = 0f;
        listOfPositions = new List<Vector3>();
        foreach(var target in targets)
        {
            totalX += target.transform.position.x;
            totalY += target.transform.position.y;
            listOfPositions.Add(target.transform.position);
        }
        var centerX = totalX / targets.Length;
        var centerY = totalY / targets.Length;

        targetStartingPosition = new Vector3(centerX, centerY + 1f, targets[0].transform.position.z);;
        Time.timeScale = timeBuffer; // allow for slowing time to see what's happening
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //f to fire
        if (Input.GetKeyDown(KeyCode.F) && canLaunch)
        {
            if(rb.isKinematic) rb.isKinematic = false;
            FiringSolution firing = new FiringSolution();
            Debug.Log("In the Space function");
            Nullable<Vector3> aimVector = firing.calculateFiringSolution(transform.position, targetStartingPosition, launchForce, Physics.gravity);
            if (aimVector.HasValue)
                rb.AddForce(aimVector.Value.normalized * 15/14 * launchForce, ForceMode.Impulse);
            canLaunch = false;
        }
        if (Input.GetKeyDown(KeyCode.R))
            moveSphereBack();
        if (Input.GetKeyDown(KeyCode.B))
            resetBlocks();
        
        checkIfBlocksFell(targets);
    } 
    async void OnCollisionEnter(Collision col){
        if (col.gameObject.tag == "block")
        {   
            await Task.Delay(1500);
            moveSphereBack();
            canLaunch = true;
        }
    }

    async void checkIfBlocksFell(GameObject[] targets)
    {
        fallenBlocks = 0;
        foreach(var target in targets)
        {
            if(target.transform.position.y < .9)
            {
                fallenBlocks++;
            }
        }
        if(fallenBlocks == targets.Length && lastCount != targets.Length)
        {
            lastCount = targets.Length;
            await Task.Delay(1500);
            gameManager.LoadNextLevel();
        }
    }

    private void moveSphereBack()
    {
        rb.isKinematic = true;
        transform.position = startingPosition;
        rb.isKinematic = false;
    }

    private void resetBlocks()
    {
        int count = 0;
        foreach (var position in listOfPositions)
        {
            targets[count].GetComponent<Rigidbody>().isKinematic = true;
            targets[count].transform.position = position;
            targets[count].GetComponent<Rigidbody>().isKinematic = false;
        }
    }
    
}