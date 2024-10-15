using Infra.CrossCutting.Domain.Services;
using Infra.CrossCutting.Notification.Handler;
using Infra.CrossCutting.Notification.Model;
using Infra.CrossCutting.Repository;
using Infra.CrossCutting.Repository.Interfaces;
using Infra.CrossCutting.UoW.Interfaces;
using Infra.CrossCutting.UoW.Models;
using MediatR;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using UnitTest.Infra.Infra.CrossCutting.Domain.Fake;
using Xunit;

namespace UnitTest.Infra.Infra.CrossCutting.Domain
{
    public class DomainServiceTest
    {
        protected readonly Mock<IRepository<EntityTestFaker, int>> _mockRepository;
        protected readonly Mock<IUnitOfWorkBase> _mockUnitOfWork;
        private DomainBaseTest _domainBase;
        private readonly EntityTestFaker _valueRandomTest;
        private readonly List<EntityTestFaker> _listRandomTest;

        public DomainServiceTest()
        {
            // configure
            _mockRepository = new Mock<IRepository<EntityTestFaker, int>>();
            _mockUnitOfWork = new Mock<IUnitOfWorkBase>();         
            _mockUnitOfWork.Setup(x => x.Commit()).Returns(new CommandResponse(true));
            _domainBase = new DomainBaseTest(_mockRepository.Object, _mockUnitOfWork.Object, new DomainNotificationHandler());
            _valueRandomTest = FakeEntityDomainBase.Create();
            _listRandomTest = FakeEntityDomainBase.CreateList();
        }

        [Fact(DisplayName = "Should return true when deleted object")]
        [Trait("[Infra.CrossCutting]-DomainServiceTest", "Domain")]
        public async void ShouldTrueWhenDeletedObject()
        {
            // prepare
            var entity = new EntityTestFaker();
            var id = 1;
            _mockRepository.Setup(x => x.DeleteAsync(id)).ReturnsAsync(true);

            // act           
            var result = await _domainBase.DeleteAsync(id);

            // assert
            Assert.True(result);
            _mockRepository.Verify(x => x.DeleteAsync(id), Times.Once);
        }

        [Fact(DisplayName = "Should return false when dont deleted object")]
        [Trait("[Infra.CrossCutting]-DomainServiceTest", "Domain")]
        public async void ShouldFalseWhenDontDeletedObject()
        {
            // prepare
            var entity = new EntityTestFaker();
            var id = 1;
            _mockRepository.Setup(x => x.DeleteAsync(id)).ReturnsAsync(false);

            // act
            var result = await _domainBase.DeleteAsync(id);

            // assert
            Assert.False(result);
            _mockRepository.Verify(x => x.DeleteAsync(id), Times.Once);
        }

        [Fact(DisplayName = "Should return to insert entity")]
        [Trait("[Infra.CrossCutting]-DomainServiceTest", "Domain")]
        public async void ShouldToInsertEntity()
        {
            // prepare
            var input = new EntityTestFaker() {  ValueRandomTest = _valueRandomTest.ValueRandomTest};          
            _mockRepository.Setup(x => x.InsertAsync(input)).ReturnsAsync(_valueRandomTest);

            // act
            var entity = await _domainBase.InsertAsync(input);

            // assert
            Assert.NotNull(entity);
            Assert.Equal(1, entity.Id);
            _mockRepository.Verify(x => x.InsertAsync(input), Times.Once);
        }

        [Fact(DisplayName = "Should return to update entity")]
        [Trait("[Infra.CrossCutting]-DomainServiceTest", "Domain")]
        public async void ShouldToUpdateEntity()
        {
            // prepare
            _valueRandomTest.ValueRandomTest = "New";            
            _mockRepository.Setup(x => x.UpdateAsync(_valueRandomTest)).ReturnsAsync(_valueRandomTest);

            // act
            var entity = await _domainBase.UpdateAsync(_valueRandomTest);

            // assert
            Assert.NotNull(entity);
            Assert.Equal(1, entity.Id);
            _mockRepository.Verify(x => x.UpdateAsync(_valueRandomTest), Times.Once);
        }

        [Fact(DisplayName = "Should return to select all results async")]
        [Trait("[Infra.CrossCutting]-DomainServiceTest", "Domain")]
        public async void ShouldToSelectAllResultsAsync()
        {
            // prepare
            _mockRepository.Setup(x => x.SelectAllAsync())
                .ReturnsAsync(_listRandomTest);

            // act
            var result = await _domainBase.SelectAllAsync();

            // assert
            _mockRepository.Verify(x => x.SelectAllAsync(), Times.Once);
            Assert.Equal(_listRandomTest, result);
        }

        [Fact(DisplayName = "Should return to select entity by id async")]
        [Trait("[Infra.CrossCutting]-DomainServiceTest", "Domain")]
        public async void ShouldToSelectEntityByIdAsync()
        {
            // prepare
            _mockRepository.Setup(x => x.SelectByIdAsync(1)).ReturnsAsync(_valueRandomTest);

            // act
            var result = await _domainBase.SelectByIdAsync(1);

            // assert
            _mockRepository.Verify(x => x.SelectByIdAsync(1), Times.Once);
            Assert.Equal(_valueRandomTest, result);
            Assert.Equal(_valueRandomTest.Id, result.Id);
        }

        [Fact(DisplayName = "Should return to filter results async")]
        [Trait("[Infra.CrossCutting]-DomainServiceTest", "Domain")]
        public async void ShouldToFilterResultsAsync()
        {
            // prepare
            _mockRepository.Setup(x => x.SelectFilterAsync(It.IsAny<Expression<Func<EntityTestFaker, bool>>>()))
                .ReturnsAsync(_listRandomTest);

            // act
            var result = await _domainBase.SelectFilterAsync(x => x.ValueRandomTest == "Value");

            // assert
            _mockRepository.Verify(x => x.SelectFilterAsync(It.IsAny<Expression<Func<EntityTestFaker, bool>>>()), Times.Once);
            Assert.Equal(2, result.Count());
        }

        [Fact(DisplayName = "Should return true if exist entity")]
        [Trait("[Infra.CrossCutting]-DomainServiceTest", "Domain")]
        public async void ShouldTrueIfExistEntity()
        {
            // prepare
            _mockRepository.Setup(x => x.ExistsAsync(1)).ReturnsAsync(true);

            // act
            var result = await _domainBase.ExistsAsync(1);

            // assert
            _mockRepository.Verify(x => x.ExistsAsync(1), Times.Once);
            Assert.True(result);
        }
    }
    

    [ExcludeFromCodeCoverage]
    public class DomainBaseTest : DomainService<EntityTestFaker, int, IUnitOfWorkBase>
    {
        public DomainBaseTest(IRepository<EntityTestFaker, int> repository, IUnitOfWorkBase unitOfWork, INotificationHandler<DomainNotification> messageHandler) : base(repository, unitOfWork, messageHandler)
        {
        }        
    }
}
