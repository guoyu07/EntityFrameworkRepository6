﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using Microsoft.Practices.Unity;
using PersistentLayer.Repositories;
using System.Linq;
using PersistentLayer.Entities;
using System.Data.Entity.Infrastructure;
using EntityFramework.Repository6;
using System.Collections.Generic;
using Unity.Lifetime;
using Unity.Injection;
using Unity;

namespace EntityFramework.Repository6.Tests
{
    [TestClass]
    public class RepositoryCreateTests
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
        public void InsertTestMethod()
        {
            var repository = LocalIoCContainer.Resolve<ISimpleDataEntityRepository>();
            var newItem = new SimpleDataEntity { Name = "My Test" };
            var actual = repository.Add(newItem);
            var result = repository.Save();

            Assert.AreNotEqual(0, actual.Id);

            var actual2 = repository.Find(actual.Id);

            Assert.AreEqual("My Test", actual2.Name);

            repository.Dispose();
        }

        [TestCategory("StandardRepository")]
        [TestMethod]
        public void InsertTestMethod2()
        {
            var repository = LocalIoCContainer.Resolve<ISimpleDataEntityRepository>();
            var newItem = new SimpleDataEntity { Name = "My Test 200" };
            var actual = repository.Add(newItem);
            var result = repository.Save();

            Assert.AreNotEqual(0, actual.Id);

            var actual2 = repository.Find(actual.Id);

            Assert.AreEqual("My Test 200", actual2.Name);

            repository.Dispose();
        }

        [TestCategory("StandardRepository")]
        [TestMethod]
        public void InsertTestMethod3()
        {
            var repository = LocalIoCContainer.Resolve<ISimpleDataEntityRepository>();
            var newItem = new SimpleDataEntity { Name = "My Test 300" };
            var actual = repository.Add(newItem);
            var result = repository.Save();

            Assert.AreNotEqual(0, actual.Id);

            var actual2 = repository.Find(actual.Id);

            Assert.AreEqual("My Test 300", actual2.Name);

            repository.Dispose();
        }

        [TestCategory("StandardRepository")]
        [TestMethod]
        public void InsertMultipleTest()
        {
            var repository = LocalIoCContainer.Resolve<ISimpleDataEntityRepository>();

            var actual1 = repository.Count();

            var newItem1 = new SimpleDataEntity { Name = "Multiple Item 1" };
            var newItem2 = new SimpleDataEntity { Name = "Multiple Item 2" };
            var newItem3 = new SimpleDataEntity { Name = "Multiple Item 3" };
            var actual = repository.Add(new List<SimpleDataEntity>
            {
                newItem1
                , newItem2
                , newItem3
            });
            var result = repository.Save();

            var actual2 = repository.Count();

            Assert.AreEqual(7, actual1);
            Assert.AreEqual(10, actual2);

            repository.Dispose();
        }

        [TestCategory("StandardRepository")]
        [TestMethod]
        public void InsertIntoCompositeKeyTableMethod()
        {
            var repository = LocalIoCContainer.Resolve<ISimpleCompositeKeyEntityRepository>();
            var newItem = new SimpleCompositeKeyEntity { Id = 1, Name = "Composite Test" };
            var actual = repository.Add(newItem);
            var result = repository.Save();

            Assert.AreNotEqual(0, actual.Id);

            var actual2 = repository.Find(actual.Id, actual.Name);

            var saveAgain = repository.Add(new SimpleCompositeKeyEntity { Id = 2, Name = "Composite Test" });
            var result2 = repository.Save();

            Assert.AreNotEqual(actual.Id, saveAgain.Id);

            Assert.AreEqual("Composite Test", actual2.Name);

            repository.Dispose();
        }

        [TestCategory("StandardRepository")]
        [TestMethod]
        [ExpectedException(typeof(DbUpdateException))]
        public void InsertDuplicateIntoCompositeKeyTableMethod()
        {
            var repository = LocalIoCContainer.Resolve<ISimpleCompositeKeyEntityRepository>();
            var newItem = new SimpleCompositeKeyEntity { Id = 1, Name = "Composite Test" };
            var actual = repository.Add(newItem);
            var result = repository.Save();

            Assert.AreNotEqual(0, actual.Id);

            var actual2 = repository.Find(actual.Id, actual.Name);

            var saveAgain = repository.Add(new SimpleCompositeKeyEntity { Id = 1, Name = "Composite Test" });
            var result2 = repository.Save();
        }
    }
}
