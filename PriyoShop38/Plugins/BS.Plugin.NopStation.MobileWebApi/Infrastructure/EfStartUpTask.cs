using System.Data.Entity;
using BS.Plugin.NopStation.MobileWebApi.Data;
using Nop.Core.Infrastructure;


namespace BS.Plugin.NopStation.MobileWebApi.Infrastructure
{
	public class EfStartUpTask : IStartupTask
	{
		public void Execute()
		{
			//It's required to set initializer to null (for SQL Server Compact).
			//otherwise, you'll get something like "The model backing the 'your context name' context has changed since the database was created. Consider using Code First Migrations to update the database"
            Database.SetInitializer<MobileWebApiObjectContext>(null);
		}

		public int Order
		{
			//ensure that this task is run first 
			get
			{
				return 0;
			}
		}
	}
}