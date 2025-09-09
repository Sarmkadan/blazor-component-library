# Security Policy

## Reporting Security Issues

If you discover a security vulnerability in the Blazor Component Library, please **do not** create a public GitHub issue. Instead, report it privately to:

**Email:** vz@sarmkadan.com  
**Subject:** [SECURITY] Blazor Component Library Vulnerability

**Please include:**
- Description of the vulnerability
- Steps to reproduce (if applicable)
- Potential impact
- Suggested fix (if available)
- Your contact information (optional)

**Response Timeline:**
- Initial acknowledgment: Within 24-48 hours
- Initial assessment: Within 1 week
- Update on fix: Every 2 weeks until resolved
- Public disclosure: After fix is released

## Supported Versions

| Version | Status | Support Ends | Security Updates |
|---------|--------|--------------|------------------|
| 1.2.x | Active | 2027-05-04 | Yes |
| 1.1.x | Maintenance | 2026-12-01 | Critical only |
| 1.0.x | EOL | 2026-06-01 | No |

**Update Frequency:**
- Security patches: As needed (critical)
- Bug fixes: Quarterly or as needed
- Features: Quarterly releases

## Security Best Practices

### For Users

1. **Keep Updated**
   ```bash
   dotnet add package BlazorComponentLibrary --version latest
   ```

2. **Review Dependencies**
   ```bash
   dotnet list package --outdated
   ```

3. **Use HTTPS** in production
   ```csharp
   app.UseHttpsRedirection();
   ```

4. **Enable Authentication**
   ```csharp
   services.AddAuthentication();
   app.UseAuthentication();
   ```

5. **Validate Input**
   ```csharp
   [Required]
   [StringLength(100)]
   public string Name { get; set; }
   ```

### For Developers

1. **Input Validation**
   - Always validate user input
   - Use data annotations
   - Implement custom validators

2. **Authentication**
   - Require authentication for sensitive endpoints
   - Use secure password storage
   - Implement rate limiting

3. **Authorization**
   - Check permissions before operations
   - Use role-based access control
   - Validate user claims

4. **Error Handling**
   - Don't expose sensitive information
   - Log errors securely
   - Return generic error messages

5. **Dependency Updates**
   - Keep dependencies current
   - Monitor security advisories
   - Use `dotnet update`

## Common Security Issues & Prevention

### SQL Injection
**Problem:** Unsanitized user input in SQL queries  
**Prevention:** Use parameterized queries, Entity Framework Core

```csharp
// ✅ Good - parameterized
var users = await context.Users
    .Where(u => u.Name == name)
    .ToListAsync();

// ❌ Bad - vulnerable
var query = $"SELECT * FROM Users WHERE Name = '{name}'";
```

### Cross-Site Scripting (XSS)
**Problem:** User input rendered as HTML  
**Prevention:** Escape output, use @Html.Encode()

```csharp
// ✅ Good - escaped
@Html.Encode(userInput)

// ❌ Bad - vulnerable
@Html.Raw(userInput)
```

### CSRF (Cross-Site Request Forgery)
**Problem:** Unauthorized actions on behalf of user  
**Prevention:** Use CSRF tokens

```csharp
// Built-in to Blazor EditForm
<EditForm Model="@model" OnValidSubmit="@HandleSubmit">
    <!-- CSRF token included automatically -->
</EditForm>
```

### Sensitive Data Exposure
**Problem:** Sensitive data in logs or responses  
**Prevention:** Encrypt, mask, don't log passwords

```csharp
// ✅ Good - don't log sensitive data
_logger.LogInformation("User login attempt: {Username}", username);

// ❌ Bad - logging password
_logger.LogInformation("Login: {Username}, {Password}", username, password);
```

## Security Features

The library includes:

### Input Validation
- Data annotation validators
- Custom validation rules
- Type-safe form fields

### Authentication Support
- User model with roles
- Password hashing
- Permission checking
- Authorization attributes

### Rate Limiting
- Request throttling
- Per-endpoint limits
- Endpoint exclusions

### Middleware
- Request validation
- Exception handling
- Logging
- CORS support

## Vulnerability Disclosure

When a security issue is reported:

1. **We verify** the vulnerability exists
2. **We assess** the severity and impact
3. **We develop** a fix
4. **We test** the fix thoroughly
5. **We release** a security patch
6. **We credit** the reporter (if desired)

### Severity Levels

| Severity | Impact | Example | Response |
|----------|--------|---------|----------|
| Critical | System compromised | Authentication bypass | 24-48 hours |
| High | Data breach possible | SQL injection | 1 week |
| Medium | Limited impact | Information disclosure | 2 weeks |
| Low | Minor issue | Denial of service | Next release |

## Security Advisories

Published at:
- GitHub Security Advisories
- Release notes for patch versions
- Email notification (subscribe)

### Subscribe to Updates

Monitor security updates:

```bash
# Watch the repository
git watch sarmkadan/blazor-component-library

# Or check releases
https://github.com/sarmkadan/blazor-component-library/releases
```

## Third-Party Dependencies

**Current Dependencies:**
- Microsoft.AspNetCore.Components
- Microsoft.AspNetCore.Components.Web
- Microsoft.Extensions.DependencyInjection
- Microsoft.Extensions.Configuration
- Newtonsoft.Json

**Security Practices:**
- Monitor for known vulnerabilities
- Update dependencies regularly
- Review CVSS scores and patches

## Compliance

The library supports:

- **OWASP Top 10** - Prevention of common vulnerabilities
- **GDPR** - Data protection compliance
- **HIPAA** - Healthcare data protection (when properly configured)
- **PCI DSS** - Payment data security (when properly configured)

## Questions?

For security questions or concerns:

1. **Check documentation** - [Security section in docs](docs/deployment.md#security-best-practices)
2. **Review examples** - [Secure implementation examples](examples/03-UserAuthentication.razor)
3. **Ask privately** - vz@sarmkadan.com
4. **Open discussion** - GitHub Discussions (for non-sensitive)

---

## Security Checklist

Before deploying to production:

- [ ] Enable HTTPS/TLS
- [ ] Configure authentication
- [ ] Implement authorization
- [ ] Enable input validation
- [ ] Review error handling
- [ ] Set up logging
- [ ] Enable rate limiting
- [ ] Configure CORS
- [ ] Update dependencies
- [ ] Run security tests
- [ ] Review secrets/credentials
- [ ] Plan backups and recovery
- [ ] Monitor vulnerabilities
- [ ] Document security measures

---

**Last Updated:** May 4, 2026  
**Version:** 1.2.0  
**Maintained by:** Vladyslav Zaiets

For more information, visit https://sarmkadan.com
