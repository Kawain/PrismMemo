using PrismMemo.Views;
using Prism.Ioc;
using Prism.Modularity;
using System.Windows;
using Notes;
using Problems;
using Notes.Views;
using Prism.Mvvm;
using Notes.ViewModels;

namespace PrismMemo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            //モジュール追加
            moduleCatalog.AddModule<PrismMemoModule>();
            moduleCatalog.AddModule<NotesModule>();
            moduleCatalog.AddModule<ProblemsModule>();
        }

        
    }
}
