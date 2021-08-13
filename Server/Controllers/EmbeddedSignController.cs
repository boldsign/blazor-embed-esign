using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BlazorBoldSignApp.Shared;
using BoldSign.Api;
using BoldSign.Model;
using System.IO;

namespace BlazorBoldSignApp.Server.Controllers
{
    [ApiController]
    [Route("[controller]/{action}")]
    public class EmbeddedSignController : ControllerBase
    {
        public string DocumentID { get; set; }
        private readonly ILogger<EmbeddedSignController> _logger;
        private readonly TemplateClient templateClient;
        private readonly DocumentClient documentClient;

        public EmbeddedSignController(ILogger<EmbeddedSignController> logger, TemplateClient templateClient, DocumentClient documentClient)
        {
            _logger = logger;
            this.documentClient = documentClient;
            this.templateClient = templateClient;
        }

        [HttpGet]
        [ActionName("SignLink")]
        public async Task<EmbeddedSignDetails> SignLink()
        {
            var sendForSignFromTemplate = new SendForSignFromTemplate()
            {
                TemplateId = "**TemplateID**",
                Title = "Affidavit of Residence",
            };
            DocumentCreated documentCreated = await this.templateClient.SendUsingTemplateAsync(sendForSignFromTemplate).ConfigureAwait(false);
            EmbeddedSigningLink embeddedSigning = this.documentClient.GetEmbeddedSignLink(
                 documentId: documentCreated.DocumentId,
                 signerEmail: "Signer1@email.com",
                 redirectUrl: $"{this.Request.Scheme}://{this.Request.Host}/response");
            return new EmbeddedSignDetails() { DocumentID = documentCreated.DocumentId, SignLink = embeddedSigning.SignLink };
        }

    }
}
