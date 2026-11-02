using Mapster;
using OLP.Domain.Entities;
using OLP.Application.DTOs.Course;
using OLP.Application.DTOs.Lesson;
using OLP.Application.DTOs.Assignment;
using OLP.Application.DTOs.Submission;
using OLP.Application.DTOs.Enrollment;
using OLP.Application.DTOs.Review;
using OLP.Application.DTOs.LessonProgress;
using OLP.Application.DTOs.CourseCategory;
using OLP.Application.DTOs.Notification;
using OLP.Application.DTOs.User;

namespace OLP.Application.Mapping
{
    public class MappingConfig
    {
        public static TypeAdapterConfig Create()
        {
            var config = new TypeAdapterConfig();
            config.NewConfig<CreateCourseDto, Course>();
            config.NewConfig<UpdateCourseDto, Course>();
            config.NewConfig<Course, CourseResponseDto>()
                .Map(d => d.TeacherName, s => s.Teacher.FirstName + " " + s.Teacher.LastName)
                .Map(d => d.CategoryName, s => s.Category.Name);

            config.NewConfig<CreateLessonDto, Lesson>();
            config.NewConfig<UpdateLessonDto, Lesson>();
            config.NewConfig<Lesson, LessonResponseDto>();

            config.NewConfig<CreateAssignmentDto, Assignment>();
            config.NewConfig<UpdateAssignmentDto, Assignment>();
            config.NewConfig<Assignment, AssignmentResponseDto>();

            config.NewConfig<CreateSubmissionDto, Submission>();
            config.NewConfig<UpdateSubmissionDto, Submission>();
            config.NewConfig<Submission, SubmissionResponseDto>();

            config.NewConfig<Enrollment, EnrollmentResponseDto>()
                .Map(d => d.CourseTitle, s => s.Course.Title);

            config.NewConfig<CreateReviewDto, Review>();
            config.NewConfig<UpdateReviewDto, Review>();
            config.NewConfig<Review, ReviewResponseDto>()
                .Map(d => d.StudentName, s => s.Student.FirstName + " " + s.Student.LastName);

            config.NewConfig<LessonProgress, LessonProgressResponseDto>();

            config.NewConfig<CreateCourseCategoryDto, CourseCategory>();
            config.NewConfig<UpdateCourseCategoryDto, CourseCategory>();
            config.NewConfig<CourseCategory, CourseCategoryResponseDto>();

            config.NewConfig<CreateNotificationDto, Notification>();
            config.NewConfig<Notification, NotificationResponseDto>();

            config.NewConfig<UpdateUserDto, User>();
            config.NewConfig<User, UserResponseDto>();
            return config;
        }
    }
}
