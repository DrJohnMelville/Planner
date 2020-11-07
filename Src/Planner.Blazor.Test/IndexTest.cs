using System;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NodaTime;
using Xunit;

namespace Planner.Blazor.Test
{
    public class IndexTest
    {
        private readonly TestContext context = new TestContext();
        private readonly Mock<IClock> clock = new Mock<IClock>();
        private readonly MockNavigationManager nav = new MockNavigationManager();


        [Fact]
        public void Test1()
        {
            clock.Setup(i => i.GetCurrentInstant()).Returns(Instant.FromUtc(1975, 07, 28, 23, 59, 59));
            context.Services.AddSingleton(clock.Object);
            context.Services.AddSingleton<NavigationManager>(nav);
            var sut = context.RenderComponent<Planner.Blazor.Pages.Index>();
            Assert.Equal("/DailyPage/1975-07-28", nav.NavigateToLocation);
        }
    }
    
    public class MockNavigationManager : NavigationManager, IDisposable
    {
        public string NavigateToLocation { get; private set; }



        /// <summary>
        /// Navigates to the specified URI.
        /// </summary>
        /// <param name="uri">The destination URI. This can be absolute, or relative to the base URI
        /// (as returned by <see cref="NavigationManager.BaseUri"/>).</param>
        /// <param name="forceLoad">If true, bypasses client-side routing and forces the browser to load the new page from the server, whether or not the URI would normally be handled by the client-side router.</param>
        public new virtual void NavigateTo(string uri, bool forceLoad = false)
        {
            //HERE WE ARE INTENTIONALLY OVERRIDING THE IMPLEMENTATION.
            NavigateToCore(uri, forceLoad);
        }
        protected override void NavigateToCore(string uri, bool forceLoad)
        {
            NavigateToLocation = uri;
            Uri = $"{this.BaseUri}{uri}";
        }

        protected sealed override void EnsureInitialized()
        {
            Initialize("http://localhost:5000/", "http://localhost:5000/dashboard/");
        }

        public MockNavigationManager()
        {
            EnsureInitialized();
        }


        public void Dispose()
        {
        }
    }
}