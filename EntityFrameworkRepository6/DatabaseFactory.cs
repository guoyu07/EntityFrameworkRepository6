﻿using System;
using System.Data.Entity;

namespace EntityFramework.Repository6
{
    public interface IDatabaseFactory<C>
    {
        C GetNewDbContext();
        Action<string> Logging { get; set; }
    }

    public class DatabaseFactory<C> : IDatabaseFactory<C>
        where C : DbContext
    {
        public Action<string> Logging { get; set; }

        protected string dbConnectionString { get; set; }

        public DatabaseFactory(string connectionString)
        {
            dbConnectionString = connectionString;
        }
        
        public virtual C GetNewDbContext()
        {
            var newContext = (C)Activator.CreateInstance(typeof(C), dbConnectionString);

            if (Logging != null) { newContext.Database.Log = Logging; }
            
            return newContext;
        }
    }
}
