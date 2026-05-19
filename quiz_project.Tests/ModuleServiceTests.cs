using FluentAssertions;
using Moq;
using quiz_project.Entities;
using quiz_project.Interfaces;
using quiz_project.Services;
using quiz_project.ViewModels;

namespace quiz_project.Tests;

public class ModuleServiceTests
{
    private static Module MakeModule(int id = 1, int courseId = 10) =>
        new() { ModuleId = id, CourseId = courseId, Title = "Module", Description = "Desc" };

    private static ModuleViewModel MakeVm(int id = 0, int courseId = 10) =>
        new() { ModuleId = id, CourseId = courseId, Title = "Module", Description = "Desc" };

    private static ModuleService BuildService(
        Mock<IModuleRepository> repo,
        Mock<IModuleMapper>? mapper = null)
    {
        mapper ??= new Mock<IModuleMapper>();
        mapper.Setup(m => m.ToEntity(It.IsAny<ModuleViewModel>()))
              .Returns(MakeModule());
        mapper.Setup(m => m.ToViewModel(It.IsAny<Module>()))
              .Returns(MakeVm());
        return new ModuleService(repo.Object, mapper.Object);
    }

    // ── CreateAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task Create_CallsRepositoryCreateOnce()
    {
        var repo = new Mock<IModuleRepository>();
        repo.Setup(r => r.CreateModuleAsync(It.IsAny<Module>())).Returns(Task.CompletedTask);
        var svc = BuildService(repo);

        var (success, error) = await svc.CreateAsync(MakeVm());

        success.Should().BeTrue();
        error.Should().BeEmpty();
        repo.Verify(r => r.CreateModuleAsync(It.IsAny<Module>()), Times.Once);
    }

    // ── GetEditAsync ─────────────────────────────────────────────────────────

    [Fact]
    public async Task GetEdit_ExistingModule_ReturnsViewModel()
    {
        var repo = new Mock<IModuleRepository>();
        repo.Setup(r => r.GetModuleByIdAsync(1)).ReturnsAsync(MakeModule(1));
        var svc = BuildService(repo);

        var result = await svc.GetEditAsync(1);

        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetEdit_NonExistentModule_ReturnsNull()
    {
        var repo = new Mock<IModuleRepository>();
        repo.Setup(r => r.GetModuleByIdAsync(99)).ReturnsAsync((Module?)null);
        var svc = BuildService(repo);

        var result = await svc.GetEditAsync(99);

        result.Should().BeNull();
    }

    // ── PostEditAsync ────────────────────────────────────────────────────────

    [Fact]
    public async Task PostEdit_NonExistentModule_ReturnsError()
    {
        var repo = new Mock<IModuleRepository>();
        repo.Setup(r => r.GetModuleByIdAsync(99)).ReturnsAsync((Module?)null);
        var svc = BuildService(repo);

        var (success, errors) = await svc.PostEditAsync(MakeVm(id: 99));

        success.Should().BeFalse();
        errors.Should().ContainSingle().Which.Should().Contain("not found");
    }

    [Fact]
    public async Task PostEdit_ExistingModule_CallsUpdateAndSucceeds()
    {
        var repo = new Mock<IModuleRepository>();
        repo.Setup(r => r.GetModuleByIdAsync(1)).ReturnsAsync(MakeModule(1));
        repo.Setup(r => r.UpdateModuleAsync(It.IsAny<Module>())).Returns(Task.CompletedTask);
        var svc = BuildService(repo);

        var (success, errors) = await svc.PostEditAsync(MakeVm(id: 1));

        success.Should().BeTrue();
        errors.Should().BeEmpty();
        repo.Verify(r => r.UpdateModuleAsync(It.IsAny<Module>()), Times.Once);
    }

    // ── DeleteAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task Delete_NonExistentModule_DoesNotCallDeleteOnRepository()
    {
        var repo = new Mock<IModuleRepository>();
        repo.Setup(r => r.GetModuleByIdAsync(99)).ReturnsAsync((Module?)null);
        var svc = BuildService(repo);

        await svc.DeleteAsync(99);

        repo.Verify(r => r.DeleteModuleAsync(It.IsAny<Module>()), Times.Never);
    }

    [Fact]
    public async Task Delete_ExistingModule_CallsDeleteOnRepository()
    {
        var module = MakeModule(1);
        var repo = new Mock<IModuleRepository>();
        repo.Setup(r => r.GetModuleByIdAsync(1)).ReturnsAsync(module);
        repo.Setup(r => r.DeleteModuleAsync(module)).Returns(Task.CompletedTask);
        var svc = BuildService(repo);

        await svc.DeleteAsync(1);

        repo.Verify(r => r.DeleteModuleAsync(module), Times.Once);
    }

    // ── ReorderAsync ─────────────────────────────────────────────────────────

    [Fact]
    public async Task Reorder_UpdatesOrderForEachItem()
    {
        var m1 = MakeModule(1);
        var m2 = MakeModule(2);
        var repo = new Mock<IModuleRepository>();
        repo.Setup(r => r.GetModuleByIdAsync(1)).ReturnsAsync(m1);
        repo.Setup(r => r.GetModuleByIdAsync(2)).ReturnsAsync(m2);
        repo.Setup(r => r.UpdateModuleAsync(It.IsAny<Module>())).Returns(Task.CompletedTask);
        var svc = BuildService(repo);

        await svc.ReorderAsync(
        [
            new ReorderItem { Id = 1, Order = 3 },
            new ReorderItem { Id = 2, Order = 1 }
        ]);

        m1.Order.Should().Be(3);
        m2.Order.Should().Be(1);
        repo.Verify(r => r.UpdateModuleAsync(It.IsAny<Module>()), Times.Exactly(2));
    }

    [Fact]
    public async Task Reorder_SkipsMissingModules()
    {
        var repo = new Mock<IModuleRepository>();
        repo.Setup(r => r.GetModuleByIdAsync(99)).ReturnsAsync((Module?)null);
        var svc = BuildService(repo);

        await svc.ReorderAsync([new ReorderItem { Id = 99, Order = 5 }]);

        repo.Verify(r => r.UpdateModuleAsync(It.IsAny<Module>()), Times.Never);
    }
}
