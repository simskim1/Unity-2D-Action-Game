using UnityEngine;

public class StateMachine
{
    public IState CurrentState { get; private set; }

    // 첫 상태를 설정할 때 사용 (보통 Idle)
    public void Initialize(IState startingState)
    {
        CurrentState = startingState;
        CurrentState.Enter();
    }

    // 상태를 다른 상태로 바꿀 때 사용
    public void TransitionTo(IState nextState, bool force = false)
    {
        Debug.Log($"{CurrentState?.GetType().Name} -> {nextState.GetType().Name}");
        if (!force && CurrentState == nextState) return;

        CurrentState.Exit();       // 이전 상태 종료
        CurrentState = nextState;  // 새로운 상태로 교체
        CurrentState.Enter();      // 새로운 상태 시작
    }

    // 매 프레임마다 현재 상태의 Update를 실행
    public void Update()
    {
        if (CurrentState != null)
        {
            CurrentState.Update();
        }
    }
}