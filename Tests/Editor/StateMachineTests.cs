using NUnit.Framework;
using System;

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

    private bool onEnterWasCalled;
    private bool onExitWasCalled;
    private bool onTransitionWasCalled;

    private void onEnter() { onEnterWasCalled = true; }
    private void onExit() { onExitWasCalled = true; }
    private void onTransition() { onTransitionWasCalled = true; }

    public class GivenStateIsConfigured : StateMachineTests
    {
      public class GivenTriggerIsPermitted : GivenStateIsConfigured
      {
        public class WhenTriggerIsFired : GivenTriggerIsPermitted
        {
          [SetUp]
          public void BeforeEach()
          {
            onEnterWasCalled = false;
            onExitWasCalled = false;
            onTransitionWasCalled = false;

            machine = new StateMachine<State, Trigger>(State.OnHook);

            machine.Configure(State.OnHook)
              .OnExit(onExit)
              .Permit(Trigger.PickedUp, State.OffHook);

            machine.Configure(State.OffHook)
              .OnEnter(onEnter);
          }

          [Test]
          public void StateTransitionsFromOneStateToTheNext()
          {
            machine.Fire(Trigger.PickedUp);

            Assert.That(machine.State, Is.EqualTo(State.OffHook));
          }

          [Test]
          public void OnExitMethodIsCalled()
          {
            machine.Fire(Trigger.PickedUp);

            Assert.That(onExitWasCalled, Is.True);
          }

          [Test]
          public void OnEnterMethodIsCalled()
          {
            machine.Fire(Trigger.PickedUp);

            Assert.That(onEnterWasCalled, Is.True);
          }

          [Test]
          public void TransitionMethodIsCalled()
          {
            machine.Fire(Trigger.PickedUp, onTransition);

            Assert.That(onTransitionWasCalled, Is.True);
          }
        }
      }

      public class GivenTriggerIsPermittedToSameState : GivenStateIsConfigured
      {
        public class WhenTriggerIsFired : GivenTriggerIsPermitted
        {
          [SetUp]
          public void BeforeEach()
          {
            onEnterWasCalled = false;
            onExitWasCalled = false;
            onTransitionWasCalled = false;

            machine = new StateMachine<State, Trigger>(State.OnHook);

            machine.Configure(State.OnHook)
              .OnEnter(onEnter)
              .OnExit(onExit)
              .Permit(Trigger.HungUp, State.OnHook);
          }

          [Test]
          public void ShouldRemainInTheSameState()
          {
            machine.Fire(Trigger.HungUp);

            Assert.That(machine.State, Is.EqualTo(State.OnHook));
          }

          [Test]
          public void OnEnterShouldNotBeCalled()
          {
            machine.Fire(Trigger.HungUp);

            Assert.That(onEnterWasCalled, Is.False);
          }

          [Test]
          public void OnExitShouldNotBeCalled()
          {
            machine.Fire(Trigger.HungUp);

            Assert.That(onExitWasCalled, Is.False);
          }

          [Test]
          public void TransitionActionShouldBeCalled()
          {
            machine.Fire(Trigger.HungUp, onTransition);

            Assert.That(onTransitionWasCalled, Is.True);
          }
        }
      }

      public class GivenTriggerIsNotPermitted : StateMachineTests
      {
        public class WhenTriggerIsFired : GivenTriggerIsNotPermitted
        {
          [SetUp]
          public void BeforeEach()
          {
            onEnterWasCalled = false;
            onExitWasCalled = false;
            onTransitionWasCalled = false;

            machine = new StateMachine<State, Trigger>(State.OnHook);

            machine.Configure(State.OnHook)
              .OnExit(onExit)
              .Permit(Trigger.PickedUp, State.OffHook);

            machine.Configure(State.OffHook)
              .OnEnter(onEnter)
              .Permit(Trigger.HungUp, State.OnHook);
          }

          [Test]
          public void ShouldRemainInTheSameState()
          {
            machine.Fire(Trigger.CallConnected);

            Assert.That(machine.State, Is.EqualTo(State.OnHook));
          }

          [Test]
          public void OnEnterShouldNotBeCalled()
          {
            machine.Fire(Trigger.CallConnected);

            Assert.That(onEnterWasCalled, Is.False);
          }

          [Test]
          public void OnExitShouldNotBeCalled()
          {
            machine.Fire(Trigger.CallConnected);

            Assert.That(onExitWasCalled, Is.False);
          }

          [Test]
          public void TransitionMethodsShouldNotBeCalled()
          {
            machine.Fire(Trigger.CallConnected, onTransition);

            Assert.That(onTransitionWasCalled, Is.False);
          }
        }
      }
    }

    public class GivenStateIsNotConfigured : StateMachineTests
    {
      public class WhenTriggerIsFired : GivenStateIsNotConfigured
      {
        [SetUp]
        public void BeforeEach()
        {
          onEnterWasCalled = false;
          onExitWasCalled = false;
          onTransitionWasCalled = false;

          machine = new StateMachine<State, Trigger>(State.Connected);

          machine.Configure(State.OnHook)
            .OnExit(onExit)
            .Permit(Trigger.PickedUp, State.OffHook);

          machine.Configure(State.OffHook)
            .OnEnter(onEnter)
            .Permit(Trigger.HungUp, State.OnHook);
        }

        [Test]
        public void ShouldRemainInTheSameState()
        {
          machine.Fire(Trigger.PickedUp);

          Assert.That(machine.State, Is.EqualTo(State.Connected));
        }

        [Test]
        public void OnEnterShouldNotBeCalled()
        {
          machine.Fire(Trigger.PickedUp);

          Assert.That(onEnterWasCalled, Is.False);
        }

        [Test]
        public void OnExitShouldNotBeCalled()
        {
          machine.Fire(Trigger.PickedUp);

          Assert.That(onExitWasCalled, Is.False);
        }

        [Test]
        public void TransitionMethodsShouldNotBeCalled()
        {
          machine.Fire(Trigger.PickedUp, onTransition);

          Assert.That(onTransitionWasCalled, Is.False);
        }
      }
    }
  }
}
