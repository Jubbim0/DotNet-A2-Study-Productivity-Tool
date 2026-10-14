using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using StudyProductivityApp.Data;

namespace StudyProductivityApp.Tests
{
    public abstract class SqliteTestBase
    {
        private SqliteConnection connection = null!;

        protected Func<AppDbContext> CreateContext { get; private set; } = null!;

        [SetUp]
        public void SetUpDatabase()
        {
            connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(connection)
                .Options;

            CreateContext = () => new AppDbContext(options);

            using AppDbContext db = CreateContext();

            db.Database.EnsureCreated();
        }

        [TearDown]
        public void TearDownDatabase()
        {
            connection?.Dispose();
        }
}

