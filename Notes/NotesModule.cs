using Notes.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace Notes
{
    public class NotesModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion("ContentRegion1", typeof(NotesMainView));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<NotesMainView>();
            containerRegistry.RegisterForNavigation<CategoryView>();
            containerRegistry.RegisterForNavigation<FormView>();
        }
    }
}