# AzureCognitiveServiceConnector

A simple Azure Function, which takes the fileUri of a PDF document as an HTTP Query parameter and will send it to an Azure Cognitive service for analyses. 
The result will be KeyValue pairs as JSON objects. 

## Configuration

You need to set three config flags to work with the function:

* ```modelId``` = ID of the Cognitive service model to be used
* ```endpoint``` = The Azure Cognitive Services Endpoint of your Azure cognitive services resource 
* ```key``` = The Azure cognitive services key to access the Cognitive services. Shoudl be stored in an Azure Key Vault 

## Usage 

Simply make an HTTP call with ```fileUri``` as a query parameter and the function will return the key value pairs of the provided document. 

### HTTP Sample

```
GET /api/AppliedAiConnectorFunction?fileUri=https%3A%2F%2Fpath.to%2Ffile.pdf HTTP/1.1
X-Functions-Key: <Your Function Key here>
Host: https://<yourfunction>.azurewebsites.net

```

### curl

```shell
curl --request GET \
  --url 'https://<yourfunction>.azurewebsites.net/api/AppliedAiConnectorFunction?fileUri=https%3A%2F%2Fpath.to%2Ffile.pdf' \
  --header 'x-functions-key: <Your Function Key here>'
```

  
