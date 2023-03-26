namespace WebApi.Tests;
using WebApi.Models;
using WebApi.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

public class AssignmentTests
{
    private readonly DbContextOptions<LMSContext> _options;

    public AssignmentTests()
    {
        _options = new DbContextOptionsBuilder<LMSContext>()
            .UseInMemoryDatabase(databaseName: "TestAssignmentDatabase")
            .Options;
    }

    [Fact]
    public async Task PostAssignment()
    {
        var options = new DbContextOptionsBuilder<LMSContext>()
            .UseInMemoryDatabase(databaseName: "PostAssignmentTest")
            .Options;

        using (var context = new LMSContext(options))
        {
            var controller = new AssignmentsController(context);

            var module = new Module
            {
                Id = 1,
                Name = "Test Module",
                CourseId = 1
            };

            context.Modules.Add(module);
            await context.SaveChangesAsync();

            var assignment = new Assignment
            {
                Id = 1,
                Name = "Test Assignment",
                Grade = 100,
                DueDate = new DateTime(),
                ModuleId = 1
            };

            var result = await controller.PostAssignment(assignment);

            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var createdAssignment = Assert.IsType<Assignment>(createdAtActionResult.Value);

            var retrievedAssignment = await context.Assignments.FindAsync(createdAssignment.Id);
            Assert.NotNull(retrievedAssignment);
            Assert.Equal(assignment.Name, retrievedAssignment.Name);
            Assert.Equal(assignment.ModuleId, retrievedAssignment.ModuleId);
        }
    }

    [Fact]
    public async Task DeleteAssignment()
    {
        var options = new DbContextOptionsBuilder<LMSContext>()
            .UseInMemoryDatabase(databaseName: "DeleteAssignmentTest")
            .Options;

        var assignment = new Assignment
        {
            Id = 1,
            Name = "Test Assignment",
            Grade = 100,
            DueDate = new DateTime(),
            ModuleId = 1
        };

        using (var context = new LMSContext(options))
        {
            context.Assignments.Add(assignment);
            await context.SaveChangesAsync();
        }

        using (var context = new LMSContext(options))
        {
            var controller = new AssignmentsController(context);
            var result = await controller.DeleteAssignment(assignment.Id);

            Assert.IsType<NoContentResult>(result);

            Assert.Null(await context.Assignments.FindAsync(assignment.Id));
        }
    }

    [Fact]
    public async Task GetAssignments()
    {
        var options = new DbContextOptionsBuilder<LMSContext>()
            .UseInMemoryDatabase(databaseName: "GetAssignments")
            .Options;

        var assignment = new Assignment
        {
            Id = 1,
            Name = "Test Assignment",
            Grade = 100,
            DueDate = new DateTime(),
            ModuleId = 1
        };
        var assignment2 = new Assignment
        {
            Id = 2,
            Name = "Test Assignment 2",
            Grade = 100,
            DueDate = new DateTime(),
            ModuleId = 1
        };
        using (var context = new LMSContext(options))
        {
            context.Assignments.Add(assignment);
            context.Assignments.Add(assignment2);
            await context.SaveChangesAsync();
        }

        using (var context = new LMSContext(options))
        {
            var controller = new AssignmentsController(context);
            var result = await controller.GetAssignments();

            Assert.IsType<ActionResult<IEnumerable<Assignment>>>(result);
            var assignments = result.Value.ToList();
            Assert.NotNull(assignments);
            Assert.Equal(2, assignments.Count);
            Assert.Equal(assignments[0].Name, "Test Assignment");
            Assert.Equal(assignments[1].Name, "Test Assignment 2");
        }
    }

    [Fact]
    public async Task GetAssignment()
    {
        var options = new DbContextOptionsBuilder<LMSContext>()
            .UseInMemoryDatabase(databaseName: "GetAssignment")
            .Options;

        var assignment = new Assignment
        {
            Id = 1,
            Name = "Test Assignment",
            Grade = 100,
            DueDate = new DateTime(),
            ModuleId = 1
        };
        using (var context = new LMSContext(options))
        {
            context.Assignments.Add(assignment);
            await context.SaveChangesAsync();
        }

        using (var context = new LMSContext(options))
        {
            var controller = new AssignmentsController(context);
            var result = await controller.GetAssignment(1);

            Assert.IsType<ActionResult<Assignment>>(result);
            Assert.NotNull(result);
            Assert.Equal(result.Value.Name, "Test Assignment");
        }
    }

    [Fact]
    public async Task PutAssignment()
    {
        var options = new DbContextOptionsBuilder<LMSContext>()
            .UseInMemoryDatabase(databaseName: "PutAssignment")
            .Options;

        using (var context = new LMSContext(options))
        {
            var module = new Module
            {
                Id = 1,
                Name = "Test Module",
                CourseId = 1
            };
            var assignment = new Assignment
            {
                Id = 1,
                Name = "Test Assignment",
                Grade = 100,
                DueDate = new DateTime(),
                ModuleId = 1
            };
            context.Modules.Add(module);
            context.Assignments.Add(assignment);

            await context.SaveChangesAsync();
        }

        using (var context = new LMSContext(options))
        {
            var controller = new AssignmentsController(context);

            var updatedAssignment = new Assignment
            {
                Id = 1,
                Name = "Updated Assignment",
                Grade = 100,
                DueDate = new DateTime(),
                ModuleId = 1
            };

            var result = await controller.PutAssignment(1, updatedAssignment);

            Assert.IsType<NoContentResult>(result);
            var courseFromDb = await context.Assignments.FindAsync(updatedAssignment.Id);
            Assert.Equal(updatedAssignment.Name, courseFromDb.Name);
        }
    }
}
