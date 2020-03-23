using System.Collections;
using System.Collections.Generic;
using System.Net.Http; //?
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class CharNavigationController : MonoBehaviour
{
    public float rotationSpeed;

    [SerializeField]
    private bool stopActionRequested;

    private NavMeshAgent nvm;
        
    private void Awake()
    {
        nvm = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        #if UNITY_EDITOR
        nvm.speed = 40;
        #endif
    }

    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Mouse0) && ClickProvider.ClickAvaliable)
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit, 100))
            {
                MoveToPoint(hit.point);
            }

        }
    }

    

    public async void TurnAndMoveToPoint(Vector3 destination)
    {
        var path = new NavMeshPath();
        nvm.CalculatePath(destination, path);
        nvm.SetPath(path);

        if (path.corners.Length == 0) return;
        
        await 
            RotateTowards(path.corners[0]);
        
        while (!stopActionRequested)
        {
            if (nvm.remainingDistance <= nvm.stoppingDistance) break;
            await new WaitForUpdate();
        }
    }

    public async Task MoveToPoint(Vector3 point)
    {
        var path = new NavMeshPath();
        nvm.CalculatePath(point, path);
        nvm.SetPath(path);
                
        while (!stopActionRequested)
        {
            if (nvm.remainingDistance <= nvm.stoppingDistance) break;
            await new WaitForUpdate();
        }
        
    }

    public async Task RotateTowards(Vector3 target)
    {
        var t = transform;
        var targetAngle = target - t.position;
        var angleDifference = Vector3.SignedAngle(t.forward, targetAngle, Vector3.up);

        var signedRotSpeed = Mathf.Sign(angleDifference) * rotationSpeed;
        angleDifference = Mathf.Abs(angleDifference);

        while (angleDifference > 0 && !stopActionRequested)
        {
            t.Rotate(0, signedRotSpeed * Time.deltaTime, 0);
            angleDifference -= rotationSpeed * Time.deltaTime;
            await new WaitForUpdate();
        }
    }
}




