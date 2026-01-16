# kebabify-api

simple-api-for-kebabifying-strings

[![kebabify - Build](https://github.com/pellehenriksson/kebabify-api/actions/workflows/build.yml/badge.svg)](https://github.com/pellehenriksson/kebabify-api/actions/workflows/build.yml)

![code coverage](https://img.shields.io/endpoint?url=https://gist.githubusercontent.com/pellehenriksson/642415f103ee4903fc77a46f95404f77/raw/kebabify-api-coverage.json)
## architecture

- uses an azure function as the api
- write created 'kebabs' to a storage account
- use managed identity between services
- build and deploy with github
- use open api / swagger

## usage
