using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using MnistImageStore.Models;
using MnistImageStore.Persistence;

namespace MnistImageStore.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/MnistImages")]
    public class MnistImageController : ControllerBase
    {
        // TODO: 
        //   Figure out how to explicitly run ConfigureServices() on startup... currently it's running on first access.
        //   Explicit route for endpoints (include definition of where params are)
        //   Error handling
        //   Versioning??
        //   Metrics??
        //   Where to store MNIST files?  Within solution... from a URL (e.g. S3 etc..)
        //   Logging... do I use the ASP/NET one (Microsoft.Extensions.Logging)... wrap an abstraction around it?

        /// <summary>Holds all MNIST images.</summary>
        protected List<MnistImage> allMnistImages;
        /// <summary>Holds MNIST images keyed by the image label.</summary>
        protected MnistImageLabelIndex mnistImagesByLabel;

        /// <summary>
        /// Initialises a new instance of the MnistImageStore.Controllers.MnistImageController class.
        /// </summary>
        /// <param name="allMnistImages">Holds all MNIST images.</param>
        /// <param name="mnistImagesByLabel">Holds MNIST images keyed by the image label.</param>
        public MnistImageController(List<MnistImage> allMnistImages, MnistImageLabelIndex mnistImagesByLabel)
        {
            this.allMnistImages = allMnistImages;
            this.mnistImagesByLabel = mnistImagesByLabel;
        }

        [HttpGet]
        [Route("{id}")]
        public ActionResult<MnistImage> Get(Int32 id)
        {
            if (id < 0 || id > allMnistImages.Count())
                return NotFound();

            return allMnistImages[id];
        }

        // TODO: Probably shouldn't have consecutive numbers in the URI... doesn't feel like proper REST
        //   I think index should be part of an OData query
        //   Or can I do like /Label(3)/23534
        [HttpGet]
        [Route("Label/{label}/{index}")]
        public ActionResult<MnistImage> GetByLabel(Int32 label, Int32 index)
        {
            String test = HttpContext.Request.QueryString.Value;
            if (mnistImagesByLabel.ContainsLabel(label) == false)
                throw new ArgumentException($"Parameter '{nameof(label)}' contains invalid value '{label}'.", nameof(label));
            if (index < 0 || index >= mnistImagesByLabel[label].Count)
                return NotFound();

            return allMnistImages[mnistImagesByLabel[label][index]];
        }
    }
}
