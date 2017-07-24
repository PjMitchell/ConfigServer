﻿# Endpoints

### Config Client ~/Clients

GET: Gets All clients

POST: Updates client
Required Claim: option.ClientAdminClaimType 
GET 'read','write'
POST 'write'

/{Client Id} GET: Gets specified client

### Config Client Groups ~/ClientGroup
GET Gets All Groups
POST Update Groups
/{GroupId} GET
/{GroupId}/Clients GET
/None/Clients GET

Required Claim: option.ClientAdminClaimType 
GET 'read','write'
POST 'write'

### Config ~/
/\{Client Id}/\{Config name} GET: Gets config.

### Config Manager ~/Manager
GET: Configuration Manager App

### Configuration Set ~/ConfigurationSet
GET: Gets all configuration set summaries

/Model/\{Client Id}/\{Configuration Set} GET: Model for configuration set

/Value/\{Client Id}/\{Config name}

GET: Gets Config model for editor

POST: Sets Config from editor model

### Download ~/Download
/\{clientId}/\{Configuration Set}.json GET: gets configuration set file

/\{clientId}/\{Configuration Set}/\{Config name}.json GET: gets configuration file

### Upload ~/Upload
/ConfigurationSet/\{clientId}/\{Configuration Set} POST: Uploads configuration set file

/Configuration/\{clientId}/\{Config name} POST: Uploads configuration file

### Resource ~/Resource
/{clientId} GET :Gets Catalogue of resources

/{clientId}/{resource} GET: Gets Resource file

/{clientId}/{resource} POST: Uploads Resource file

/{clientId}/to/{clientId} POST: Copies resources to client in same group

/{clientId}/{resource} DELETE: Delets Resource file


### Resource Archive ~/ResourceArchive
/{clientId} GET: Gets Catalogue of archived resources

/{clientId}/{resource}  GET: Gets archived resource file

/{clientId}/{resource}  DELETE: Deletes archived resource file

/{clientId}?before={date} DELETE:Deletes archived resource files created before set date
Required Claim: option.ClientAdminClaimType 
GET 'read','write'
DELETE 'write'


### Config Archive ~/Archive
/{clientId} GET: Gets Catalogue of archived configs

/{clientId}/{configName}  GET: Gets archived configs json

/{clientId}/{resource}  DELETE: Deletes archived configs

/{clientId}?before={date} DELETE:Deletes archived configs created before set date

Required Claim: option.ClientAdminClaimType 
GET 'read','write'
DELETE 'write'

### Config Snapshot ~/Snapshot


POST Save snapshot
/{snapShotId} DELETE: Deletes snapShot

/Group/{clientGroupId} GET: Gets SnapshotIds for clientGroupId

Required Claim: option.ClientAdminClaimType 
GET DELETE 'configurator','admin'

/{snapShotId}/to/{clientId}  Post: Pushes snapshot to client Id
Requires Client Claim


