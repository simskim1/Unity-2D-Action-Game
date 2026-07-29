using UnityEngine;

public interface IState
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Enter();
    void Update();
    void Exit();
}
