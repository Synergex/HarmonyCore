@echo off
pushd %~dp0..

if exist Data\Migrations\. rmdir /s /q Data\Migrations

dotnet ef migrations add initial_identity_migration                 -c ApplicationDbContext    -o Data/Migrations/AspNetIdentity/ApplicationDb
dotnet ef migrations add initial_is4_server_configuration_migration -c ConfigurationDbContext  -o Data/Migrations/IdentityServer/ConfigurationDb
dotnet ef migrations add initial_is4_persisted_grant_migration      -c PersistedGrantDbContext -o Data/Migrations/IdentityServer/PersistedGrantDb

popd