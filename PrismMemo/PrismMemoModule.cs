using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using PrismMemo.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrismMemo
{
    class PrismMemoModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion("ContentRegion1", typeof(Title));
            regionManager.RegisterViewWithRegion("ContentRegion2", typeof(Description));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<Title>();
            containerRegistry.RegisterForNavigation<Description>();
        }
    }
}
