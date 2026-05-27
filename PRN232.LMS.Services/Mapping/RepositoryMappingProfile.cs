using AutoMapper;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Services.BusinessModels;

namespace PRN232.LMS.Services.Mapping;

public class RepositoryMappingProfile : Profile
{
    public RepositoryMappingProfile()
    {
        CreateMap<Semester, SemesterSummaryBusinessModel>();
        CreateMap<Semester, SemesterBusinessModel>();
        CreateMap<SemesterBusinessModel, Semester>()
            .ForMember(destination => destination.Courses, options => options.Ignore());

        CreateMap<Subject, SubjectSummaryBusinessModel>();
        CreateMap<Subject, SubjectBusinessModel>();
        CreateMap<SubjectBusinessModel, Subject>()
            .ForMember(destination => destination.Courses, options => options.Ignore());

        CreateMap<Course, CourseSummaryBusinessModel>();
        CreateMap<Course, CourseBusinessModel>();
        CreateMap<CourseBusinessModel, Course>()
            .ForMember(destination => destination.Semester, options => options.Ignore())
            .ForMember(destination => destination.Subject, options => options.Ignore())
            .ForMember(destination => destination.Enrollments, options => options.Ignore());

        CreateMap<Student, StudentSummaryBusinessModel>();
        CreateMap<Student, StudentBusinessModel>();
        CreateMap<StudentBusinessModel, Student>()
            .ForMember(destination => destination.Enrollments, options => options.Ignore());

        CreateMap<Enrollment, EnrollmentSummaryBusinessModel>();
        CreateMap<Enrollment, EnrollmentBusinessModel>();
        CreateMap<EnrollmentBusinessModel, Enrollment>()
            .ForMember(destination => destination.Student, options => options.Ignore())
            .ForMember(destination => destination.Course, options => options.Ignore());
    }
}
