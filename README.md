# kebabify-api

simple-api-for-kebabifying-strings

[![kebabify - Build](https://github.com/pellehenriksson/kebabify-api/actions/workflows/build.yml/badge.svg)](https://github.com/pellehenriksson/kebabify-api/actions/workflows/build.yml)

![code coverage](https://img.shields.io/endpoint?url=https://gist.githubusercontent.com/pellehenriksson/989c8b5ae41991d482f24f7cf42f741a/raw/kebabify-api-coverage.json)
## architecture

- uses an azure function as the api
- write created 'kebabs' to a storage account
- use managed identity between services
- build and deploy with github
- use open api / swagger

## usage
