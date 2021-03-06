﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using Microsoft.Practices.Unity;
using PersistentLayer.Repositories;
using System.Linq;
using PersistentLayer.Entities;
using System.Data.Entity.Infrastructure;
using EntityFramework.Repository6.Interfaces;
using EntityFramework.Repository6;
using Unity.Lifetime;
using Unity.Injection;
using Unity;

namespace EntityFramework.Repository6.Tests
{
    [TestClass]
    public class DeltaTests
    {
        UnityContainer LocalIoCContainer { get; set; }

        [TestInitialize]
        public void Setup()
        {
            Action<string> logSetup = message => Debug.WriteLine(message);

            LocalIoCContainer = new UnityContainer();
            LocalIoCContainer.RegisterType<ISimpleDataEntityRepository, SimpleDataEntityRepository>(new HierarchicalLifetimeManager());
            LocalIoCContainer.RegisterType<ISimpleCompositeKeyEntityRepository, SimpleCompositeKeyEntityRepository>(new HierarchicalLifetimeManager());

            //Use a physical database file
            //var connectionString = @"Data Source = (LocalDb)\MSSQLLocalDB; Database = TestDatabase; Integrated Security = True; Pooling = false; MultipleActiveResultSets = true";
            //or
            //var connectionString = @"TestDatabase";
            //LocalIoCContainer.RegisterType(typeof(IDatabaseFactory<>), typeof(DatabaseFactory<>)
            //    , new HierarchicalLifetimeManager()
            //    , new InjectionConstructor(connectionString)
            //    , new InjectionProperty("Logging", logSetup));

            //Use an nmemory database
            LocalIoCContainer.RegisterType(typeof(IDatabaseFactory<>), typeof(TestExampleDatabaseFactory)
                , new HierarchicalLifetimeManager()
                , new InjectionProperty("Logging", logSetup));
        }

        [TestCategory("StandardRepository")]
        [TestMethod]
        public void DeltaTest1()
        {
            var repository = LocalIoCContainer.Resolve<ISimpleDataEntityRepository>();
            var newItem = new SimpleDataEntity { Name = "Delta Test" };
            var actual = repository.Add(newItem);
            var result = repository.Save();

            var delta1 = new Delta<SimpleDataEntity>();
            delta1.SetValue("Name", "Delta Change Test");

            repository.Update(delta1, newItem.Id);

            repository.Save();

            var updatedValue = repository.Find(actual.Id);
            Assert.AreEqual("Delta Change Test", updatedValue.Name);
        }

        [TestCategory("StandardRepository")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void DeltaTestWrongDataType1()
        {
            var repository = LocalIoCContainer.Resolve<ISimpleDataEntityRepository>();
            var newItem = new SimpleDataEntity { Name = "Delta Test" };
            var actual = repository.Add(newItem);
            var result = repository.Save();

            var delta1 = new Delta<SimpleDataEntity>();
            delta1.SetValue("Name", 1);
        }

        [TestCategory("StandardRepository")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void DeltaTestInvalidFieldName1()
        {
            var repository = LocalIoCContainer.Resolve<ISimpleDataEntityRepository>();
            var newItem = new SimpleDataEntity { Name = "Delta Test" };
            var actual = repository.Add(newItem);
            var result = repository.Save();

            var delta1 = new Delta<SimpleDataEntity>();
            delta1.SetValue("NameNotRight", "Right data type");
        }

        [TestCategory("StandardRepository")]
        [TestMethod]
        public void DeltaTestWithPredicate()
        {
            var repository = LocalIoCContainer.Resolve<ISimpleDataEntityRepository>();
            var newItem = new SimpleDataEntity { Name = "Delta Test" };
            var actual = repository.Add(newItem);
            var result = repository.Save();

            var delta1 = new Delta<SimpleDataEntity>();
            delta1.SetValue("Name", "Delta Change Test");

            repository.Update(delta1, x=>x.Id == newItem.Id);

            repository.Save();

            var updatedValue = repository.Find(actual.Id);
            Assert.AreEqual("Delta Change Test", updatedValue.Name);
        }
    }
}
