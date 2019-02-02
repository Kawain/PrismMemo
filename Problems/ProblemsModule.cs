using Problems.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace Problems
{
    public class ProblemsModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion("ContentRegion1", typeof(ProblemsMainView));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<ProblemsMainView>();
            containerRegistry.RegisterForNavigation<EditMenuView>();
            containerRegistry.RegisterForNavigation<EditFormView>();
            containerRegistry.RegisterForNavigation<ProblemsCategoryView>();
            containerRegistry.RegisterForNavigation<QaView>();
        }
    }
}