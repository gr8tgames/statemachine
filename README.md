# State Machine

A simple implementation of a state machine for use in Unity projects.

```
  public enum State
  {
    OnHook,
    Ringing,
    Connected,
    OffHook
  }

  public enum Trigger
  {
    PickedUp,
    CallDialed,
    CallConnected,
    HungUp
  }

  machine = new StateMachine<State, Trigger>(State.OnHook);
  machine.Configure(State.OnHook)
    .OnEnter(() => SignalDialTone(DialTone.US))
    .Permit(Trigger.PickedUp, State.OffHook);

  machine.Fire(Trigger.PickedUp);

  // machine.State transitions to State.OffHook
```

The class is light-weight and has no dependencies.

Non-permitted triggers are ignored (nothing happens). Use `OnEnter` and `OnExit` to invoke methods in the host class. Transitions to the same state are allowed. `OnEnter` and `OnExit` are not called on transitions to self.

You can specify an action to call if the trigger is permitted. For example, given the configuration above:

```
  machine.Fire(Trigger.PickedUp, () => { // some transition method });
```

will invoke the transition method if the class is in `State.OnHook`. If the trigger is not permitted in that state, the transition method will not be called.

