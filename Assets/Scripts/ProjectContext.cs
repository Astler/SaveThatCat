using System;
using Cat;
using Line;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ProjectContext : MonoBehaviour
{
    [SerializeField] private LineDrawerView drawerView;
    [SerializeField] private CatView catView;

    public event Action GameStarted;

    public Transform GetBeesTarget() => catView.GetTransform();

    private void Awake()
    {
        drawerView.LineFinished += OnLineFinished;
        catView.Died += OnCatDied;
    }

    private void OnCatDied()
    {
        SceneManager.LoadScene("MainScene");
    }

    private void OnLineFinished()
    {
        GameStarted?.Invoke();
    }
}