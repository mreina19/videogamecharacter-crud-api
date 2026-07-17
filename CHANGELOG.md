## v1.2.0 Configurable Token Expiration (17th July 2026)

- JWT expiration is now read from `AppSettings:Expiration` instead of being hardcoded

## v1.1.0 Partial Updates (PATCH) (15th July 2026)

- Added PATCH endpoints for characters and users, allowing partial updates
- PUT and PATCH now use separate DTOs: PUT requires every field (full replace), PATCH accepts only the fields being changed
- Duplicate validation on PATCH endpoints for fields not included in the request

## v1.0.0 Video Game Character API with JWT Authentication (15th July 2026)

- Project scaffolding
- Field length validation
- Duplicate validation
- Exception handling and logging
- JWT authentication with login and token verification
- Role-based access control (`Admin`, `Normal`) via `[Authorize]`
- User management endpoints (register, get, update, delete), restricted to admins
- Password hashing using ASP.NET Core Identity's `PasswordHasher`
- Live role and active-status re-checking on every request via `DbClaimsTransformation`
- Account deactivation (`IsActive`), enforced at login
- Character write operations (create, update, delete) restricted to admins
- Self-delete protection for admin accounts
- Unique constraints on user email and character name/game combination
- `UserRoles` enum for user permission levels
- `CharacterRoles` enum for narrative character roles