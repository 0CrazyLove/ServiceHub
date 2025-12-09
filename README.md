# ServiceHub Enterprise Template

A production-ready, containerized full-stack architecture featuring **ASP.NET Core Web API**, **Astro SSR**, and **SQL Server 2022**. This template is designed for scalability, maintainability, and ease of deployment using Docker Compose.

## Quick Start

To start the entire stack (Backend, Frontend, and Database):

```bash
docker-compose --env-file .env.docker up --build
```

The application will be available at:
- **Frontend**: [http://localhost:4321](http://localhost:4321)
- **Backend API**: [http://localhost:5097](http://localhost:5097)

## Documentation

Detailed documentation is available in the `Docs` directory:

- [**Overview**](Docs/Overview.md): Architecture, technology stack, and key features.
- [**Setup Guide**](Docs/Setup.md): Prerequisites, environment setup, and running instructions.
- [**Configuration**](Docs/Configuration.md): Environment variables, database settings, and OAuth integration.

## License

This project is licensed under the **GNU General Public License v3.0**. See the [LICENSE](LICENSE) file for details.
