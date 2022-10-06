# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.2.1] - 2022-04-24
- Fix detection for PlayerPrefs where the key contains '_h'
- Use unicode for windows registry lookups to support none ASCII chars in projects names

## [1.2.0] - 2022-01-01
### Added
- Enhanced search field to filter player preferences by key or value
- Add sorting functionality for Pref entries (none, ascending, descending)

### Removed
- Remove Unity 2017 support
- Remove Unity 2018 support

## [1.1.2] - 2021-07-01
- Fixed ImageManger icon detection

## [1.1.1] - 2021-05-23
- Add utf8 key encryption support for windows

## [1.1.0] - 2021-05-17
- Improve key validation with more characters
- Async output reading for MAC plist process
- Performance optimizations

## [1.0.4] - 2020-09-20
- Add handling for special characters in product/company name
- Improvement of plist read call on MAC

## [1.0.3] - 2020-09-20
- Fix text color on professional skin

## [1.0.2] - 2020-08-11
- Switch package author to 'BG Tools'
- Fix UPM documentation image path

## [1.0.1] - 2020-06-01
- Resizable column width for table layout
- Multiple UX improvements
- Add manual

## [1.0.0] - 2020-05-26
This is the first release of PlayerPrefs Editor