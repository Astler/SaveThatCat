using System;
using Bee.Pool;
using Cat;
using Line;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class ProjectContext : MonoBehaviour
{
    [SerializeField] private LineDrawerView drawerView;
    [SerializeField] private CatView[] cats;

    [Header("Dependencies")] 
    [SerializeField] private BeesPool beesPool;

    public event Action GameStarted;

    public BeesPool GetBeesPool() => beesPool;

    public Transform GetBeesTarget() => cats[Random.Range(0, cats.Length)].GetTransform();

    private void Awake()
    {
        drawerView.LineFinished += OnLineFinished;

        foreach (CatView catView in cats)
        {
            catView.Died += OnCatDied;
        }
    }

    private void OnCatDied() => SceneManager.LoadScene("MainScene");

    private void OnLineFinished() => GameStarted?.Invoke();
}