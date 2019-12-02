using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Data.Entity.Infrastructure;

using BS.Plugin.NopStation.MobileWebApi.Data.Agent;

using Nop.Core;
using Nop.Data;
using Nop.Data.Initializers;

namespace BS.Plugin.NopStation.MobileWebApi.Data
{
    public class MobileWebApiObjectContext : DbContext, IDbContext
    {
        public MobileWebApiObjectContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
            //((IObjectContextAdapter) this).ObjectContext.ContextOptions.LazyLoadingEnabled = true;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //dynamically load all configuration
            //System.Type configType = typeof(LanguageMap);   //any of your configuration classes here
            //var typesToRegister = Assembly.GetAssembly(configType).GetTypes()

            //var typesToRegister = Assembly.GetExecutingAssembly().GetTypes()
            //.Where(type => !String.IsNullOrEmpty(type.Namespace))
            //.Where(type => type.BaseType != null && type.BaseType.IsGenericType &&
            //    type.BaseType.GetGenericTypeDefinition() == typeof(NopEntityTypeConfiguration<>));
            //foreach (var type in typesToRegister)
            //{
            //    dynamic configurationInstance = Activator.CreateInstance(type);
            //    modelBuilder.Configurations.Add(configurationInstance);
            //}
            //...or do it manually below. For example,
            //modelBuilder.Configurations.Add(new LanguageMap());

            //disable EdmMetadata generation
            //modelBuilder.Conventions.Remove<IncludeMetadataConvention>();

            // Mobile Web API Configuration
            modelBuilder.Configurations.Add(new DeviceMap());
            modelBuilder.Configurations.Add(new BS_FeaturedProductsMap());
            modelBuilder.Configurations.Add(new BS_ContentManagementTemplateMap());
            modelBuilder.Configurations.Add(new BS_ContentManagementMap());
            modelBuilder.Configurations.Add(new BS_CategoryIconMap());
            modelBuilder.Configurations.Add(new BS_SliderMap());
            modelBuilder.Configurations.Add(new BS_RecentlyViewedProductsApiMap());
            modelBuilder.Configurations.Add(new BS_HomePageCategoryMap());
            modelBuilder.Configurations.Add(new BS_HomePageCategoryProductMap());

            #region Agent
            modelBuilder.Configurations.Add(new AgentMasterInformationTempMap());
            modelBuilder.Configurations.Add(new AgentImageInformationTempMap());
            #endregion

            base.OnModelCreating(modelBuilder);
        }

        public string CreateDatabaseScript()
        {
            return ((IObjectContextAdapter)this).ObjectContext.CreateDatabaseScript();
        }

        /// <summary>
        /// Install
        /// </summary>
        public void Install()
        {
            //create the table
            //var dbScript = "SELECT * FROM INFORMATION_SCHEMA.TABLES  WHERE TABLE_SCHEMA = 'TheSchema' AND  TABLE_NAME = 'TheTable'";

            //var dbScript = CreateDatabaseScript();
            //try
            //{
            //    Database.ExecuteSqlCommand(dbScript);
            //}
            //catch { }

            //SaveChanges();

            var tablesToValidate = new[] { "BS_WebApi_Device", "BS_FeaturedProducts", "BS_ContentManagementTemplate", "BS_ContentManagement",
                "BS_CategoryIcons", "BS_Slider", "BS_RecentlyViewedProductsApi" };

            //custom commands (stored proedures, indexes)
            var customCommands = new List<string>();

            //Check if table is exists or not.
            var initializer = new CreateTablesIfNotExist<MobileWebApiObjectContext>(tablesToValidate, customCommands.ToArray());
            //Create table if not exists
            initializer.InitializeDatabase(this);
        }

        /// <summary>
        /// Uninstall
        /// </summary>
        public void Uninstall()
        {
            // this.DropPluginTable("Bs_WebApi_Device");

            //drop the table
            this.DropPluginTable("BS_WebApi_Device");
            this.DropPluginTable("BS_FeaturedProducts");
            this.DropPluginTable("BS_ContentManagement");
            this.DropPluginTable("BS_ContentManagementTemplate");
            this.DropPluginTable("BS_CategoryIcons");
            this.DropPluginTable("BS_Slider");
            this.DropPluginTable("BS_RecentlyViewedProductsApi");
            this.DropPluginTable("BS_HomePageCategory");
            this.DropPluginTable("BS_HomePageCategoryProduct");
        }

        public new IDbSet<TEntity> Set<TEntity>() where TEntity : BaseEntity
        {
            return base.Set<TEntity>();
        }

        public IList<TEntity> ExecuteStoredProcedureList<TEntity>(string commandText, params object[] parameters) where TEntity : BaseEntity, new()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TElement> SqlQuery<TElement>(string sql, params object[] parameters)
        {
            return Database.SqlQuery<TElement>(sql, parameters);
        }

        public int ExecuteSqlCommand(string sql, bool doNotEnsureTransaction = false, int? timeout = null, params object[] parameters)
        {
            return Database.ExecuteSqlCommand(doNotEnsureTransaction ? TransactionalBehavior.DoNotEnsureTransaction : TransactionalBehavior.EnsureTransaction, sql, parameters);
        }

        public void Detach(object entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            ((IObjectContextAdapter)this).ObjectContext.Detach(entity);
        }

        /// <summary>
        /// Gets or sets a value indicating whether proxy creation setting is enabled (used in EF)
        /// </summary>
        public virtual bool ProxyCreationEnabled
        {
            get
            {
                return Configuration.ProxyCreationEnabled;
            }
            set
            {
                Configuration.ProxyCreationEnabled = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether auto detect changes setting is enabled (used in EF)
        /// </summary>
        public virtual bool AutoDetectChangesEnabled
        {
            get
            {
                return Configuration.AutoDetectChangesEnabled;
            }
            set
            {
                Configuration.AutoDetectChangesEnabled = value;
            }
        }
    }
}
