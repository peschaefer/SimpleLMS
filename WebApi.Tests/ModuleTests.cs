namespace WebApi.Tests;
using WebApi.Models;
using WebApi.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

public class ModuleTests
{
    private readonly DbContextOptions<LMSContext> _options;

    public ModuleTests()
    {
        _options = new DbContextOptionsBuilder<LMSContext>()
            .UseInMemoryDatabase(databaseName: "TestModuleDatabase")
            .Options;
    }

    [Fact]
    public async Task PostModule()
    {
        var options = new DbContextOptionsBuilder<LMSContext>()
            .UseInMemoryDatabase(databaseName: "PostModuleTest")
            .Options;

        using (var context = new LMSContext(options))
        {
            var controller = new ModulesController(context);

            var module = new Module
            {
                Id = 1,
                Name = "Test Module",
                Course = null,
                Assignments = new List<Assignment>()
            };

            var result = await controller.PostModule(module);

            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var createdModule = Assert.IsType<Module>(createdAtActionResult.Value);

            var retrievedModule = await context.Modules.FindAsync(createdModule.Id);
            Assert.NotNull(retrievedModule);
            Assert.Equal(module.Name, retrievedModule.Name);
        }
    }

    [Fact]
    public async Task DeleteModule()
    {
        var options = new DbContextOptionsBuilder<LMSContext>()
            .UseInMemoryDatabase(databaseName: "DeleteModuleTest")
            .Options;

        var module = new Module
        {
            Id = 1,
            Name = "Test Module",
            Course = null,
            Assignments = new List<Assignment>()
        };

        using (var context = new LMSContext(options))
        {
            context.Modules.Add(module);
            await context.SaveChangesAsync();
        }

        using (var context = new LMSContext(options))
        {
            var controller = new ModulesController(context);
            var result = await controller.DeleteModule(module.Id);

            Assert.IsType<NoContentResult>(result);

            Assert.Null(await context.Modules.FindAsync(module.Id));
        }
    }

    [Fact]
    public async Task GetModules()
    {
        var options = new DbContextOptionsBuilder<LMSContext>()
            .UseInMemoryDatabase(databaseName: "GetModules")
            .Options;

        var module = new Module
        {
            Id = 1,
            Name = "Test Module",
            Course = null,
            Assignments = new List<Assignment>()
        };
        var module2 = new Module
        {
            Id = 2,
            Name = "Test Module 2",
            Course = null,
            Assignments = new List<Assignment>()
        };
        using (var context = new LMSContext(options))
        {
            context.Modules.Add(module);
            context.Modules.Add(module2);
            await context.SaveChangesAsync();
        }

        using (var context = new LMSContext(options))
        {
            var controller = new ModulesController(context);
            var result = await controller.GetModules();

            Assert.IsType<ActionResult<IEnumerable<Module>>>(result);
            var modules = result.Value.ToList();
            Assert.NotNull(modules);
            Assert.Equal(2, modules.Count);
            Assert.Equal(modules[0].Name, "Test Module");
            Assert.Equal(modules[1].Name, "Test Module 2");
        }
    }

    [Fact]
    public async Task GetModule()
    {
        var options = new DbContextOptionsBuilder<LMSContext>()
            .UseInMemoryDatabase(databaseName: "GetModule")
            .Options;

        var course = new Course { Id = 1, Name = "Test Course" };
        var module = new Module
        {
            Id = 1,
            Name = "Test Module",
            Course = null,
            Assignments = new List<Assignment>()
        };
        using (var context = new LMSContext(options))
        {
            context.Courses.Add(course);
            context.Modules.Add(module);
            await context.SaveChangesAsync();
        }

        using (var context = new LMSContext(options))
        {
            var controller = new ModulesController(context);
            var result = await controller.GetModule(1);

            Assert.IsType<ActionResult<Module>>(result);
            Assert.NotNull(result);
            Assert.Equal(result.Value.Name, "Test Module");
        }
    }

    [Fact]
    public async Task PutModule()
    {
        var options = new DbContextOptionsBuilder<LMSContext>()
            .UseInMemoryDatabase(databaseName: "PutModuleDatabase")
            .Options;

        using (var context = new LMSContext(options))
        {
            var module = new Module
            {
                Id = 1,
                Name = "Test Module",
                Course = null,
                Assignments = new List<Assignment>()
            };
            context.Modules.Add(module);
            
            await context.SaveChangesAsync();
        }

        using (var context = new LMSContext(options))
        {
            var controller = new ModulesController(context);

            var updatedModule = new Module
            {
                Id = 1,
                Name = "Updated Module",
                Course = null,
                Assignments = new List<Assignment>()
            };

            var result = await controller.PutModule(1, updatedModule);

            Assert.IsType<NoContentResult>(result);
            var courseFromDb = await context.Modules.FindAsync(updatedModule.Id);
            Assert.Equal(updatedModule.Name, courseFromDb.Name);
        }
    }

    //Create a module with 2 assignment, read
    [Fact]
    public async Task PostModuleWith2Assignments()
    {
        var options = new DbContextOptionsBuilder<LMSContext>()
            .UseInMemoryDatabase(databaseName: "PostModuleWith2Assignments")
            .Options;

        using (var context = new LMSContext(options))
        {
            var controller = new ModulesController(context);

            var assignments = new List<Assignment>();
            assignments.Add(new Assignment
            {
                Id = 1,
                Name = "Test Assignment",
                Grade = 100,
                DueDate = new DateTime(),
                Module = null
            });

            assignments.Add(new Assignment
            {
                Id = 2,
                Name = "Test Assignment 2",
                Grade = 100,
                DueDate = new DateTime(),
                Module = null
            });

            var module = new Module
            {
                Id = 1,
                Name = "Test Module",
                Course = null,
                Assignments = assignments
            };

            var result = await controller.PostModule(module);

            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var createdModule = Assert.IsType<Module>(createdAtActionResult.Value);

            var retrievedModule = await context.Modules.FindAsync(createdModule.Id);
            Assert.NotNull(retrievedModule);
            Assert.Equal(module.Name, retrievedModule.Name);
            Assert.Equal(assignments[0].Name, retrievedModule.Assignments[0].Name);
            Assert.Equal(assignments[1].Name, retrievedModule.Assignments[1].Name);
        }
    }
}
