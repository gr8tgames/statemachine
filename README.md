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
    .OnEntry(() => SignalDialTone(DialTone.US))
    .Permit(Trigger.PickedUp, State.OffHook);

  machine.Fire(Trigger.PickedUp);

  // machine.State transitions to State.OffHook
```

The class is light-weight and has no dependencies. Non-permitted triggers are ignored. Use `OnEntry` and `OnExit` to invoke methods in the host class.

