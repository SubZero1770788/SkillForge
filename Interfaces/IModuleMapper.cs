using quiz_project.Entities;
using quiz_project.ViewModels;

namespace quiz_project.Interfaces
{
    public interface IModuleMapper
    {
        Module ToEntity(ModuleViewModel viewModel);
        ModuleViewModel ToViewModel(Module module);
    }
}
