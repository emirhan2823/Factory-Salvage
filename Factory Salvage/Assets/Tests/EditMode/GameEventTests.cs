using NUnit.Framework;
using UnityEngine;
using FactorySalvage.Core;

namespace FactorySalvage.Tests
{
    public class GameEventTests
    {
        #region GameEvent Tests

        [Test]
        public void GameEvent_Raise_Invokes_Registered_Listener()
        {
            var gameEvent = ScriptableObject.CreateInstance<GameEvent>();
            bool wasCalled = false;

            gameEvent.Register(() => wasCalled = true);
            gameEvent.Raise();

            Assert.IsTrue(wasCalled);

            Object.DestroyImmediate(gameEvent);
        }

        [Test]
        public void GameEvent_Unregister_Stops_Receiving()
        {
            var gameEvent = ScriptableObject.CreateInstance<GameEvent>();
            int callCount = 0;
            void Listener() => callCount++;

            gameEvent.Register(Listener);
            gameEvent.Raise();
            gameEvent.Unregister(Listener);
            gameEvent.Raise();

            Assert.AreEqual(1, callCount);

            Object.DestroyImmediate(gameEvent);
        }

        [Test]
        public void GameEvent_Multiple_Listeners_All_Called()
        {
            var gameEvent = ScriptableObject.CreateInstance<GameEvent>();
            int callCount = 0;

            gameEvent.Register(() => callCount++);
            gameEvent.Register(() => callCount++);
            gameEvent.Register(() => callCount++);
            gameEvent.Raise();

            Assert.AreEqual(3, callCount);

            Object.DestroyImmediate(gameEvent);
        }

        #endregion

        #region IntGameEvent Tests

        [Test]
        public void IntGameEvent_Raise_Passes_Value()
        {
            var gameEvent = ScriptableObject.CreateInstance<IntGameEvent>();
            int receivedValue = 0;

            gameEvent.Register(v => receivedValue = v);
            gameEvent.Raise(42);

            Assert.AreEqual(42, receivedValue);

            Object.DestroyImmediate(gameEvent);
        }

        #endregion

        #region FloatGameEvent Tests

        [Test]
        public void FloatGameEvent_Raise_Passes_Value()
        {
            var gameEvent = ScriptableObject.CreateInstance<FloatGameEvent>();
            float receivedValue = 0f;

            gameEvent.Register(v => receivedValue = v);
            gameEvent.Raise(3.14f);

            Assert.AreEqual(3.14f, receivedValue, 0.001f);

            Object.DestroyImmediate(gameEvent);
        }

        #endregion
    }
}
