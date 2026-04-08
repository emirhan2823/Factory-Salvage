using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using FactorySalvage.Core;

namespace FactorySalvage.Tests
{
    public class ServiceLocatorTests
    {
        #region Setup

        [SetUp]
        public void SetUp()
        {
            ServiceLocator.Clear();
        }

        [TearDown]
        public void TearDown()
        {
            ServiceLocator.Clear();
        }

        #endregion

        #region Tests

        [Test]
        public void Register_And_Get_Returns_Same_Instance()
        {
            var service = new TestService();
            ServiceLocator.Register<TestService>(service);

            var result = ServiceLocator.Get<TestService>();

            Assert.AreSame(service, result);
        }

        [Test]
        public void Get_Without_Register_Returns_Null()
        {
            LogAssert.Expect(LogType.Warning, "[ServiceLocator] Service not found: TestService");

            var result = ServiceLocator.Get<TestService>();

            Assert.IsNull(result);
        }

        [Test]
        public void Unregister_Removes_Service()
        {
            var service = new TestService();
            ServiceLocator.Register<TestService>(service);
            ServiceLocator.Unregister<TestService>();

            LogAssert.Expect(LogType.Warning, "[ServiceLocator] Service not found: TestService");
            var result = ServiceLocator.Get<TestService>();

            Assert.IsNull(result);
        }

        [Test]
        public void TryGet_Returns_True_When_Registered()
        {
            var service = new TestService();
            ServiceLocator.Register<TestService>(service);

            bool found = ServiceLocator.TryGet<TestService>(out var result);

            Assert.IsTrue(found);
            Assert.AreSame(service, result);
        }

        [Test]
        public void TryGet_Returns_False_When_Not_Registered()
        {
            bool found = ServiceLocator.TryGet<TestService>(out var result);

            Assert.IsFalse(found);
            Assert.IsNull(result);
        }

        [Test]
        public void Clear_Removes_All_Services()
        {
            ServiceLocator.Register<TestService>(new TestService());
            ServiceLocator.Register<AnotherTestService>(new AnotherTestService());
            ServiceLocator.Clear();

            LogAssert.Expect(LogType.Warning, "[ServiceLocator] Service not found: TestService");
            LogAssert.Expect(LogType.Warning, "[ServiceLocator] Service not found: AnotherTestService");

            Assert.IsNull(ServiceLocator.Get<TestService>());
            Assert.IsNull(ServiceLocator.Get<AnotherTestService>());
        }

        #endregion

        #region Test Helpers

        private class TestService { }
        private class AnotherTestService { }

        #endregion
    }
}
