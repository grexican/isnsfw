# DB Setup
Need a local install of PostGres.

Then run this script (from the main postgres db):

```
CREATE USER isnsfw WITH PASSWORD 'IsNSFW';
GRANT ALL PRIVILEGES ON DATABASE isnsfw TO isnsfw;
```

Need to test your migrations from the CLI? `> dotnet tool install -g FluentMigrator.DotNet.Cli`

Then... https://fluentmigrator.github.io/articles/runners/dotnet-fm.html

Example:
Migrate all:
`\isnsfw\src\IsNsfw.Migration\bin\Debug\netcoreapp2.1>dotnet fm migrate -p postgres -c "Server=127.0.0.1;Port=5432;Database=isnsfw;User ID=isnsfw;Password=IsNSFW;Pooling=true;" -a IsNsfw.Migration.dll`

Rollback all:
`\isnsfw\src\IsNsfw.Migration\bin\Debug\netcoreapp2.1>dotnet fm rollback -p postgres -c "Server=127.0.0.1;Port=5432;Database=isnsfw;User ID=isnsfw;Password=IsNSFW;Pooling=true;" -a IsNsfw.Migration.dll`


# Testing Deployments
You'll need Docker: https://store.docker.com/editions/community/docker-ce-desktop-windows
Run `docker.bat` from `src/IsNsfw.Mvc`