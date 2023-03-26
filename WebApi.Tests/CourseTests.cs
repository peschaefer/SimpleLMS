namespace WebApi.Tests;
using WebApi.Models;
using WebApi.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

public class CourseTests
{
    private readonly DbContextOptions<LMSContext> _options;

    public CourseTests()
    {
        _options = new DbContextOptionsBuilder<LMSContext>()
            .UseInMemoryDatabase(databaseName: "TestCourseDatabase")
            .Options;
    }

    [Fact]
    public async Task PostCourse()
    {
        // Arrange
        using (var context = new LMSContext(_options))
        {
            var controller = new CoursesController(context);

            var course = new Course { Name = "Test Course" };

            // Act
            var result = await controller.PostCourse(course);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var createdCourse = Assert.IsType<Course>(createdAtActionResult.Value);
            Assert.Equal("Test Course", createdCourse.Name);
        }
    }

    [Fact]
    public async Task DeleteCourse()
    {
        var options = new DbContextOptionsBuilder<LMSContext>()
            .UseInMemoryDatabase(databaseName: "DeleteCourseDatabase")
            .Options;

        var course = new Course { Id = 1, Name = "Test Course" };
        using (var context = new LMSContext(options))
        {
            context.Courses.Add(course);
            await context.SaveChangesAsync();
        }

        using (var context = new LMSContext(options))
        {
            var controller = new CoursesController(context);
            var result = await controller.DeleteCourse(course.Id);

            Assert.IsType<NoContentResult>(result);

            Assert.Null(await context.Courses.FindAsync(course.Id));
        }
    }

    [Fact]
    public async Task GetCourses()
    {
        var options = new DbContextOptionsBuilder<LMSContext>()
            .UseInMemoryDatabase(databaseName: "GetCourses")
            .Options;

        var course = new Course { Id = 1, Name = "Test Course" };
        var course2 = new Course { Id = 2, Name = "Test Course 2" };
        using (var context = new LMSContext(options))
        {
            context.Courses.Add(course);
            context.Courses.Add(course2);
            await context.SaveChangesAsync();
        }

        using (var context = new LMSContext(options))
        {
            var controller = new CoursesController(context);
            var result = await controller.GetCourses();

            Assert.IsType<ActionResult<IEnumerable<Course>>>(result);
            var courses = result.Value.ToList();
            Assert.NotNull(courses);
            Assert.Equal(2, courses.Count);
            Assert.Equal(courses[0].Name, "Test Course");
            Assert.Equal(courses[1].Name, "Test Course 2");
        }
    }

    [Fact]
    public async Task GetCourse()
    {
        var options = new DbContextOptionsBuilder<LMSContext>()
            .UseInMemoryDatabase(databaseName: "GetCourse")
            .Options;

        var course = new Course { Id = 1, Name = "Test Course" };
        using (var context = new LMSContext(options))
        {
            context.Courses.Add(course);
            await context.SaveChangesAsync();
        }

        using (var context = new LMSContext(options))
        {
            var controller = new CoursesController(context);
            var result = await controller.GetCourse(1);

            Assert.IsType<ActionResult<Course>>(result);
            Assert.NotNull(result);
            Assert.Equal(result.Value.Name, "Test Course");
        }
    }

    [Fact]
    public async Task GetModulesInCourse()
    {
        var options = new DbContextOptionsBuilder<LMSContext>()
            .UseInMemoryDatabase(databaseName: "GetModulesInCourse")
            .Options;

        var course = new Course { Id = 1, Name = "Test Course" };
        using (var context = new LMSContext(options))
        {
            context.Courses.Add(course);
            context.Modules.Add(new Module {Id = 1, Name = "Test Module", CourseId = 1});
            context.Modules.Add(new Module {Id = 2, Name = "Test Module 2", CourseId = 1});
            await context.SaveChangesAsync();
        }

        using (var context = new LMSContext(options))
        {
            var controller = new CoursesController(context);
            var result = await controller.GetModulesInCourse(1);

            var modules = Assert.IsType<List<Module>>(result.Value);
            Assert.Equal(2, modules.Count);
            Assert.Equal(modules[0].Name, "Test Module");
            Assert.Equal(modules[1].Name, "Test Module 2");
        }
    }

    [Fact]
    public async Task PutCourse()
    {
        var options = new DbContextOptionsBuilder<LMSContext>()
            .UseInMemoryDatabase(databaseName: "PutDatabase")
            .Options;

        using (var context = new LMSContext(options))
        {
            var course = new Course { Id = 1, Name = "Test Course" };
            context.Courses.Add(course);
            await context.SaveChangesAsync();
        }

        using (var context = new LMSContext(options))
        {
            var controller = new CoursesController(context);

            var updatedCourse = new Course { Id = 1, Name = "Updated Course" };

            var result = await controller.PutCourse(1, updatedCourse);

            Assert.IsType<NoContentResult>(result);
            var courseFromDb = await context.Courses.FindAsync(updatedCourse.Id);
            Assert.Equal(updatedCourse.Name, courseFromDb.Name);
        }
    }
}
