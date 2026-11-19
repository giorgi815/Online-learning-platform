using Microsoft.AspNetCore.Identity;
using OLP.Application.Contracts.Persistence;
using OLP.Application.Contracts.Services;
using OLP.Application.DTOs.Admin;
using OLP.Application.Exceptions;
using OLP.Domain.Entities;

namespace OLP.Application.Services
{
    public class AdminService : IAdminService
    {
        private readonly IUserRepository _profiles;
        private readonly ICourseRepository _courses;
        private readonly IEnrollmentRepository _enrollments;
        private readonly ISubmissionRepository _submissions;
        private readonly UserManager<ApplicationUser> _users;
        private readonly RoleManager<IdentityRole<Guid>> _roles;

        public AdminService(IUserRepository userRepository,
            ICourseRepository courseRepository,
            IEnrollmentRepository enrollmentRepository,
            ISubmissionRepository submissionRepository,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole<Guid>> roleManager)
        {
            _profiles = userRepository;
            _courses = courseRepository;
            _enrollments = enrollmentRepository;
            _submissions = submissionRepository;
            _users = userManager;
            _roles = roleManager;
        }

        public async Task<PlatformStatisticsDto> GetStatisticsAsync(Guid adminAccountId)
        {
            await EnsureAdmin(adminAccountId);

            var (_, users) = await _profiles.GetAllAsync(tracikng: false);
            var (_, courses) = await _courses.GetAllAsync(tracikng: false);
            var (_, enrollments) = await _enrollments.GetAllAsync(tracikng: false);
            var (_, submissions) = await _submissions.GetAllAsync(tracikng: false);

            return new PlatformStatisticsDto
            {
                Users = users,
                Courses = courses,
                Enrollments = enrollments,
                Submissions = submissions
            };
        }

        public async Task GrantTeacherRoleAsync(Guid adminAccountId, Guid targetUserId)
        {
            await EnsureAdmin(adminAccountId);

            var profile = ServiceChecks.Found(await _profiles.GetAsync(u => u.Id == targetUserId), "User");
            var account = ServiceChecks.Found(await _users.FindByIdAsync(profile.ApplicationUserId.ToString()), "Account");

            if (!account.EmailConfirmed)
                throw new BadRequestException("Confirm email first.");

            if (!await _roles.RoleExistsAsync("Teacher"))
                throw new InvalidOperationException("Teacher role not seeded.");

            if (await _users.IsInRoleAsync(account, "Teacher")) 
                return;

            var result = await _users.AddToRoleAsync(account, "Teacher");

            if (!result.Succeeded)
                throw new BadRequestException(string.Join("; ", result.Errors.Select(e => e.Description)));
        }

        private async Task EnsureAdmin(Guid accountId)
        {
            var account = await _users.FindByIdAsync(accountId.ToString());

            if (account is null || !await _users.IsInRoleAsync(account, "Admin"))
                throw new ForbiddenException("Admin role required.");
        }
    }
}
