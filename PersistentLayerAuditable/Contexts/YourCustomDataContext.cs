﻿using PersistentLayerAuditable.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using System.Data.Entity.ModelConfiguration.Conventions;
using PersistentLayerAuditable.Initializers;
using EntityFrameworkAuditableRepository6.Base;

namespace PersistentLayerAuditable.Contexts
{
    public class YourCustomDataContext : BaseContext<YourCustomDataContext>
    {
        public DbSet<SimpleDataEntity> SimpleDataEntities { get; set; }
        public DbSet<SimpleCompositeKeyEntity> SimpleCompositeKeyEntities { get; set; }
        public DbSet<SimpleDataEntityAudit> SimpleDataEntityAudits { get; set; }


        public YourCustomDataContext(string connectionString) : base(connectionString) { }
        public YourCustomDataContext(DbConnection dbConnection) : base(dbConnection) { }

        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        //}
    }
}