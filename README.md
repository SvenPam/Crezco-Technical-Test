
# Crezco Technical Test

## Brief

Design a micro-service that would use (wrap, really) any of the 3rd party location-from-IP address service.

 The service that you’ll build must expose a REST interface.

    - Service should maintain a cache
    - Service should maintain a persistent store of the looked-up values.
    - unit tests
    - integration tests.

It may use Swagger (or Postman) as a UI

There is nothing special about IP lookup – you may choose some other comparable 3rd party service that would look something up – say, whether from location.

We expect the service to be implemented in C#/Net 7.

 ## Setup

 This project requires Azure Cosmos to run. To set this up for free, locally, you can follow these instructions:

 1. Download and install the emulator from instructions [here](https://learn.microsoft.com/en-us/azure/cosmos-db/local-emulator?tabs=ssl-netstd21).
 2. Run the emulator to obtain address and key.
    a. Address can be placed in `appSettings.Development`
    b. Key can be placed in local user secrets store:
```` json
 {
   "CosmosConfiguration": {
     "Key": "your-key-here"
   }
 }
````
The solution will run through either docker or IIS express.

## Comments

This project is, by admission, overengineered for the problem. That said, it aims to display the approaches which can be taken for a long life service, where clean, testable code is required to provide assurance of a working service, which remains maintainable.

The solution makes use of a multilayered 'onion' approach to the project, respecting SRP at an architectural level. Primarily, the application layer uses Mediatr to abstract some core themes as caching and validation through
 behaviours. This allows easy reuse for future requests.

To highlight the spec above:

>  - Service should maintain a cache

This is achieved through a cacheing behaviour on requests. It utilizes the Polly.NET caching policy, with an in-memory cache. This could easily be replaced with a distributed cache through the service registration.

It provides a 30-min sliding cache by IP Address.

> - Service should maintain a persistent store of the looked-up values.

Kay value pairs make a great fit for CosmosDb, there is a single write per document, with no cross queries/shared data (JOINS). This has been implemented through EF Core, see `LocationDbContext`.

> - unit tests

There is near 100% coverage of pertinent functionality, see `tests/**`

> - integration tests.

This is covered in the `API.Tests` project.
