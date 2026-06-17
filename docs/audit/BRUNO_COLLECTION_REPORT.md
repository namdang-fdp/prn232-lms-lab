# Bruno LAB 2 Collection Report

## 1. Summary

Created a Bruno collection for PRN232 LMS LAB 2 runtime smoke testing and demo through Docker.

Collection path:

```text
bruno/PRN232-LMS-LAB2
```

The collection covers:

- Swagger and Dozzle access checks
- JSON/XML/406 content negotiation
- Public resource routes and query features
- Auth login and refresh-token rotation
- Validation failures
- JWT-protected writes
- Admin role authorization
- API versioning and advanced routing
- Manual database verification notes

## 2. Files/Folders Created

- `bruno/PRN232-LMS-LAB2/bruno.json`
- `bruno/PRN232-LMS-LAB2/environments/local.bru`
- `bruno/PRN232-LMS-LAB2/RUN_ORDER.md`
- `bruno/PRN232-LMS-LAB2/00-Health-and-Docs`
- `bruno/PRN232-LMS-LAB2/01-Content-Negotiation`
- `bruno/PRN232-LMS-LAB2/02-Public-Resources`
- `bruno/PRN232-LMS-LAB2/03-Validation`
- `bruno/PRN232-LMS-LAB2/04-Auth-Login`
- `bruno/PRN232-LMS-LAB2/05-Refresh-Token`
- `bruno/PRN232-LMS-LAB2/06-Authorization`
- `bruno/PRN232-LMS-LAB2/07-Admin-Role`
- `bruno/PRN232-LMS-LAB2/08-Advanced-Routing-Versioning`
- `bruno/PRN232-LMS-LAB2/09-Database-Verification-Notes`
- `docs/audit/BRUNO_COLLECTION_REPORT.md`

Created 34 request `.bru` files plus 1 environment `.bru` file.

## 3. Environment Variables

Environment file:

```text
bruno/PRN232-LMS-LAB2/environments/local.bru
```

Variables:

- `baseUrl = http://localhost:8080`
- `swaggerUrl = http://localhost:8080/swagger`
- `dozzleUrl = http://localhost:9999`
- `adminUsername = admin`
- `adminPassword = 123456`
- `userUsername = user`
- `userPassword = 123456`
- `adminAccessToken`
- `adminRefreshToken`
- `userAccessToken`
- `userRefreshToken`
- `newAdminAccessToken`
- `newAdminRefreshToken`
- `createdSubjectId`

## 4. Folder/Request Inventory

### 00-Health-and-Docs

- `Swagger UI`
- `Dozzle UI`

### 01-Content-Negotiation

- `Students JSON`
- `Students XML`
- `Unsupported Accept`

### 02-Public-Resources

- `v1 Students list`
- `v1 Students by ID`
- `v1 Student not found`
- `v1 Students query features`
- `v1 Courses list`
- `v1 Enrollments list`
- `v2 Students list`

### 03-Validation

- `Subject credit invalid`
- `Student invalid FPTU email`
- `Student future DOB`

### 04-Auth-Login

- `Login admin success`
- `Login user success`
- `Login wrong password`

### 05-Refresh-Token

- `Refresh admin token success`
- `Reuse old admin refresh token should fail`
- `Refresh with invalid token`

### 06-Authorization

- `Protected write without token`
- `Protected write with admin token`
- `Protected write with refreshed admin token`
- `Public GET still works without token`

### 07-Admin-Role

- `Admin check without token`
- `Admin check with user token`
- `Admin check with admin token`
- `Admin check with refreshed admin token`

### 08-Advanced-Routing-Versioning

- `Nested course students`
- `Nested course not found`
- `Route constraint invalid student ID`
- `Unsupported API version`
- `Named route create smoke`

### 09-Database-Verification-Notes

- `DATABASE_VERIFICATION.md`

## 5. Token Capture Behavior

The login requests use Bruno post-response scripts:

- `Login admin success` saves:
  - `adminAccessToken`
  - `adminRefreshToken`
- `Login user success` saves:
  - `userAccessToken`
  - `userRefreshToken`
- `Refresh admin token success` saves:
  - `newAdminAccessToken`
  - `newAdminRefreshToken`

Auth response tests assert that sensitive values are not returned:

- `passwordHash`
- `refreshTokenHash`
- `tokenHash`

## 6. Manual Run Order

See:

```text
bruno/PRN232-LMS-LAB2/RUN_ORDER.md
```

Recommended order:

1. Start stack with Docker: `docker compose up -d --build`
2. Open Dozzle: `http://localhost:9999`
3. Open Swagger: `http://localhost:8080/swagger`
4. Run `01-Content-Negotiation`
5. Run `02-Public-Resources`
6. Run `04-Auth-Login`
7. Run `05-Refresh-Token`
8. Run `03-Validation`
9. Run `06-Authorization`
10. Run `07-Admin-Role`
11. Run `08-Advanced-Routing-Versioning`
12. Run manual SQL verification commands from `09-Database-Verification-Notes/DATABASE_VERIFICATION.md`

## 7. Known Caveats

- Bruno cannot directly run SQL without a DB endpoint. The collection includes manual Docker/PostgreSQL SQL commands instead.
- Some create requests are repeatable-smoke tolerant: they pass for `201 Created` on first run or `400` business/duplicate validation on later runs, but they must not return `401` when a valid token is supplied.
- Token-dependent folders require `04-Auth-Login` first.
- Refreshed-token tests require `05-Refresh-Token/Refresh admin token success` before refreshed-token authorization/admin checks.
- Dozzle and Swagger checks require the Docker stack to be running.

## 8. Scope Confirmation

Confirmed:

- No application code was modified.
- `Program.cs` was not modified.
- Controllers, services, repositories, and entities were not modified.
- `Dockerfile` was not modified.
- DB schema was not modified.
- No migrations were added.
- No `dotnet restore`, `dotnet build`, or `dotnet test` commands were run.
- `docker compose up` and `docker compose up --build` were not run.
- No destructive Docker or database commands were run.
