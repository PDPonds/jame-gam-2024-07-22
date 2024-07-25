using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class GameManager : Singleton<GameManager>
{
    NavMeshSurface nav;

    private void Awake()
    {
        nav = GetComponent<NavMeshSurface>();

    }

    private void Start()
    {
        BrakeNav();
    }

    public void BrakeNav()
    {
        nav.BuildNavMesh();
    }

}
