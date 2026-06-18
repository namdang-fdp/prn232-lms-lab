# XML Serialization Fix Report

## 1. Actual root cause

`Accept: application/xml` still returned `406 Not Acceptable` after switching to `AddXmlDataContractSerializerFormatters()` because controller metadata still constrained successful responses to JSON:

```csharp
[Produces("application/json")]
```

MVC therefore tried to write the successful response using the explicit content types from action metadata instead of negotiating XML from the request `Accept` header. The API log showed this as:

```text
No output formatter was found for content types 'application/json' to write the response.
```

After allowing XML in `[Produces]`, the next runtime issue was `DataContractSerializer` known-type resolution. Paged list actions return `ApiResponse<PageResponse<object>>`; the `object` collection contains concrete response DTOs such as `StudentResponse`, and field-shaped responses contain `Dictionary<string, object>`. `DataContractSerializer` requires these concrete runtime types to be declared.

## 2. Files changed

- `PRN232.LMS.API/Controllers/CoursesController.cs`
- `PRN232.LMS.API/Controllers/EnrollmentsController.cs`
- `PRN232.LMS.API/Controllers/SemestersController.cs`
- `PRN232.LMS.API/Controllers/StudentsController.cs`
- `PRN232.LMS.API/Controllers/SubjectsController.cs`
- `PRN232.LMS.API/Common/Response/PageResponse.cs`
- `docs/audit/XML_SERIALIZATION_FIX_REPORT.md`

## 3. Exact fix

Updated controller `[Produces]` metadata from JSON-only to:

```csharp
[Produces("application/json", "application/xml")]
```

Preserved `ProducesResponseType` attributes, Swagger documentation behavior, `ReturnHttpNotAcceptable = true`, JSON `ReferenceHandler.IgnoreCycles`, and `AddXmlDataContractSerializerFormatters()`.

Added `KnownType` declarations to `PageResponse<T>` for the concrete response DTOs and `Dictionary<string, object>` used by shaped paged responses. This preserves the JSON response shape while allowing XML serialization of successful paged responses.

## 4. docker compose config result

Command:

```bash
docker compose config
```

Result: Passed. Compose configuration rendered successfully.

## 5. docker compose up result

Command:

```bash
docker compose up -d --build api
```

Result: Passed. The `prn232-lms-api:1.0.0` image built successfully and `lms-api` was recreated and started.

## 6. curl results

Command:

```bash
curl -i -H "Accept: application/xml" "http://localhost:8080/api/v1/students?page=1&size=3"
```

Result: `HTTP/1.1 200 OK`, `Content-Type: application/xml; charset=utf-8`, XML body returned.

Command:

```bash
curl -i -H "Accept: application/json" "http://localhost:8080/api/v1/students?page=1&size=3"
```

Result: `HTTP/1.1 200 OK`, `Content-Type: application/json; charset=utf-8`, JSON body returned.

Command:

```bash
curl -i -H "Accept: text/csv" "http://localhost:8080/api/v1/students?page=1&size=3"
```

Result: `HTTP/1.1 406 Not Acceptable`.

Additional shaped-response check:

```bash
curl -i -H "Accept: application/xml" "http://localhost:8080/api/v1/students?page=1&size=1&fields=studentId,fullName"
```

Result: `HTTP/1.1 200 OK`, `Content-Type: application/xml; charset=utf-8`, XML body returned.

## 7. Remaining risk

XML output is produced by `DataContractSerializer`, so element names and namespaces are DataContract-style rather than the JSON property names. JSON response shape is unchanged.
