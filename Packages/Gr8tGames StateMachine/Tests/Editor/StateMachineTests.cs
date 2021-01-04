using NUnit.Framework;

namespace Gr8tGames.Tests
{

  public class StateMachineTests
  {
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

    private StateMachine<State, Trigger> machine;

    [Test]
    public void TheInitialStateIsSetInTheConstructor()
    {
      machine = new StateMachine<State, Trigger>(State.OnHook);
      Assert.That(machine.State, Is.EqualTo(State.OnHook));

      machine = new StateMachine<State, Trigger>(State.OffHook);
      Assert.That(machine.State, Is.EqualTo(State.OffHook));
    }

    [Test]
    public void FiringPermittedTriggerInConfiguredStateTransitionsToNextState()
    {
      machine = new StateMachine<State, Trigger>(State.OnHook);
      machine.Configure(State.OnHook)
        .Permit(Trigger.PickedUp, State.OffHook);

      machine.Fire(Trigger.PickedUp);

      Assert.That(machine.State, Is.EqualTo(State.OffHook));
    }

    [Test]
    public void FiringAnUnpermittedTriggerInConfiguredStateHasNoEffect()
    {
      machine = new StateMachine<State, Trigger>(State.OnHook);
      machine.Configure(State.OnHook)
        .Permit(Trigger.PickedUp, State.OffHook);

      machine.Fire(Trigger.CallDialed);

      Assert.That(machine.State, Is.EqualTo(State.OnHook));
    }

    [Test]
    public void FiringTriggerInUnconfiguredStateHasNoEffect()
    {
      machine = new StateMachine<State, Trigger>(State.OnHook);

      machine.Fire(Trigger.PickedUp);

      Assert.That(machine.State, Is.EqualTo(State.OnHook));
    }

    [Test]
    public void FiringPermittedTriggerInConfiguredStateCallsOnEnterOfNextState()
    {
      bool wasCalled = false;
      machine = new StateMachine<State, Trigger>(State.OnHook);
      machine.Configure(State.OnHook)
        .Permit(Trigger.PickedUp, State.OffHook);

      machine.Configure(State.OffHook)
        .OnEnter(() => wasCalled = true);

      machine.Fire(Trigger.PickedUp);

      Assert.That(wasCalled, Is.True);
    }

    [Test]
    public void FiringPermittedTriggerInConfiguredStateCallsOnExitOfPreviousStateBeforeCallingOnEnterOfNextState()
    {
      int sum = 2;
      machine = new StateMachine<State, Trigger>(State.OnHook);
      machine.Configure(State.OnHook)
        .Permit(Trigger.PickedUp, State.OffHook)
        .OnExit(() => sum += 2);

      machine.Configure(State.OffHook)
        .OnEnter(() => sum /= 2);

      machine.Fire(Trigger.PickedUp);

      Assert.That(sum, Is.EqualTo(2));
    }

    [Test]
    public void FiringPermittedTriggerInConfiguredStateWithTransitionActionCallsActionBeforeOnExit()
    {
      int sum = 2;
      machine = new StateMachine<State, Trigger>(State.OnHook);
      machine.Configure(State.OnHook)
        .Permit(Trigger.PickedUp, State.OffHook)
        .OnExit(() => sum += 2);

      machine.Configure(State.OffHook)
        .OnEnter(() => sum /= 2);

      machine.Fire(Trigger.PickedUp, () => sum /= 2);

      Assert.That(sum, Is.EqualTo(1));
    }

    [Test]
    public void FiringNonPermittedTriggerInConfiguredStateWithTransitionActionDoesNotCallTransitionAction()
    {
      int sum = 3;
      machine = new StateMachine<State, Trigger>(State.OnHook);
      machine.Configure(State.OnHook)
        .OnExit(() => sum += 2);

      machine.Fire(Trigger.PickedUp, () => sum /= 3);

      Assert.That(sum, Is.EqualTo(3));
    }
  }
}
