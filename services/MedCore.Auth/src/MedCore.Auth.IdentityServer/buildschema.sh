#!/bin/bash

rm -rf "Data/Migrations"

dotnet ef migrations add InitialMigration -c ApplicationDbContext -o Data/Migrations
