# PRN232 LAB 2 Final Audit and Smoke Checklist

## 1. Executive Summary

Estimated LAB 2 completion: **95%**

Submission readiness: **Ready for LAB 2 demo/submission after runtime smoke testing with Docker Compose**.

Critical blockers: **None found at build/config level**.

Remaining work before demo is runtime verification, not implementation:

- Run the API and PostgreSQL with Docker Compose.
- Execute the curl smoke checklist below.
- Open Swagger UI and verify v1/v2 docs plus the Authorize button.
- Verify seeded `admin` and `user` accounts exist in the running database.

## 2. Requirement Checklist

| Area | Requirement | Status | Evidence | Notes / risk |
|---|---|---:|---|---|
| LAB 1 architecture | 3-layer architecture preserved | PASS | `PRN232.LMS.API`, `PRN232.LMS.Services`, `PRN232.LMS.Repositories` | No architecture refactor performed during final audit. |
| LAB 1 architecture | Controllers thin | PASS | `PRN232.LMS.API/Controllers/*Controller.cs` | Controllers map DTOs, call services, wrap responses. Auth workflow remains in service layer. |
| LAB 1 architecture | Services contain business/security logic | PASS | `StudentService`, `CourseService`, `AuthService`, `JwtTokenService` | Login, refresh rotation, course existence checks, and business validation are in services. |
| LAB 1 architecture | Repositories contain data access only | PASS | `GenericRepository`, `QueryableExtensions`, `LmsQueryConfigurations` | Query mechanics remain repository-layer concerns. |
| LAB 1 architecture | DTO separation preserved | PASS | API request/response models, service business models, repository entities | Request/response DTOs stay in API project; business models stay in Services. |
| LAB 1 architecture | Entity models are not returned directly | PASS | `ApiMappingProfile`, controllers return response DTOs | API responses use `StudentResponse`, `CourseResponse`, `AuthTokenResponse`, etc. |
| LAB 1 architecture | Request/Response models not used in repository layer | PASS | `PRN232.LMS.Repositories` search found entities/query classes only | Repository layer has no dependency on API DTO namespaces. |
| REST | RESTful API design preserved | PASS | `api/v{version:apiVersion}/students`, `courses`, `subjects`, `semesters`, `enrollments`, `auth` | Resource-oriented routes remain; no action-style LMS routes added. |
| Docker | Docker deployment preserved | PASS | `docker-compose.yml`, `PRN232.LMS.API/Dockerfile` | `docker compose config` passed. |
| Swagger | OpenAPI preserved | PASS | `Program.cs`, `AuthorizeCheckOperationFilter` | v1/v2 docs configured; JWT bearer security added. Runtime UI should still be opened manually. |
| Response format | Consistent `ApiResponse` preserved | PASS | `ApiResponse<T>`, controllers, invalid model state factory, JWT events | Normal responses and validation/auth errors use wrapper. Middleware errors are JSON wrapper. |
| Query features | Search/sort/paging/fields/expand preserved | PASS | `QueryRequest`, `LmsQueryConfigurations`, `QueryableExtensions`, `ResponseShaper`, `PageResponse` | Runtime smoke should verify representative query behavior. |
| Content negotiation | JSON response support | PASS | `Program.cs` controllers + JSON options | Build verified. |
| Content negotiation | XML response support | PASS | `AddXmlSerializerFormatters()` | Runtime XML curl still needed. |
| Content negotiation | Unsupported `Accept` returns 406 | PASS | `options.ReturnHttpNotAcceptable = true` | Runtime 406 curl still needed. |
| Model binding | `[FromRoute]` | PASS | resource controllers | ID parameters explicitly bound. |
| Model binding | `[FromQuery]` | PASS | list actions and nested course students route | `QueryRequest` explicitly bound. |
| Model binding | `[FromBody]` | PASS | create/update/auth actions | Request payloads explicitly bound. |
| Model binding | `[FromHeader(Name = "X-Request-Id")]` | PASS | `StudentsController.CreateStudent` | Header is accepted and intentionally unused. |
| Validation | DataAnnotations | PASS | `PRN232.LMS.API/Models/Requests/*` | Required, length, range, email, regex attributes present. |
| Validation | FluentValidation | PASS | `CreateStudentRequestValidator`, `UpdateStudentRequestValidator`, `Program.cs` | Auto-validation registered. |
| Validation | Custom validation rule | PASS | `FptuStudentEmailValidator` | `fpt.edu.vn` local-part must match FPTU-style code such as `SE19886`. |
| Advanced routing | Attribute routing | PASS | controller route attributes | URL segment versioned attribute routes configured. |
| Advanced routing | Route constraints | PASS | `"{id:int}"`, `"{courseId:int}/students"` | Non-int IDs route-mismatch before action. |
| Advanced routing | Nested resources | PASS | `CoursesController.GetStudentsByCourse` | `GET /api/v1/courses/{courseId:int}/students`. |
| Advanced routing | Named routes | PASS | `GetStudentByIdV1`, `GetCourseByIdV1`, etc. | Create actions use `CreatedAtRoute`. |
| API versioning | `/api/v1` | PASS | `[ApiVersion(1.0)]`, `api/v{version:apiVersion}` | v1 resource and auth routes configured. |
| API versioning | `/api/v2` | PASS | `StudentsV2Controller` | Minimal `GET /api/v2/students` demo endpoint. |
| Middleware | Global exception handling | PASS | `GlobalExceptionMiddleware` | Handles `ApiException`, `ServiceException`, unexpected exceptions safely. |
| Middleware | Request logging | PASS | `RequestLoggingMiddleware` | Logs method, path, status code, elapsed ms; does not log body/secrets. |
| Auth schema | User table | PASS | `User` entity, `LmsDbContext`, `EnsureAuthSchema` | Table is created in lab-friendly init path. Runtime DB verification needed. |
| Auth schema | RefreshToken table | PASS | `RefreshToken` entity, `LmsDbContext`, `EnsureAuthSchema` | Stores token hashes, not raw tokens. Runtime DB verification needed. |
| Auth security | Password hashing | PASS | `PasswordHasherService`, `PasswordHasher<User>` registration | Demo users seeded with hashes, not plain text. |
| Auth security | JWT token generation | PASS | `JwtTokenService.GenerateAccessToken` | Includes user id/name/role/jti/iat claims. |
| Auth security | JWT bearer validation | PASS | `Program.cs AddJwtBearer` | Validates issuer, audience, signing key, lifetime. |
| Auth API | Login endpoint | PASS | `AuthController.Login` | `POST /api/v1/auth/login`. Runtime smoke needed. |
| Auth API | Refresh-token endpoint | PASS | `AuthController.RefreshToken` | `POST /api/v1/auth/refresh-token`; rotation implemented in `AuthService`. |
| Authorization | Protected APIs with `[Authorize]` | PASS | write actions in resource controllers | POST/PUT/DELETE require JWT; GET remains public for demo. |
| Authorization | Role-based endpoint | PASS | `AuthController.AdminCheck` | `GET /api/v1/auth/admin-check` uses `[Authorize(Roles = "Admin")]`. |
| Docker config | JWT env variables | PASS | `docker-compose.yml` | `Jwt__Issuer`, `Jwt__Audience`, `Jwt__Secret`, token lifetime values configured. |
| Swagger JWT | Authorize button and protected endpoint support | PASS | `AddSecurityDefinition`, `AuthorizeCheckOperationFilter` | Runtime Swagger UI visual check still needed. |
| Migration strategy | EF migrations | PARTIAL | `EnsureCreated`, `EnsureAuthSchema`, `dotnet ef` unavailable | Lab-friendly, non-destructive table init exists; production-grade migrations are still a follow-up. |

## 3. Build Verification

### `dotnet restore`

Command:

```bash
dotnet restore
```

Result: **PASS**

```text
Determining projects to restore...
All projects are up-to-date for restore.
```

### `dotnet build`

Command:

```bash
dotnet build --no-restore
```

Result: **PASS**

```text
Build succeeded.
0 Warning(s)
0 Error(s)
```

### `dotnet test`

Test project search:

```bash
rg --files -g '*Test*.csproj' -g '*Tests*.csproj'
```

Result: **SKIPPED**

No test project was found, so `dotnet test` was not run.

### `docker compose config`

Command:

```bash
docker compose config
```

Result: **PASS**

Compose rendered successfully and includes API, PostgreSQL, Redis, and JWT environment variables.

### Optional EF migration inspection

Command:

```bash
dotnet ef migrations list --no-build
```

Result: **NOT AVAILABLE**

`dotnet-ef` is not installed on PATH in this environment. This is not a build blocker. The project currently uses `EnsureCreated()` plus non-destructive auth table initialization for lab schema setup.

## 4. Route Inventory

| Route | Method | Auth | Evidence |
|---|---:|---|---|
| `/api/v1/students` | GET | Public | `StudentsController.GetStudents` |
| `/api/v1/students/{id}` | GET | Public | `StudentsController.GetStudent`, route name `GetStudentByIdV1` |
| `/api/v1/students` | POST | JWT required | `StudentsController.CreateStudent`, `[Authorize]` |
| `/api/v1/courses/{courseId}/students` | GET | Public | `CoursesController.GetStudentsByCourse` |
| `/api/v2/students` | GET | Public | `StudentsV2Controller.GetStudents` |
| `/api/v1/auth/login` | POST | Public | `AuthController.Login` |
| `/api/v1/auth/refresh-token` | POST | Public | `AuthController.RefreshToken` |
| `/api/v1/auth/admin-check` | GET | Admin JWT required | `AuthController.AdminCheck`, `[Authorize(Roles = "Admin")]` |

## 5. Manual Runtime Smoke Checklist

Run these after starting the app, for example with Docker Compose. Do not drop databases or delete volumes as part of smoke testing.

### A. JSON response

```bash
curl -i -H "Accept: application/json" \
  "http://localhost:8080/api/v1/students?page=1&size=3"
```

Expected: HTTP 200, JSON `ApiResponse/PageResponse` wrapper.

### B. XML response

```bash
curl -i -H "Accept: application/xml" \
  "http://localhost:8080/api/v1/students?page=1&size=3"
```

Expected: HTTP 200, XML response.

### C. 406 unsupported Accept

```bash
curl -i -H "Accept: text/csv" \
  "http://localhost:8080/api/v1/students?page=1&size=3"
```

Expected: HTTP 406 Not Acceptable.

### D. Validation attribute failure

```bash
curl -i -X POST "http://localhost:8080/api/v1/subjects" \
  -H "Content-Type: application/json" \
  -H "Accept: application/json" \
  -H "Authorization: Bearer PASTE_ACCESS_TOKEN_HERE" \
  -d '{"subjectCode":"LAB999","subjectName":"Authorization Smoke","credit":0}'
```

Expected: HTTP 400 with `ApiResponse` error wrapper and credit range validation error.

### E. FluentValidation/custom FPTU failure

```bash
curl -i -X POST "http://localhost:8080/api/v1/students" \
  -H "Content-Type: application/json" \
  -H "Accept: application/json" \
  -H "Authorization: Bearer PASTE_ACCESS_TOKEN_HERE" \
  -H "X-Request-Id: lab2-final-smoke" \
  -d '{"fullName":"Test Student","email":"abc@fpt.edu.vn","dateOfBirth":"2000-01-01"}'
```

Expected: HTTP 400 with `ApiResponse` error wrapper and FPTU student email local-part validation message.

### F. v1 list route

```bash
curl -i -H "Accept: application/json" \
  "http://localhost:8080/api/v1/students?page=1&size=3&search=student&sort=fullName&fields=studentId,fullName,email&expand=enrollments"
```

Expected: HTTP 200, search/sort/page/fields/expand behavior preserved.

### G. v2 list route

```bash
curl -i -H "Accept: application/json" \
  "http://localhost:8080/api/v2/students?page=1&size=3"
```

Expected: HTTP 200, v2-compatible `ApiResponse/PageResponse`.

### H. Nested course students route

```bash
curl -i -H "Accept: application/json" \
  "http://localhost:8080/api/v1/courses/1/students?page=1&size=5"
```

Expected: HTTP 200 if course 1 exists, `ApiResponse/PageResponse` with student response DTOs.

### I. Login admin

```bash
curl -i -X POST "http://localhost:8080/api/v1/auth/login" \
  -H "Content-Type: application/json" \
  -H "Accept: application/json" \
  -d '{"username":"admin","password":"123456"}'
```

Expected: HTTP 200, `accessToken`, `refreshToken`, `expiresIn`; no password hash or refresh token hash.

### J. Login user

```bash
curl -i -X POST "http://localhost:8080/api/v1/auth/login" \
  -H "Content-Type: application/json" \
  -H "Accept: application/json" \
  -d '{"username":"user","password":"123456"}'
```

Expected: HTTP 200, `accessToken`, `refreshToken`, `expiresIn`; no password hash or refresh token hash.

### K. Refresh token success

```bash
curl -i -X POST "http://localhost:8080/api/v1/auth/refresh-token" \
  -H "Content-Type: application/json" \
  -H "Accept: application/json" \
  -d '{"refreshToken":"PASTE_REFRESH_TOKEN_HERE"}'
```

Expected: HTTP 200, new `accessToken`, new `refreshToken`, `expiresIn`.

### L. Refresh token reuse should fail

```bash
curl -i -X POST "http://localhost:8080/api/v1/auth/refresh-token" \
  -H "Content-Type: application/json" \
  -H "Accept: application/json" \
  -d '{"refreshToken":"PASTE_OLD_REFRESH_TOKEN_HERE"}'
```

Expected: HTTP 401 with safe `ApiResponse` error wrapper.

### M. Protected write endpoint without token should return 401

```bash
curl -i -X POST "http://localhost:8080/api/v1/subjects" \
  -H "Content-Type: application/json" \
  -H "Accept: application/json" \
  -d '{"subjectCode":"LAB999","subjectName":"Authorization Smoke","credit":3}'
```

Expected: HTTP 401 Unauthorized with safe `ApiResponse` error wrapper.

### N. Protected write endpoint with valid token should reach action

```bash
curl -i -X POST "http://localhost:8080/api/v1/subjects" \
  -H "Content-Type: application/json" \
  -H "Accept: application/json" \
  -H "Authorization: Bearer PASTE_ACCESS_TOKEN_HERE" \
  -d '{"subjectCode":"LAB999","subjectName":"Authorization Smoke","credit":3}'
```

Expected: Request reaches the action. It may return 201 Created or a service-level duplicate/business validation response, but it must not return 401 for a valid token.

### O. Admin-check with user token should return 403

```bash
curl -i -H "Accept: application/json" \
  -H "Authorization: Bearer PASTE_USER_ACCESS_TOKEN_HERE" \
  "http://localhost:8080/api/v1/auth/admin-check"
```

Expected: HTTP 403 Forbidden with safe `ApiResponse` error wrapper.

### P. Admin-check with admin token should return 200

```bash
curl -i -H "Accept: application/json" \
  -H "Authorization: Bearer PASTE_ADMIN_ACCESS_TOKEN_HERE" \
  "http://localhost:8080/api/v1/auth/admin-check"
```

Expected: HTTP 200 with `ApiResponse` success message: `Admin authorization verified.`

### Q. Swagger opens and Authorize button exists

Open:

```text
http://localhost:8080/swagger
```

Expected:

- v1/v2 docs load.
- Authorize button is visible.
- Protected endpoints show JWT auth support.
- Bearer token can call protected endpoints.

## 6. Security Checklist

| Security item | Status | Evidence / notes |
|---|---:|---|
| No plain-text passwords stored | PASS | `PasswordHasherService`, seeded users hash `"123456"` before storage. |
| No password hash in auth response | PASS | `AuthTokenResponse` has token fields, username, role; no `PasswordHash`. |
| No refresh token hash in auth response | PASS | Raw refresh token returned once; `TokenHash` stays in repository entity. |
| Refresh token rotation | PASS | `AuthService.RefreshTokenAsync` revokes old token and creates a new one. |
| JWT secret via env | PASS | `docker-compose.yml` uses `Jwt__Secret`; `JwtOptions` bound from config. |
| No logging body/auth header/token/secrets | PASS | `RequestLoggingMiddleware` logs method/path/status/elapsed only. |
| 401/403 safe responses | PASS | `JwtBearerEvents.OnChallenge` and `OnForbidden` return safe `ApiResponse` JSON. |
| JWT validation | PASS | Issuer, audience, signing key, lifetime validation configured. |

## 7. Known Risks / Manual Checks Before Demo

- `docs/` is gitignored in `.gitignore`, so audit reports may not be committed unless the ignore rule is adjusted or files are force-added intentionally.
- Runtime Swagger v1/v2 should be opened manually.
- `docker compose up` smoke test is still required; this audit intentionally did not start containers.
- The project uses `EnsureCreated()` plus non-destructive auth table initialization. This is lab-friendly but not a production-grade migration strategy.
- Verify seeded users exist in the running DB:

```sql
SELECT COUNT(*) FROM "Users";
SELECT "Username", "Role", "PasswordHash" FROM "Users";
SELECT COUNT(*) FROM "RefreshTokens";
```

Expected:

- `admin` and `user` exist.
- `PasswordHash` does not equal `123456`.
- Roles include `Admin` and `User`.
- `RefreshTokens` table exists; count may be 0 before login.

- Verify JWT env values load in Docker:

```bash
docker compose config
```

Expected:

- API environment includes `Jwt__Issuer`, `Jwt__Audience`, `Jwt__Secret`, `Jwt__AccessTokenMinutes`, and `Jwt__RefreshTokenDays`.

## 8. Final Recommendation

The project is **ready for LAB 2 demo/submission after runtime smoke testing**.

Build verification passed, Docker Compose configuration passed, and the static audit found no critical blockers. The remaining confidence gap is runtime-only: start the stack, run the curl checklist, verify Swagger UI, and confirm seeded auth users in the running database.
