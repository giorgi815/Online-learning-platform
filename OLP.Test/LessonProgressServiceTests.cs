using System.Linq.Expressions;
using MapsterMapper;
using Moq;
using Xunit;
using OLP.Application.Contracts.Persistence;
using OLP.Application.Exceptions;
using OLP.Application.Services;
using OLP.Domain.Entities;

namespace OLP.Tests;

public class LessonProgressServiceTests
{
    private readonly Mock<ILessonProgressRepository> _progress = new();
    private readonly Mock<ILessonRepository> _lessons = new();
    private readonly Mock<IEnrollmentRepository> _enrollments = new();
    private readonly Mock<ICourseRepository> _courses = new();
    private readonly Mock<IMapper> _mapper = new();

    private LessonProgressService CreateService() =>
    new(
        _progress.Object,
        _lessons.Object,
        _courses.Object,
        _enrollments.Object,
        _mapper.Object
    );

    [Fact]
    public async Task IsLessonCompleted_ReturnsTrue_WhenCompleted()
    {
        var lessonId = Guid.NewGuid();
        var studentId = Guid.NewGuid();

        _progress.Setup(x => x.ExistsAsync(
                It.IsAny<Expression<Func<LessonProgress, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var service = CreateService();

        var result = await service.IsLessonCompletedAsync(
            lessonId, studentId);

        Assert.True(result);
    }

    [Fact]
    public async Task MarkLessonCompleted_Throws_WhenNotEnrolled()
    {
        var lessonId = Guid.NewGuid();
        var studentId = Guid.NewGuid();

        _lessons.Setup(x => x.GetAsync(
                It.IsAny<Expression<Func<Lesson, bool>>>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<Func<IQueryable<Lesson>, IQueryable<Lesson>>>()))
            .ReturnsAsync(new Lesson
            {
                Id = lessonId,
                CourseId = Guid.NewGuid()
            });

        _enrollments.Setup(x => x.ExistsAsync(
                It.IsAny<Expression<Func<Enrollment, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var service = CreateService();

        await Assert.ThrowsAsync<ForbiddenException>(
            () => service.MarkLessonCompletedAsync(
                lessonId, studentId));
    }
}
