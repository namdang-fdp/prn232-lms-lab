# PRN232 LMS LAB 2 Bruno Run Order

1. Start the stack with Docker:

```bash
docker compose up -d --build
```

2. Open Dozzle:

```text
http://localhost:9999
```

3. Open Swagger:

```text
http://localhost:8080/swagger
```

4. In Bruno, select the `local` environment.
5. Run `01-Content-Negotiation`.
6. Run `02-Public-Resources`.
7. Run `04-Auth-Login` before validation, protected, refresh, and admin tests. Login saves tokens to environment variables.
8. Run `05-Refresh-Token`. Refresh saves rotated admin tokens to environment variables.
9. Run `03-Validation`.
10. Run `06-Authorization`.
11. Run `07-Admin-Role`.
12. Run `08-Advanced-Routing-Versioning`.
13. Run SQL verification commands manually from `09-Database-Verification-Notes/DATABASE_VERIFICATION.md`.

Some create requests are intentionally repeatable-smoke tolerant: they accept either first-run `201 Created` or repeated-run business validation such as duplicate data, but they must not return `401` when a valid token is supplied.
