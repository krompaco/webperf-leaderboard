add-type @"
    using System.Net;
    using System.Security.Cryptography.X509Certificates;
    public class TrustAllCertsPolicy :ICertificatePolicy {
        public bool CheckValidationResult(
            ServicePoint srvPoint, X509Certificate certificate,
            WebRequest request, int certificateProblem) {
            return true;
        }
    }
"@
[System.Net.ServicePointManager]::CertificatePolicy = New-Object TrustAllCertsPolicy
$url = "https://localhost:5001/swagger/v1/swagger.json"
$output = "$PSScriptRoot\swagger.json"
$start_time = Get-Date

Invoke-WebRequest -Uri $url -OutFile $output
Write-Output "Time taken:$((Get-Date).Subtract($start_time).Seconds) second(s)"

nswag swagger2csclient /input:"swagger.json" /generateUpdateJsonSerializerSettingsMethod:true /generateClientClasses:true /generateClientInterfaces:false /injectHttpClient:"true" /disposeHttpClient:"false" /generateExceptionClasses:true /exceptionClass:"ApiException" /wrapDtoExceptions:true /useHttpClientCreationMethod:false /httpClientType:"System.Net.Http.HttpClient" /useHttpRequestMessageCreationMethod:false /useBaseUrl:true /generateBaseUrlProperty:true /generateSyncMethods:false /exposeJsonSerializerSettings:true /clientClassAccessModifier:"public" /typeAccessModifier:"public" /generateContractsOutput:false /parameterDateTimeFormat:"s" /generateUpdateJsonSerializerSettingsMethod:false /serializeTypeInformation:false /queryNullValue:"" /className:"{controller}Client" /operationGenerationMode:"MultipleClientsFromPathSegments" /generateOptionalParameters:false /generateJsonMethods:false /enforceFlagEnums:false /parameterArrayType:"System.Collections.Generic.IEnumerable" /parameterDictionaryType:"System.Collections.Generic.IDictionary" /responseArrayType:"System.Collections.Generic.ICollection" /responseDictionaryType:"System.Collections.Generic.IDictionary" /wrapResponses:false /generateResponseClasses:true /responseClass:"SwaggerResponse" /namespace:"WebApp.Services.Generated" /requiredPropertiesMustBeDefined:true /dateType:"System.DateTimeOffset" /anyType:"object" /dateTimeType:"System.DateTimeOffset" /timeType:"System.TimeSpan" /timeSpanType:"System.TimeSpan" /arrayType:"System.Collections.Generic.ICollection" /arrayInstanceType:"System.Collections.ObjectModel.Collection" /dictionaryType:"System.Collections.Generic.IDictionary" /dictionaryInstanceType:"System.Collections.Generic.Dictionary" /arrayBaseType:"System.Collections.ObjectModel.Collection" /dictionaryBaseType:"System.Collections.Generic.Dictionary" /classStyle:"Poco" /generateDefaultValues:true /generateDataAnnotations:true /handleReferences:false /generateImmutableArrayProperties:false /generateImmutableDictionaryProperties:false /inlineNamedArrays:false /inlineNamedDictionaries:false /inlineNamedTuples:true /inlineNamedAny:false /generateDtoTypes:true /generateOptionalPropertiesAsNullable:false /output:"src\WebApp\Services\Generated\ApiClient.cs"