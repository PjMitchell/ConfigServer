# Endpoints

### Config Client ~/Clients

GET: Gets All clients

POST: Updates client

/\{Client Id} GET: Gets specified client

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
/{clientId}/{resource} DELETE: Delets Resource file


### Resource Archive ~/ResourceArchive
/{clientId} GET: Gets Catalogue of archived resources
/{clientId}/{resource}  GET: Gets archived resource file
/{clientId}/{resource}  DELETE: Deletes archived resource file
/{clientId}?before={date} DELETE:Deletes archived resource files created before set date

### Config Archive ~/Archive
/{clientId} GET: Gets Catalogue of archived configs
/{clientId}/{configName}  GET: Gets archived configs json
/{clientId}/{resource}  DELETE: Deletes archived configs
/{clientId}?before={date} DELETE:Deletes archived configs created before set date