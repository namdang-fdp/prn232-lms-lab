using AutoMapper;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Services.Models;

namespace PRN232.LMS.Services.Mapping;

public class RepositoryMappingProfile : Profile
{
    public RepositoryMappingProfile()
    {
        CreateMap<Semester, SemesterSummaryModel>();
        CreateMap<Semester, SemesterModel>();
        CreateMap<SemesterModel, Semester>()
            .ForMember(destination => destination.Courses, options => options.Ignore());

        CreateMap<Subject, SubjectSummaryModel>();
        CreateMap<Subject, SubjectModel>();
        CreateMap<SubjectModel, Subject>()
            .ForMember(destination => destination.Courses, options => options.Ignore());

        CreateMap<Course, CourseSummaryModel>();
        CreateMap<Course, CourseModel>();
        CreateMap<CourseModel, Course>()
            .ForMember(destination => destination.Semester, options => options.Ignore())
            .ForMember(destination => destination.Subject, options => options.Ignore())
            .ForMember(destination => destination.Enrollments, options => options.Ignore());

        CreateMap<Student, StudentSummaryModel>();
        CreateMap<Student, StudentModel>();
        CreateMap<StudentModel, Student>()
            .ForMember(destination => destination.Enrollments, options => options.Ignore());

        CreateMap<Enrollment, EnrollmentSummaryModel>();
        CreateMap<Enrollment, EnrollmentModel>();
        CreateMap<EnrollmentModel, Enrollment>()
            .ForMember(destination => destination.Student, options => options.Ignore())
            .ForMember(destination => destination.Course, options => options.Ignore());
    }
}
