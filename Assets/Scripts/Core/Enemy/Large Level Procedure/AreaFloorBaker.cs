using System.Collections;
using NavMeshBuilder = UnityEngine.AI.NavMeshBuilder;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using Unity.AI.Navigation;
using System;
public class AreaFloorBaker : MonoBehaviour
{
    [SerializeField]
    private NavMeshSurface[] _surface;
    [SerializeField]
    private IEnemy _enemy;
    [SerializeField]
    private float _updateRate;
    [SerializeField]
    private float _movementThreshold = 3;
    [SerializeField]
    private Vector3 _navMeshSize = new Vector3(20, 20, 20);

    public delegate void NavMeshUpdateEvent(Bounds bounds);

    public event NavMeshUpdateEvent OnNavMeshUpdate;

    private Vector3 _worldAnchor;
    private NavMeshData[] _navMeshData;
    private List<NavMeshBuildSource> Sources = new List<NavMeshBuildSource>();

    private void Awake()
    {
        _navMeshData = new NavMeshData[_surface.Length];
        for (int i = 0; i < _surface.Length; i++)
        {
            _navMeshData[i] = new NavMeshData();
            NavMesh.AddNavMeshData(_navMeshData[i]);
        }
        BuildNavMesh(false);
        StartCoroutine(CheckEnemyMovement());

    }

    IEnumerator CheckEnemyMovement()
    {
        WaitForSeconds wait = new WaitForSeconds(_updateRate);

        while (true)
        {
            if (_enemy != null)
            {
                if (Vector3.Distance(_worldAnchor, _enemy.GameObject.transform.position) > _movementThreshold)
                {
                    _worldAnchor = _enemy.GameObject.transform.position;
                    BuildNavMesh(true);
                }
            }

            yield return wait;
        }
    }

    private void BuildNavMesh(bool isAsync)
    {
        Bounds bounds = new Bounds(_enemy.GameObject.transform.position, _navMeshSize);
        List<NavMeshBuildMarkup> markups = new List<NavMeshBuildMarkup>();

        List<NavMeshModifier> modifiers;
        for (int index = 0; index < _surface.Length; index++)
        {
            if (_surface[index].collectObjects == CollectObjects.Children)
            {
                modifiers = new List<NavMeshModifier>(_surface[index].GetComponentsInChildren<NavMeshModifier>());
            }
            else
            {
                modifiers = NavMeshModifier.activeModifiers;
            }

            for (int i = 0; i < modifiers.Count; i++)
            {
                NavMeshModifier modifier = modifiers[i];
                if ((_surface[index].layerMask & (1 << modifier.gameObject.layer)) == 1
                    && modifier.AffectsAgentType(_surface[index].agentTypeID))
                {
                    markups.Add(new NavMeshBuildMarkup()
                    {
                        root = modifier.transform,
                        overrideArea = modifier.overrideArea,
                        area = modifier.area,
                        ignoreFromBuild = modifier.ignoreFromBuild
                    });
                }
            }
            if (_surface[index].collectObjects == CollectObjects.Children)
            {
                NavMeshBuilder.CollectSources(
                    transform,
                    _surface[index].layerMask,
                    _surface[index].useGeometry,
                    _surface[index].defaultArea,
                    markups,
                    Sources);
            }
            else
            {
                NavMeshBuilder.CollectSources(
                    bounds,
                    _surface[index].layerMask,
                    _surface[index].useGeometry,
                    _surface[index].defaultArea,
                    markups,
                    Sources);
            }

            Sources.RemoveAll(source => source.component != null && source.component.gameObject.GetComponent<NavMeshAgent>() != null);

            if (isAsync)
            {
                AsyncOperation navMeshUpdateOperation = NavMeshBuilder.UpdateNavMeshDataAsync(_navMeshData[index], _surface[index].GetBuildSettings(), Sources, bounds);
                navMeshUpdateOperation.completed += HandleNavMeshUpdate;
            }
            else
            {
                NavMeshBuilder.UpdateNavMeshData(_navMeshData[index], _surface[index].GetBuildSettings(), Sources, bounds);
            }
        }


    }

    private void HandleNavMeshUpdate(AsyncOperation operation)
    {
        OnNavMeshUpdate?.Invoke(new Bounds(_worldAnchor, _navMeshSize));
    }
}
