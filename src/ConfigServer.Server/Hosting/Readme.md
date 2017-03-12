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
/ConfigurationSet/\{clientId}/\{Config name} POST: Uploads configuration set file

/Configuration/\{clientId}/\{Configuration Set}/\{Config name} POST: Uploads configuration file