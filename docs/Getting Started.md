# DB Setup
Need a local install of PostGres.

Then run this script (from the main postgres db):

```
CREATE USER isnsfw WITH PASSWORD 'IsNSFW';
GRANT ALL PRIVILEGES ON DATABASE isnsfw TO isnsfw;
```

Need to test your migrations from the CLI? `> dotnet tool install -g FluentMigrator.DotNet.Cli`

Then... https://fluentmigrator.github.io/articles/runners/dotnet-fm.html