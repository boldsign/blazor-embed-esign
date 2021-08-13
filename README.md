# BoldSign - Blazor
## Integrate e-signature solution using BoldSign in Blazor application

BoldSign API provides an easy and clean interface to integrate the entire workflow into your app with a matter of minutes. 

### Prerequisites
1.	Signup for [BoldSign trial](https://account.boldsign.com/signup?planId=101)
2.	Create a Hosted Blazor WebAssembly application using the following command.
```csharp
dotnet new blazorwasm --hosted -o BlazorESignApp
```
3.	Install BoldSign API’s NuGet package with the following command.
```csharp
dotnet add package BoldSign.Api
```
4.	Acquire needed BoldSing app credentials from here. [Authentication - Help Center - BoldSign](https://www.boldsign.com/help/api/general/authentication/#basic-authentication)
5.	Create a template in the BoldSign template designer.
[Creating Templates - Help Center - BoldSign](https://www.boldsign.com/help/getting-started/creating-templates/)
6.	Now you got all the prerequisites ready to start embedding BoldSign API in your blazor app.

### Add BoldSign Services to DI 

To communicate with BoldSign API, you need to add authentication headers, base path, etc., to HttpClient. Use the following code in the “BlazorESignApp.Server” project’s startup.cs to do the same

```csharp
ApiClient apiClient = new ApiClient(); 

apiClient.Configuration.DefaultHeader.Add("X-API-KEY", "**Api key**"); 

services.AddSingleton(apiClient); 

services.AddSingleton(new DocumentClient(apiClient)); 

services.AddSingleton(new TemplateClient(apiClient)); 
```

### Create & Send  e-signature documents through API 

You have multiple options to create and send e-signature documents to needed recipients. For this article, we are choosing a template-based approach. A document template can be configured with predefined documents and placeholder recipients with signature fields for each. To use a template, you need to give the actual recipient name & email address for each placeholder recipient and the template ID of the required template from the BoldSign web app. Place the following code in the “BlazorESignApp.Server” project’s controller SignLink action and input the former in the following code
```csharp
var sendForSignFromTemplate = new SendForSignFromTemplate() 
{ 
    TemplateId = "**TemplateID**", 
    Title = "Affidavit of Residence", 
}; 

DocumentCreated documentCreated = await this.templateClient.SendUsingTemplateAsync(sendForSignFromTemplate).ConfigureAwait(false); 
```

### Collect signature from a recipient 

Now that you created an e-signature document request, you can get the URL to sign for each recipient and show it to them within an Iframe to the corresponding user. For this, you need codes in two places. One is in the “BlazorESignApp.Server” project to add a controller action to fetch URL to sign for each recipient to do this. Another is in the “BlazorESignApp.Client” project to add Iframe to display the document to sign. Find the code for 

```csharp
EmbeddedSigningLink embeddedSigning = this.documentClient.GetEmbeddedSignLink( 
                 documentId: documentCreated.DocumentId, 
                 signerEmail: "signer1@email.com", 
                 redirectUrl: $"{this.Request.Scheme}://{this.Request.Host}/response"); 
```

```csharp
<iframe id="sign_page"
        src="@embeddedSignDetails.SignLink"
        height="600"
        width="1100"
        class="frame">
</iframe>
```
```cs
@code { 

    private EmbeddedSignDetails embeddedSignDetails; 

    protected override async Task OnInitializedAsync() 
    { 
        embeddedSignDetails = await Http.GetFromJsonAsync<EmbeddedSignDetails>("EmbeddedSign/SignLink").ConfigureAwait(false); 
    } 
} 
```
Once all the recipients signed the document, you use the [DownloadDocument](https://www.boldsign.com/help/api/document/download-document/) API to fetch the completed document and send further processes, if any.
