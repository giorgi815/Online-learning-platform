using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;
using OLP.Application.Contracts.Services;
using OLP.Application.Mapping;
using OLP.Application.Services;

namespace OLP.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(
            this IServiceCollection services)
        {
            // Mapster
            services.AddSingleton<TypeAdapterConfig>(
                MappingConfig.Create());

            services.AddScoped<IMapper, ServiceMapper>();

            // services
            services.AddScoped<ICourseCategoryService, CourseCategoryService>();
            services.AddScoped<ICourseService, CourseService>();
            services.AddScoped<ILessonService, LessonService>();
            services.AddScoped<IAssignmentService, AssignmentService>();
            services.AddScoped<IEnrollmentService, EnrollmentService>();
            services.AddScoped<ISubmissionService, SubmissionService>();
            services.AddScoped<ILessonProgressService, LessonProgressService>();
            services.AddScoped<IReviewService, ReviewService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<ICertificateService, CertificateService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ICacheService, RedisCacheService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IRefreshTokenStore, RedisRefreshTokenStore>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IFileStorageService, FileStorageService>();

            return services;
        }
    }
}
