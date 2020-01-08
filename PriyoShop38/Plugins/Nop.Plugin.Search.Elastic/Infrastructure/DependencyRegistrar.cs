using Autofac;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Plugin.Search.Elastic.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Search.Elastic.Infrastructure
{
   public class DependencyRegistrar : IDependencyRegistrar
	{
		public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
		{
			//const string CONTEXT = "nop_object_context_sohel_elastic_search";


          builder.RegisterType<ElasticSearchService>().As<IElasticSearchService>().InstancePerLifetimeScope();

           
            
        }

		public int Order
		{
			get { return 100; }
		}
	}
}
