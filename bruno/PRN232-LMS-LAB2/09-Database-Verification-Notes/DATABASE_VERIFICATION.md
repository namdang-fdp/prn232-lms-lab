# Database Verification Notes

Bruno should not run SQL directly for this project because there is no DB verification API endpoint, and the LAB 2 scope should not add one.

Run these manually after the Docker stack is running.

## Verify Demo Users

```bash
docker compose exec postgres sh -lc 'psql -U "$POSTGRES_USER" -d "$POSTGRES_DB" -c "SELECT \"Username\", \"Role\", \"PasswordHash\" FROM \"Users\";"'
```

Expected:

- `admin` exists with role `Admin`.
- `user` exists with role `User`.
- `PasswordHash` values do not equal `123456`.

## Verify Refresh Tokens Table

```bash
docker compose exec postgres sh -lc 'psql -U "$POSTGRES_USER" -d "$POSTGRES_DB" -c "SELECT COUNT(*) FROM \"RefreshTokens\";"'
```

Expected:

- `RefreshTokens` table exists.
- Count may be `0` before auth smoke tests.
- Count should increase after login/refresh-token requests.

## Log Verification

Open Dozzle after the stack starts:

```text
http://localhost:9999
```

Expected:

- API logs show request method, path, response status, and elapsed time.
- Logs should not include request bodies, passwords, JWTs, refresh tokens, or secrets.
