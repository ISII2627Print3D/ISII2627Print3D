namespace AppForSEII.UT;

public class AppForSEII4SqliteUT : IDisposable
{
    protected readonly DbConnection _connection;
    protected readonly ApplicationDbContext _context;
    protected readonly DbContextOptions<ApplicationDbContext> _contextOptions;

    public AppForSEII4SqliteUT()
    {
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        _contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new ApplicationDbContext(_contextOptions);
        _context.Database.EnsureCreated();
    }

    protected ApplicationDbContext CreateContext()
    {
        return new ApplicationDbContext(_contextOptions);
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Dispose();
    }
}