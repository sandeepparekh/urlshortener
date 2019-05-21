# Introduction
This is your basic Url Shortening service similar to `bit.ly`. It can take long Url and convert to a short alias. Also, inflate short URL back to it's original form.

# How it works
## Short code generation
1. Compute MD5 hash of the given long Url
2. Convert MD5 to a base62 encoding ( a-z,A-Z,0-9)
3. Take first 6 characters as Short Url Code. In case of duplication, algorighthm takes the next 6 characters from base62 string untill all options are exhausted.

With base62 encoding  six characters would produce 62^6 (56.8 Billion) possible Short codes which should suffice for this service.

## User Interface
Service works in two modes: Public (default) and User(with User authentication).

### Public mode
Anonymous Users can generated short urls without authentication. If user tries to generate a Short Url for a long Url that has already been converted, user will receive existing Short Code.

### User mode
It is similar with public mode in terms of functionlity but requires authentications. Auth0 is used as an Identity provider. Users can see a list of all the URLs they converted on a separate page. If users tries to a Short Url for a long Url that has already been converted, user will receive existing Short Code.

Note: If a registered user tries to shorten a URL that has already been converted by a Anonymous user and not yet shortened by registered user, it will generate a `new` Short code.

## Data Partiotioning
Azure Table Storage is used as a backend data store. To optimized the data retrival for redirect and User interface, data is stored in two tables.

### UrlRead
Data in this table is consumed by URL shortening user interface. `UserId` is used as `PartitionKey` and 6 character `Short Url Code` is used as `RowKey`. This optimizes the URL data retrieval for Url List Page.

### UrlRedirect
Date in this table is consumed by URL redirect module. First three characters of Shor Url Code are used as `PartitionKey` and 6 character `Short Url Code` is used as `RowKey`. This optimized the retrieval of Long url from short code while doing redirect.

## Scaling
Both UI and redirector scale out if Average CPU% goes over 70%. Max scale count is 10, min is 1. 

## Caching
Url short code and long URL are stored as a Key/Value pair in Azure Cache for Redis for 7 days before they expire.

## Data Replication
Azure storage account is Read-Access-Geo-Redundant Storage(RA-GRS) which provides geo-redundant storage with additional benefits of read accesss to the secondary endpoint.  If an outage occurs in the primary endpoint, applications configured for RA-GRS and designed for high availability can continue to read from the secondary endpoint.


# Project Structure
## UrlShortener.Extensions
This project contains extensions methods for Base conversion and Md5 hash.

## UrlShortener.Models
Contains all the Data Entities used by services and repositories.

## UrlShortener.Redirector
This project handles the redirect from Short URL to long URL. It's deployed as a separate webapp so that it can be individually scaled as need arise.

## UrlShortener.Repositories
Handles data store and retrieval from Azure Storage.

## UrlShortener.Services
Contains all the business login and is unit tested.

## UrlShortener.UI
It's the user interface for generating short urls.

## UrlShortener.UnitTests
Has all the unit tests for business logic.

# Run Locally
1. Checkout the repo locally
2. Open `UrlShortener.sln` in Visual Studio
3. Set `UrlShortener.Redirector` and `UrlShortener.UI` as startup projects. You can do this by going into properties of the solutions and select those two projects as startup projects.
4. Update `ShortUrlDomain` and `UiDomain` in appsettings.json of `UrlShortener.Redirector` and `UrlShortener.UI` with local host and port if required.

# Live Version
UI - https://naniurl.azurewebsites.net
Redirector- http://naniurl.club

PS: Nani means short in a local lanaguage from where I come.

# Future
* API Access to developers
* Protection against abuse
* Infrastructure as code
* Use CosmosDb for high avaibility and lower latency if cost is not an issue.
* Do proper paging with Azure Storage