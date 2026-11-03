.PHONY: build test format format-check run restore clean setup-hooks

# Default target
all: build

## setup-hooks: Configure git to use project pre-commit hooks
setup-hooks:
	git config core.hooksPath .githooks

## restore: Restore NuGet packages
restore:
	dotnet restore ElternAnker.slnx

## build: Build the entire solution
build: restore
	dotnet build ElternAnker.slnx --no-restore --configuration Release

## test: Run all tests
test: build
	dotnet test ElternAnker.slnx --no-build --configuration Release \
		--logger "console;verbosity=normal" \
		--collect:"XPlat Code Coverage"

## format: Format all source files
format:
	dotnet format ElternAnker.slnx

## format-check: Verify formatting (CI gate – exits 1 on diff)
format-check:
	dotnet format ElternAnker.slnx --verify-no-changes

## run: Start the API in Development mode
run:
	dotnet run --project src/ElternAnker.Api --environment Development --configuration Debug

## clean: Remove build artifacts
clean:
	dotnet clean ElternAnker.slnx
	Get-ChildItem -Recurse -Directory -Filter "bin"  | Remove-Item -Recurse -Force
	Get-ChildItem -Recurse -Directory -Filter "obj"  | Remove-Item -Recurse -Force

## help: Show this help
help:
	@Select-String -Path Makefile -Pattern "^## " | ForEach-Object { $_.Line -replace "^## ", "" }

