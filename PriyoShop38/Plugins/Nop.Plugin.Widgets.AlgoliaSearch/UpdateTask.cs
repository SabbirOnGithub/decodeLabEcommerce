using Nop.Plugin.Widgets.AlgoliaSearch.Factories;
using Nop.Services.Tasks;

namespace Nop.Plugin.Widgets.AlgoliaSearch
{
    public class UpdateTask : ITask
    {
        private readonly IProductModelFactory _productModelFactory;

        public UpdateTask(IProductModelFactory productModelFactory)
        {
            this._productModelFactory = productModelFactory;
        }

        public void Execute()
        {
            _productModelFactory.UpdateAlgoliaModel();
        }
    }
}
