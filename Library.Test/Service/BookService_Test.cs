using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Library.ADT;
using Library.Service;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace Library.Test
{
    public class BookService_Test
    {
        private Mock<IRepositoryManager> _mockRepo;
        private IBookService _bookService;

        private void BuildRepo()
        {
            _mockRepo = new Mock<IRepositoryManager>();

            _mockRepo.Setup(p => p.BookRepository.UnSubscribe(It.IsNotIn(Guid.Empty), It.IsNotIn(Guid.Empty))).ReturnsAsync(() => true);
            _mockRepo.Setup(p => p.BookRepository.UnSubscribe(It.IsIn(Guid.Empty), It.IsIn(Guid.Empty))).ReturnsAsync(() => false);
            _mockRepo.Setup(p => p.BookRepository.Subscribe(It.IsNotIn(Guid.Empty), It.IsNotIn(Guid.Empty))).ReturnsAsync(() => true);
            _mockRepo.Setup(p => p.BookRepository.Subscribe(It.IsIn(Guid.Empty), It.IsIn(Guid.Empty))).ReturnsAsync(() => false);
            _mockRepo.Setup(p => p.BookRepository.IsSubscribed(It.IsNotIn(Guid.Empty), It.IsNotIn(Guid.Empty))).ReturnsAsync(() => true);
            _mockRepo.Setup(p => p.BookRepository.IsSubscribed(It.IsIn(Guid.Empty), It.IsIn(Guid.Empty))).ReturnsAsync(() => false);

            _bookService = new BookService(_mockRepo.Object, new NullLogger<BookService>());
        }

        public static IEnumerable<object[]> GuidsNotSub
        {
            get
            {
                yield return new object[]{ "Not subscribed", Guid.Empty, Guid.Empty };
                yield return new object[]{ "Not subscribed", Guid.NewGuid(), Guid.Empty };
                yield return new object[]{ "Not subscribed", Guid.Empty, Guid.NewGuid() };
                yield return new object[]{ "", Guid.NewGuid(), Guid.NewGuid() };
            }
        }

        public static IEnumerable<object[]> GuidsSub
        {
            get
            {
                yield return new object[]{ "An error has occurred", Guid.Empty, Guid.Empty };
                yield return new object[]{ "An error has occurred", Guid.NewGuid(), Guid.Empty };
                yield return new object[]{ "An error has occurred", Guid.Empty, Guid.NewGuid() };
                yield return new object[]{ "Already subscribed", Guid.NewGuid(), Guid.NewGuid() };
            }
        }

        [Theory, MemberData(nameof(GuidsNotSub))]
        public async Task UnSubscribe_GuidList_MatchesExpected(string expected, Guid bookID, Guid userID)
        {
            BuildRepo();

            var response = await _bookService.UnSubscribe(bookID, userID);

            Assert.Equal(expected, response);
        }

        [Theory, MemberData(nameof(GuidsSub))]
        public async Task Subscribe_GuidList_MatchesExpected(string expected, Guid bookID, Guid userID)
        {
            BuildRepo();

            var response = await _bookService.Subscribe(bookID, userID);

            Assert.Equal(expected, response);
        }
    }
}
