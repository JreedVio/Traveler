/*
Code Source: https://game.courses/bots-ai-statemachines/
Author: Jason Weimann
*/
public interface IState
{
    void Tick();
    void OnEnter();
    void OnExit();
}