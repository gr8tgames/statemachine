using System;
using System.Collections.Generic;

namespace Gr8tGames
{
  public class StateMachine<TState, TTrigger>
  {
    public TState State { get; private set; }

    private IDictionary<TState, IDictionary<TTrigger, TState>> transitions;
    private IDictionary<TState, Action> onEnter;
    private IDictionary<TState, Action> onExit;

    public StateMachine(TState initialState)
    {
      State = initialState;
      transitions = new Dictionary<TState, IDictionary<TTrigger, TState>>();
      onEnter = new Dictionary<TState, Action>();
      onExit = new Dictionary<TState, Action>();
    }

    public Configuration Configure(TState state)
    {
      return new Configuration(this, state);
    }

    public void Fire(TTrigger trigger)
    {
      if (!transitions.ContainsKey(State)) return;
      if (!transitions[State].ContainsKey(trigger)) return;

      if(onExit.ContainsKey(State))
      {
        onExit[State]();
      }
      
      State = transitions[State][trigger];

      if(onEnter.ContainsKey(State))
      {
        onEnter[State]();
      }
    }

    public class Configuration
    {
      private readonly StateMachine<TState, TTrigger> target;
      private readonly TState forState;

      public Configuration(StateMachine<TState, TTrigger> target, TState forState)
      {
        this.target = target;
        this.forState = forState;
      }

      public Configuration Permit(TTrigger trigger, TState nextState)
      {
        if (!target.transitions.ContainsKey(forState))
        {
          target.transitions[forState] = new Dictionary<TTrigger, TState>();
        }

        target.transitions[forState][trigger] = nextState;

        return this;
      }

      public Configuration OnEnter(Action action)
      {
        target.onEnter[forState] = action;
        return this;
      }

      public Configuration OnExit(Action action)
      {
        target.onExit[forState] = action;
        return this;
      }
    }
  }
}
