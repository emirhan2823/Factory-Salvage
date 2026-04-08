using NUnit.Framework;
using UnityEngine;
using FactorySalvage.Core;

namespace FactorySalvage.Tests
{
    public class VariableTests
    {
        #region FloatVariable Tests

        [Test]
        public void FloatVariable_Set_Value_Triggers_Event()
        {
            var variable = ScriptableObject.CreateInstance<FloatVariable>();
            float receivedValue = 0f;

            variable.OnValueChanged += v => receivedValue = v;
            variable.Value = 10.5f;

            Assert.AreEqual(10.5f, receivedValue, 0.001f);
            Assert.AreEqual(10.5f, variable.Value, 0.001f);

            Object.DestroyImmediate(variable);
        }

        [Test]
        public void FloatVariable_Same_Value_Does_Not_Trigger()
        {
            var variable = ScriptableObject.CreateInstance<FloatVariable>();
            int callCount = 0;

            variable.OnValueChanged += _ => callCount++;
            variable.Value = 5f;
            variable.Value = 5f;

            Assert.AreEqual(1, callCount);

            Object.DestroyImmediate(variable);
        }

        [Test]
        public void FloatVariable_Add_Increases_Value()
        {
            var variable = ScriptableObject.CreateInstance<FloatVariable>();
            variable.Value = 10f;
            variable.Add(5f);

            Assert.AreEqual(15f, variable.Value, 0.001f);

            Object.DestroyImmediate(variable);
        }

        #endregion

        #region IntVariable Tests

        [Test]
        public void IntVariable_Set_Value_Triggers_Event()
        {
            var variable = ScriptableObject.CreateInstance<IntVariable>();
            int receivedValue = 0;

            variable.OnValueChanged += v => receivedValue = v;
            variable.Value = 42;

            Assert.AreEqual(42, receivedValue);
            Assert.AreEqual(42, variable.Value);

            Object.DestroyImmediate(variable);
        }

        [Test]
        public void IntVariable_Add_Increases_Value()
        {
            var variable = ScriptableObject.CreateInstance<IntVariable>();
            variable.Value = 10;
            variable.Add(3);

            Assert.AreEqual(13, variable.Value);

            Object.DestroyImmediate(variable);
        }

        #endregion

        #region RuntimeSet Tests

        [Test]
        public void RuntimeSet_Add_And_Remove()
        {
            var set = ScriptableObject.CreateInstance<TransformRuntimeSet>();
            var go = new GameObject("Test");

            set.Add(go.transform);
            Assert.AreEqual(1, set.Count);
            Assert.AreSame(go.transform, set.Items[0]);

            set.Remove(go.transform);
            Assert.AreEqual(0, set.Count);

            Object.DestroyImmediate(go);
            Object.DestroyImmediate(set);
        }

        [Test]
        public void RuntimeSet_No_Duplicates()
        {
            var set = ScriptableObject.CreateInstance<TransformRuntimeSet>();
            var go = new GameObject("Test");

            set.Add(go.transform);
            set.Add(go.transform);

            Assert.AreEqual(1, set.Count);

            Object.DestroyImmediate(go);
            Object.DestroyImmediate(set);
        }

        #endregion
    }
}
