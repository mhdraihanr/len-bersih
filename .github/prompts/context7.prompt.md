// .github/copilot-instructions.md

# Len Bersih Copilot + Context7 Integration

## Context7 Auto-Trigger Rules

- Always use Context7 MCP for .NET, C#, ASP.NET Core, Blazor, Entity Framework queries
- Auto-resolve library documentation before generating code
- Validate against current .NET 9 patterns from Context7

## Project-Specific Context

- **Architecture**: LenBersih.Api (backend) + LenBersih.Web (Blazor WASM) + LenBersih.Shared (DTOs)
- **Styling**: Component-scoped CSS with AOS animations, mobile-first responsive design
- **Build Chain**: `dotnet build LenBersih.sln` after every change

## Development Workflow

1. Context7 resolves documentation automatically
2. Generate code following retrieved patterns
3. Build project to validate: `dotnet build LenBersih.sln`
4. Fix any errors before proceeding
5. Update README.md with notable changes

## Animation & UI Patterns

- Use AOS library (`data-aos` attributes) for scroll animations
- Follow established keyframes: `slideInFromLeft`, `fadeInUp`
- Maintain mobile breakpoints at ≤768px
- Keep component CSS scoped to avoid conflicts

## API Integration

- All endpoints follow REST conventions via `ReportsController`
- Shared validation through `ReportMetadata` helper class
- In-memory storage (extend to database when needed)
