using AutoMapper;
using PRN232.LMS.API.Models.Requests;
using PRN232.LMS.API.Models.Responses;
using PRN232.LMS.Services.Models;
using PRN232.LMS.Services.Models.Common;

namespace PRN232.LMS.API.Mapping;

public class ApiMappingProfile : Profile
{
    public ApiMappingProfile()
    {
        CreateMap<QueryRequest, QueryParametersModel>();

        CreateMap<CreateSemesterRequest, SemesterModel>();
        CreateMap<UpdateSemesterRequest, SemesterModel>();
        CreateMap<SemesterSummaryModel, SemesterSummaryResponse>();
        CreateMap<SemesterModel, SemesterResponse>();

        CreateMap<CreateSubjectRequest, SubjectModel>();
        CreateMap<UpdateSubjectRequest, SubjectModel>();
        CreateMap<SubjectSummaryModel, SubjectSummaryResponse>();
        CreateMap<SubjectModel, SubjectResponse>();

        CreateMap<CreateCourseRequest, CourseModel>();
        CreateMap<UpdateCourseRequest, CourseModel>();
        CreateMap<CourseSummaryModel, CourseSummaryResponse>();
        CreateMap<CourseModel, CourseResponse>();

        CreateMap<CreateStudentRequest, StudentModel>();
        CreateMap<UpdateStudentRequest, StudentModel>();
        CreateMap<StudentSummaryModel, StudentSummaryResponse>();
        CreateMap<StudentModel, StudentResponse>();

        CreateMap<CreateEnrollmentRequest, EnrollmentModel>();
        CreateMap<UpdateEnrollmentRequest, EnrollmentModel>();
        CreateMap<EnrollmentSummaryModel, EnrollmentSummaryResponse>();
        CreateMap<EnrollmentModel, EnrollmentResponse>();
    }
}
