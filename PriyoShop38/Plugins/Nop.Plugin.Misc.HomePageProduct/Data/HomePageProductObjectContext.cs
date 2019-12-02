using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using Nop.Core;
using System.Collections.Generic;
using System;
using Nop.Data;

namespace Nop.Plugin.Misc.HomePageProduct.Data
{
    public partial class HomePageProductObjectContext : DbContext, IDbContext 
    {
        #region Ctr

        public HomePageProductObjectContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
        }

        #endregion

        #region Entity

        public new IDbSet<TEntity> Set<TEntity>() where TEntity : BaseEntity
        {
            return base.Set<TEntity>();
        }

        #endregion

        #region Utility

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //Add references to the mapping files
            modelBuilder.Configurations.Add(new HomePageProductCategoryMap());
            modelBuilder.Configurations.Add(new HomePageProductCategoryImageMap());
            modelBuilder.Configurations.Add(new HomePageSubCategoryMap());
            modelBuilder.Configurations.Add(new HomePageCategoryMap());
          

            //disable EdmMetaDataGeneration
            modelBuilder.Conventions.Remove<IncludeMetadataConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            base.OnModelCreating(modelBuilder);
        }

        public string CreateDatabaseInstallationScript()
        {
            return ((IObjectContextAdapter) this).ObjectContext.CreateDatabaseScript();
        }

        public void InstallSchema()
        {
            Database.ExecuteSqlCommand(CreateDatabaseInstallationScript());
            SaveChanges();
        }
        /// <summary>
        /// Uninstall
        /// </summary>
        public void Uninstall()
        {
            //drop the table
            //var tableHomePageProductCategory = "HomePageProductCategory";
            //var tableHomePageProductCategoryImage = "HomePageProductCategoryImage";
            //var tableHomePageSubCategoryMap = "HomePageSubCategoryMap";
            //var tableHomePageCategory = "HomePageCategory";

            ////var tableName = "GoogleProduct";
            this.DropPluginTable("HomePageSubCategory");
            this.DropPluginTable("HomePageProductCategory");
            this.DropPluginTable("HomePageProductCategoryImage");

            this.DropPluginTable("HomePageCategory");
            //this.DropPluginTable(tableHomePageSubCategoryMap);
           
        }


        public IList<TEntity> ExecuteStoredProcedureList<TEntity>(string commandText, params object[] parameters) where TEntity : BaseEntity, new()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TElement> SqlQuery<TElement>(string sql, params object[] parameters)
        {
            throw new NotImplementedException();
        }

        public int ExecuteSqlCommand(string sql, bool doNotEnsureTransaction = false, int? timeout = null, params object[] parameters)
        {
            throw new NotImplementedException();
        }

        #endregion


        public void Detach(object entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            ((IObjectContextAdapter)this).ObjectContext.Detach(entity);
        }

        public bool ProxyCreationEnabled
        {
            get
            {
                return this.Configuration.ProxyCreationEnabled;
            }
            set
            {
                this.Configuration.ProxyCreationEnabled = value;
            }
        }

        public bool AutoDetectChangesEnabled
        {
            get
            {
                return this.Configuration.AutoDetectChangesEnabled;
            }
            set
            {
                this.Configuration.AutoDetectChangesEnabled = value;
            }
        }
    }
}
