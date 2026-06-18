# XML Serialization Fix Report

## 1. Root cause

`GET /api/v1/students?page=1&size=3` with `Accept: application/xml` returned `406 Not Acceptable` because MVC was configured with `AddXmlSerializerFormatters()`. The XML serializer formatter attempted to create an `XmlSerializer` for `ApiResponse<object>`, but `ApiResponse<T>.Errors` is a `Dictionary<string, string>?`. `XmlSerializer` cannot serialize types that implement `IDictionary`, so content negotiation could not select a working XML output formatter.

## 2. Files changed

- `PRN232.LMS.API/Program.cs`
- `docs/audit/XML_SERIALIZATION_FIX_REPORT.md`

## 3. Fix applied

Replaced `AddXmlSerializerFormatters()` with `AddXmlDataContractSerializerFormatters()` in controller configuration.

Preserved:

- `ReturnHttpNotAcceptable = true`
- JSON `ReferenceHandler.IgnoreCycles`
- existing validation setup
- Swagger setup
- JWT setup
- API versioning setup

No API response JSON shape was changed.

## 4. Command run and result

Command:

```bash
docker compose config
```

Result: Passed. Docker Compose configuration rendered successfully.

## 5. Docker restart command for user

```bash
docker compose up -d --build api
```

## 6. Manual test commands

```bash
curl -i -H "Accept: application/xml" "http://localhost:8080/api/v1/students?page=1&size=3"
```

Expected: `200` XML

```bash
curl -i -H "Accept: application/json" "http://localhost:8080/api/v1/students?page=1&size=3"
```

Expected: `200` JSON

```bash
curl -i -H "Accept: text/csv" "http://localhost:8080/api/v1/students?page=1&size=3"
```

Expected: `406`
