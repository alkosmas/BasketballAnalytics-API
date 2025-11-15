using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BasketballAnalytics.Application.Common.Interfaces;
using BasketballAnalytics.Application.Features.Teams.Commands;
using BasketballAnalytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace BasketballAnalytics.Application.Tests.Features.Teams.Command
{
    public  static class DbContextMock
    {
        public static Mock<DbSet<T>> Create<T>(List<T> data) where T : class
        {
            var queryable = data.AsQueryable();
            var mockDbSet = new Mock<DbSet<T>>();
            mockDbSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            mockDbSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            mockDbSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            mockDbSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());
            mockDbSet.Setup(d => d.Add(It.IsAny<T>())).Callback<T>((s) => data.Add(s));
            return mockDbSet;
        }
    }

    public class CreateTeamCommandHandlerTests
    {
        private readonly Mock<IApplicationDbContext> _mockContext;

        private readonly Mock<IMemoryCache> _mockCache;
        
        private readonly CreateTeamCommandHandler _handler;

        public CreateTeamCommandHandlerTests()
        {
            _mockContext = new Mock<IApplicationDbContext>();
            
            _mockCache = new Mock<IMemoryCache>(); 

            var teamsData = new List<Team>();

            var mockTeamDbSet = DbContextMock.Create(teamsData);

            _mockContext.Setup(c => c.Teams).Returns(mockTeamDbSet.Object);

            _handler = new CreateTeamCommandHandler(_mockContext.Object, _mockCache.Object);

            
        }
    

    [Fact]
    public async Task Handle_Should_AddTeamToDbContext_And_SaveChangesAsync()
    {
        var command = new CreateTeamCommand("Test Team", "Test City");

        var result = await _handler.Handle(command, CancellationToken.None);

        _mockContext.Verify(c => c.Teams.Add(It.IsAny<Team>()), Times.Once);

        _mockContext.Verify(c => c.SaveChangesAsync(CancellationToken.None), Times.Once);

        Assert.NotEqual(Guid.Empty, result);
    }
    }
}

