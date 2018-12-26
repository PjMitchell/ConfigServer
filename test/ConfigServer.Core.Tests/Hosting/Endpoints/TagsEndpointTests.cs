using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Xunit;
using Microsoft.AspNetCore.Http;
using ConfigServer.Server;
using System.Linq;
using System;
using System.Security.Claims;

namespace ConfigServer.Core.Tests.Hosting
{
    public class TagEndpointTests
    {
        private readonly TagEndpoint target;
        private readonly Mock<IHttpResponseFactory> responseFactory;
        private readonly ConfigurationModelRegistry registry;
        private readonly ConfigServerOptions options;
        private  readonly Tag tagOne;
        private readonly Tag tagTwo;

        public TagEndpointTests()
        {
            tagOne = new Tag { Value = "Tag One" };
            tagTwo = new Tag { Value = "Tag Two" };

            registry = new ConfigurationModelRegistry();
            registry.AddConfigurationSet(Create<ConfigSetWithTagOne>(tagOne));
            registry.AddConfigurationSet(Create<ConfigSetWithTagTwo>(tagTwo));
            registry.AddConfigurationSet(Create<ConfigSetWithTagOneAgain>(tagOne));
            registry.AddConfigurationSet(Create<ConfigSetWithNoTag>(null));
            options = new ConfigServerOptions();
            responseFactory = new Mock<IHttpResponseFactory>();
            responseFactory.Setup(r => r.BuildJsonResponse(It.IsAny<HttpContext>(), It.IsAny<IEnumerable<Tag>>()))
                .Returns(Task.FromResult(true));
                        
            target = new TagEndpoint(responseFactory.Object, registry);
        }

        [Fact]
        public async Task CallsResponseFactoryWithTags()
        {
            var context = TestHttpContextBuilder.CreateForPath($"/")
               .WithClaims()
               .TestContext;
            var expectedTags = new[] { tagOne, tagTwo };
            await target.Handle(context, options);
            responseFactory.Verify(r => r.BuildJsonResponse(context, It.Is<IEnumerable<Tag>>(o => TagsMatch(o, expectedTags))), Times.AtLeastOnce());
        }

        private bool TagsMatch(IEnumerable<Tag> t1, IEnumerable<Tag> t2)
        {
            return new HashSet<Tag>(t1).SetEquals(t2);
        }

        private ConfigurationSetModel<TModel> Create<TModel>(Tag tag) where TModel : ConfigurationSet
        {
            var model = new ConfigurationSetModel<TModel>();
            model.RequiredClientTag = tag;
            return model;
        }

        public class ConfigSetWithTagOne : ConfigurationSet
        {

        }

        public class ConfigSetWithTagTwo : ConfigurationSet
        {

        }

        public class ConfigSetWithTagOneAgain : ConfigurationSet
        {

        }

        public class ConfigSetWithNoTag : ConfigurationSet
        {

        }
    }


}
