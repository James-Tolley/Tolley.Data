using System.Data;
using Moq;
using NUnit.Framework;

namespace Tolley.Data.Tests
{
    [TestFixture]
    public class DataContextTests
    {
        private static IDbTransaction GetMockTransaction()
        {
            IDbTransaction transaction = new Mock<IDbTransaction>().Object;
            return transaction;
        }

        private static IDbConnection GetMockConnection()
        {
            var connection = new Mock<IDbConnection>();
            connection.Setup(x => x.BeginTransaction()).Returns(GetMockTransaction);

            return connection.Object;
        }

        private static IConnectionFactory GetMockConnectionFactory()
        {
            var factory = new Mock<IConnectionFactory>();
            factory.Setup(f => f.GetConnection()).Returns(GetMockConnection);

            return factory.Object;
        }

        [Test]
        public void Dispose_CleansUpTransaction()
        {
            var transaction = new Mock<IDbTransaction>();
            var connection = new Mock<IDbConnection>();
            connection.Setup(x => x.BeginTransaction()).Returns(transaction.Object);
            var factory = new Mock<IConnectionFactory>();
            factory.Setup(f => f.GetConnection()).Returns(connection.Object);

            IUnitOfWork uow;
            using (var context = new DataContext(factory.Object))
            {
                uow = context.BeginScope();
            }

            Assert.IsNotNull(uow);
            transaction.Verify(x => x.Rollback(), Times.Once);
            transaction.Verify(x => x.Dispose(), Times.Once);
        }

        [Test]
        public void Dispose_ClosesConnection()
        {
            var transaction = new Mock<IDbTransaction>();
            var connection = new Mock<IDbConnection>();
            connection.Setup(x => x.BeginTransaction()).Returns(transaction.Object);
            var factory = new Mock<IConnectionFactory>();
            factory.Setup(f => f.GetConnection()).Returns(connection.Object);

            using (new DataContext(factory.Object))
            {
            }

            connection.Verify(x => x.Close(), Times.Once);
        }

        [Test]
        public void BeginScope_ReturnsAUnitOfWork()
        {
            var context = new DataContext(GetMockConnectionFactory());

            using (IUnitOfWork uow = context.BeginScope())
            {
                Assert.IsNotNull(uow);
            }
        }

        [Test]
        public void FinalSaveChanges_CommitsTransaction()
        {
            var transaction = new Mock<IDbTransaction>();
            var connection = new Mock<IDbConnection>();
            connection.Setup(x => x.BeginTransaction()).Returns(transaction.Object);
            var factory = new Mock<IConnectionFactory>();
            factory.Setup(f => f.GetConnection()).Returns(connection.Object);
            var context = new DataContext(factory.Object);

            using (IUnitOfWork uow = context.BeginScope())
            {
                using (IUnitOfWork uow2 = context.BeginScope())
                {
                    uow2.SaveChanges();
                }

                uow.SaveChanges();
            }

            transaction.Verify(x => x.Commit(), Times.Once);
        }

        [Test]
        public void InnerSaveChanges_DoesNotCommitTransaction()
        {
            var transaction = new Mock<IDbTransaction>();
            var connection = new Mock<IDbConnection>();
            connection.Setup(x => x.BeginTransaction()).Returns(transaction.Object);
            var factory = new Mock<IConnectionFactory>();
            factory.Setup(f => f.GetConnection()).Returns(connection.Object);
            var context = new DataContext(factory.Object);

            using (context.BeginScope())
            {
                using (IUnitOfWork uow2 = context.BeginScope())
                {
                    uow2.SaveChanges();
                    transaction.Verify(x => x.Commit(), Times.Never);
                }
            }
        }

        [Test]
        public void NestedUnitOfWork_UsesSameTransaction()
        {
            var context = new DataContext(GetMockConnectionFactory());

            using (context.BeginScope())
            {
                IDbTransaction trn = context.Transaction;
                using (context.BeginScope())
                {
                    Assert.AreSame(trn, context.Transaction);
                }
            }
        }

        [Test]
        public void UnitOfWork_IfCommitted_DoesNotRollBackOnDispose()
        {
            var transaction = new Mock<IDbTransaction>();
            var connection = new Mock<IDbConnection>();
            connection.Setup(x => x.BeginTransaction()).Returns(transaction.Object);
            var factory = new Mock<IConnectionFactory>();
            factory.Setup(f => f.GetConnection()).Returns(connection.Object);
            var context = new DataContext(factory.Object);

            using (context.BeginScope())
            {
                using (IUnitOfWork uow2 = context.BeginScope())
                {
                    uow2.SaveChanges();
                }

                transaction.Verify(x => x.Rollback(), Times.Never);
            }
        }

        [Test]
        public void UnitOfWork_IfInnerRolledBack_RollsBackAll()
        {
            var transaction = new Mock<IDbTransaction>();
            var connection = new Mock<IDbConnection>();
            connection.Setup(x => x.BeginTransaction()).Returns(transaction.Object);
            var factory = new Mock<IConnectionFactory>();
            factory.Setup(f => f.GetConnection()).Returns(connection.Object);
            var context = new DataContext(factory.Object);

            using (IUnitOfWork uow = context.BeginScope())
            {
                using (context.BeginScope())
                {
                }

                uow.SaveChanges();
            }

            transaction.Verify(x => x.Commit(), Times.Never);
        }

        [Test]
        public void UnitOfWork_IfNotSaved_RollsBack()
        {
            var transaction = new Mock<IDbTransaction>();
            var connection = new Mock<IDbConnection>();
            connection.Setup(x => x.BeginTransaction()).Returns(transaction.Object);
            var factory = new Mock<IConnectionFactory>();
            factory.Setup(f => f.GetConnection()).Returns(connection.Object);
            var context = new DataContext(factory.Object);

            using (context.BeginScope())
            {
            }

            transaction.Verify(x => x.Rollback(), Times.Once);
        }

        [Test]
        public void UnitOfWork_IfSaved_CommitsTransaction()
        {
            var transaction = new Mock<IDbTransaction>();
            var connection = new Mock<IDbConnection>();
            connection.Setup(x => x.BeginTransaction()).Returns(transaction.Object);
            var factory = new Mock<IConnectionFactory>();
            factory.Setup(f => f.GetConnection()).Returns(connection.Object);
            var context = new DataContext(factory.Object);

            using (IUnitOfWork uow = context.BeginScope())
            {
                uow.SaveChanges();
            }

            transaction.Verify(x => x.Commit(), Times.Once);
        }
    }
}