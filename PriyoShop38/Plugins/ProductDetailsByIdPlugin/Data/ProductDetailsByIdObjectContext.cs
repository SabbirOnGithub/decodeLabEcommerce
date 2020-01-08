using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using Nop.Core;
using System.Collections.Generic;
using System;
using Nop.Data;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core.Metadata.Edm;
using System.Text.RegularExpressions;

namespace Nop.Plugin.Misc.ProductDetailsById.Data
{
    public partial class ProductDetailsByIdObjectContext : DbContext, IDbContext 
    {
        #region Ctr

        public ProductDetailsByIdObjectContext(string nameOrConnectionString)
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
            //modelBuilder.Configurations.Add(new LiveAnnouncementMap());

            //modelBuilder.Configurations.Add(new AnnouncementMap());    

            //disable EdmMetaDataGeneration
            //modelBuilder.Conventions.Remove<IncludeMetadataConvention>();
            //modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            //base.OnModelCreating(modelBuilder);
        }

        public string CreateDatabaseInstallationScript()
        {
            return ((IObjectContextAdapter) this).ObjectContext.CreateDatabaseScript();
        }

        public void InstallSchema()
        {
            //Database.ExecuteSqlCommand(CreateDatabaseInstallationScript());
            //SaveChanges();
        }
        /// <summary>
        /// Uninstall
        /// </summary>
        public void Uninstall()
        {
            //drop the table

            //this.DropPluginTable("Announcement");
           
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
