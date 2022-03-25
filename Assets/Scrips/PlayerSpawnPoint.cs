using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnPoint : MonoBehaviour
{
    // Start is called before the first frame update
    private void Awake() => PlayerSpawnSystem.AddSpawnPonint(transform);

    private void OnDestroy() => PlayerSpawnSystem.RemoveSpawnPoint(transform);

    private void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position,1f);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position,transform.position + transform.forward * 2);
    } 
}
