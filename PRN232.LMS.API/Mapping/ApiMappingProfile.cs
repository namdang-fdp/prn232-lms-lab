using AutoMapper;
using PRN232.LMS.API.Models.Requests;
using PRN232.LMS.API.Models.Responses;
using PRN232.LMS.Services.BusinessModels;
using PRN232.LMS.Services.BusinessModels.Common;

namespace PRN232.LMS.API.Mapping;

public class ApiMappingProfile : Profile
{
    public ApiMappingProfile()
    {
        CreateMap<QueryRequest, QueryParametersBusinessModel>();

        CreateMap<CreateSemesterRequest, SemesterBusinessModel>();
        CreateMap<UpdateSemesterRequest, SemesterBusinessModel>();
        CreateMap<SemesterSummaryBusinessModel, SemesterSummaryResponse>();
        CreateMap<SemesterBusinessModel, SemesterResponse>();

        CreateMap<CreateSubjectRequest, SubjectBusinessModel>();
        CreateMap<UpdateSubjectRequest, SubjectBusinessModel>();
        CreateMap<SubjectSummaryBusinessModel, SubjectSummaryResponse>();
        CreateMap<SubjectBusinessModel, SubjectResponse>();

        CreateMap<CreateCourseRequest, CourseBusinessModel>();
        CreateMap<UpdateCourseRequest, CourseBusinessModel>();
        CreateMap<CourseSummaryBusinessModel, CourseSummaryResponse>();
        CreateMap<CourseBusinessModel, CourseResponse>();

        CreateMap<CreateStudentRequest, StudentBusinessModel>();
        CreateMap<UpdateStudentRequest, StudentBusinessModel>();
        CreateMap<StudentSummaryBusinessModel, StudentSummaryResponse>();
        CreateMap<StudentBusinessModel, StudentResponse>();

        CreateMap<CreateEnrollmentRequest, EnrollmentBusinessModel>();
        CreateMap<UpdateEnrollmentRequest, EnrollmentBusinessModel>();
        CreateMap<EnrollmentSummaryBusinessModel, EnrollmentSummaryResponse>();
        CreateMap<EnrollmentBusinessModel, EnrollmentResponse>();
    }
}
