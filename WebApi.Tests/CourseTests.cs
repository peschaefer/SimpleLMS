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



            var course = new Course { 
                Name = "Test Course",
                Modules = new List<Module>()
            };

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

        var course = new Course { Id = 1, Name = "Test Course", Modules = new List<Module>() };
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

        var course = new Course { Id = 1, Name = "Test Course", Modules = new List<Module>() };
        var course2 = new Course { Id = 2, Name = "Test Course 2", Modules = new List<Module>() };
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

        var course = new Course { Id = 1, Name = "Test Course", Modules = new List<Module>() };
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
    public async Task PutCourse()
    {
        var options = new DbContextOptionsBuilder<LMSContext>()
            .UseInMemoryDatabase(databaseName: "PutDatabase")
            .Options;

        using (var context = new LMSContext(options))
        {
            var course = new Course { Id = 1, Name = "Test Course", Modules = new List<Module>() };
            context.Courses.Add(course);
            await context.SaveChangesAsync();
        }

        using (var context = new LMSContext(options))
        {
            var controller = new CoursesController(context);

            var updatedCourse = new Course { Id = 1, Name = "Updated Course", Modules = new List<Module>() };

            var result = await controller.PutCourse(1, updatedCourse);

            Assert.IsType<NoContentResult>(result);
            var courseFromDb = await context.Courses.FindAsync(updatedCourse.Id);
            Assert.Equal(updatedCourse.Name, courseFromDb.Name);
        }
    }

    //Create a course with two modules, and read the course with its related modules, assert.
    [Fact]
    public async Task PostCourseWith2Modules()
    {
        // Arrange
        using (var context = new LMSContext(_options))
        {
            var controller = new CoursesController(context);

            var modules = new List<Module>();
            modules.Add(new Module
            {
                Id = 1,
                Name = "Module 1",
                Course = null,
                Assignments = new List<Assignment>()
            });
            modules.Add(new Module
            {
                Id = 2,
                Name = "Module 2",
                Course = null,
                Assignments = new List<Assignment>()
            });

            var course = new Course { 
                Name = "Test Course",
                Modules = modules
            };

            // Act
            var result = await controller.PostCourse(course);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var createdCourse = Assert.IsType<Course>(createdAtActionResult.Value);
            Assert.Equal("Test Course", createdCourse.Name);
            Assert.Equal(modules[0].Name, createdCourse.Modules[0].Name);
            Assert.Equal(modules[1].Name, createdCourse.Modules[1].Name);
        }
    }

    //Create three courses, and read all three courses, assert.
    [Fact]
    public async Task Create3Courses()
    {
        var options = new DbContextOptionsBuilder<LMSContext>()
            .UseInMemoryDatabase(databaseName: "Get3Courses")
            .Options;

        var course = new Course { Id = 1, Name = "Test Course 1", Modules = new List<Module>() };
        var course2 = new Course { Id = 2, Name = "Test Course 2", Modules = new List<Module>() };
        var course3 = new Course { Id = 3, Name = "Test Course 3", Modules = new List<Module>() };

        using (var context = new LMSContext(options))
        {
            var controller = new CoursesController(context);
            await controller.PostCourse(course);
            await controller.PostCourse(course2);
            await controller.PostCourse(course3);
            var result = await controller.GetCourses();

            Assert.IsType<ActionResult<IEnumerable<Course>>>(result);
            var courses = result.Value.ToList();
            Assert.NotNull(courses);
            Assert.Equal(3, courses.Count);
            Assert.Equal(courses[0].Name, "Test Course 1");
            Assert.Equal(courses[1].Name, "Test Course 2");
            Assert.Equal(courses[2].Name, "Test Course 3");
        }
    }
}
