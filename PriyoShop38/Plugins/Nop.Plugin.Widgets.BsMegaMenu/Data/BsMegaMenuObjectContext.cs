using Nop.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;

namespace Nop.Plugin.Widgets.BsMegaMenu.Data
{
    public class BsMegaMenuObjectContext : DbContext, IDbContext
    {
        #region Constructor
        public BsMegaMenuObjectContext(string nameOrConnectionString) : base(nameOrConnectionString) { }
        #endregion

        #region Utilities
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new BsMegaMenuMap());
            base.OnModelCreating(modelBuilder);
        }
        #endregion

        #region Methods
        public new IDbSet<TEntity> Set<TEntity>() where TEntity : Core.BaseEntity
        {
            return base.Set<TEntity>();
        }
        public string CreateDatabaseInstallationScript()
        {
            return ((IObjectContextAdapter)this).ObjectContext.CreateDatabaseScript();
        }
        public void Install()
        {
            Database.SetInitializer<BsMegaMenuObjectContext>(null);
            Database.ExecuteSqlCommand(CreateDatabaseInstallationScript());
            SaveChanges();
        }

        public void Uninstall()
        {
            this.DropPluginTable("BsMegaMenu_ItemOrganizer");
        }

        public IList<TEntity> ExecuteStoredProcedureList<TEntity>(string commandText, params object[] parameters) where TEntity : BaseEntity, new()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TElement> SqlQuery<TElement>(string sql, params object[] parameters)
        {
            throw new NotImplementedException();
        }

        public int ExecuteSqlCommand(string sql, bool doNotEnsureTransaction = false, int? timeout = default(int?), params object[] parameters)
        {
            throw new NotImplementedException();
        }

        public void Detach(object entity)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Properties
        public bool ProxyCreationEnabled
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public bool AutoDetectChangesEnabled
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        } 
        #endregion

    }
}
