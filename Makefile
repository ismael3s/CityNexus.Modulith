INFRA_PROJECT = ./src/CityNexus.Modulith.Infra
STARTUP_PROJECT = ./src/CityNexus.Modulith.Api

apply-migrations:
	dotnet ef database update --project $(INFRA_PROJECT) -s $(STARTUP_PROJECT)

remove-last-unapplied-migration:
	dotnet ef migrations remove --project $(INFRA_PROJECT) -s $(STARTUP_PROJECT) 

add-migration:
	@if [ -z "$(name)" ]; then \
		echo "Error: You must provide a migration name. Usage: make add-migration name=YourMigrationName"; \
		exit 1; \
	fi; \
	dotnet ef migrations add $(name) --project $(INFRA_PROJECT) -s $(STARTUP_PROJECT) 

revert-and-remove-last-applied-migration:
	@LAST_MIGRATION=$$(dotnet ef migrations list --project $(INFRA_PROJECT) -s $(STARTUP_PROJECT) | tail -2 | head -1 | cut -d "_" -f2); \
	if [ -z "$$LAST_MIGRATION" ]; then \
		echo "No migration to revert."; \
		exit 1; \
	fi; \
	dotnet ef database update $$LAST_MIGRATION --project $(INFRA_PROJECT) -s $(STARTUP_PROJECT); \
	dotnet ef migrations remove --project $(INFRA_PROJECT) -s $(STARTUP_PROJECT)
list-migrations:
	dotnet ef migrations list --project $(INFRA_PROJECT) -s $(STARTUP_PROJECT)