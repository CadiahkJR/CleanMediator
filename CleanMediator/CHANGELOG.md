# 📦 Changelog

All notable changes to **CleanMediator** will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/),
and this project adheres to [Semantic Versioning](https://semver.org/).

---

## [Unreleased]

### Added
- Custom dispatcher for `IRequest<T>` and `IRequest` support
- Pipeline behavior support for both generic and non-generic requests
- DI registration via `AddCleanMediator()` extension
- Example usage with xUnit + Shouldly tests
- GitHub Actions for NuGet publishing with version validation

---

## [1.0.0] - 2024-04-14

### Added
- Initial release of CleanMediator
- Generic and non-generic request support
- IPipelineBehavior pre/post processing support
- Automatic handler and behavior discovery
- MIT licensed, open source and extensible