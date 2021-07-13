# Speedy

Speedy is a simple easy to use Entity Framework unit testing framework, sync framework, and all around data framework.

### Setup an interface to describe your database

``` csharp
public interface IContosoDatabase : IDatabase
{
	IRepository<Address, int> Addresses { get; }
	IRepository<Person, int> People { get; }
}
```

### Setup your Entity Framework database

``` csharp
public class ContosoDatabase : EntityFrameworkDatabase, IContosoDatabase
{
	public ContosoDatabase()
	{
		// Default constructor needed for Add-Migration
	}

	public ContosoDatabase(DbContextOptions contextOptions, DatabaseOptions options = null)
		: base(contextOptions, options)
	{
	}

	public IRepository<Address, int> Addresses => GetRepository<Address>();
	public IRepository<Person, int> People => GetRepository<Person>();
}
```

Version 4 for Entity Framework 6

``` csharp
public class ContosoDatabase : EntityFrameworkDatabase, IContosoDatabase
{
	public ContosoDatabase()
		: this("name=DefaultConnection")
	{
		// Default constructor needed for Add-Migration
	}

	public ContosoDatabase(string nameOrConnectionString, DatabaseOptions options = null)
		: base(nameOrConnectionString, options)
	{
	}

	public IRepository<Address, int> Addresses => GetRepository<Address>();
	public IRepository<Person, int> People => GetRepository<Person>();
}
```

### Setup your Memory database

``` csharp
public class ContosoMemoryDatabase : Database, IContosoDatabase
{
	public ContosoMemoryDatabase(string directory = null, DatabaseOptions options = null)
		: base(directory, options)
	{
		Addresses = GetRepository<Address>();
		People = GetRepository<Person>();
	}

	public IRepository<Address, int> Addresses { get; }
	public IRepository<Person, int> People { get; }
}
```

### Now you can write test that will run against EntityFramework and / or the Memory database

Preferable you'll write all your _unit test_ using the memory database. You can then use the Entity Framework database for your _integration tests_.

``` csharp
[TestMethod]
public void AddAddressTest()
{
	var options = new DbContextOptionsBuilder<ContosoDatabase>().UseSqlServer("server=localhost;database=Speedy;integrated security=true;").Options;

	foreach (var database in new IContosoDatabase[] { new ContosoDatabase(options), new ContosoMemoryDatabase() })
	{
		using (database)
		{
			database.Addresses.Add(new Address
				{
					City = "Greenville",
					Line1 = "Main Street",
					Line2 = string.Empty,
					Postal = "29671",
					State = "SC"
				});
			
			database.SaveChanges();
		}
	}
}

```

This is a test for v4 for Entity Framework 6.

``` csharp
[TestMethod]
public void AddAddressTest()
{
	var connectionString = "server=localhost;database=Speedy;integrated security=true;";

	foreach (var database in new IContosoDatabase[] { new ContosoDatabase(connectionString), new ContosoMemoryDatabase() })
	{
		using (database)
		{
			database.Addresses.Add(new Address
				{
					City = "Greenville",
					Line1 = "Main Street",
					Line2 = string.Empty,
					Postal = "29671",
					State = "SC"
				});
			
			database.SaveChanges();
		}
	}
}
```

## todo

- Add ability to mark properties as readonly, to now allow edits like Entity.CreatedOn
- Partial entity updates, how could we allow only some columns to update? How would we track the changes?
- WebApi incoming data, how can we support partial post in an efficient way but still have maintainable readable code.
- Speedy.Extensions.EnumExtensions::SetFlag is throwing a cast exception with a ulong enum
- SyncManager: need to allow some members to be more public

## Versions

- v9 supports Entity Framework Core 5
- v7,8 support Entity Framework Core 3
- v5+ supports Entity Framework Core 2
- v4 supports Entity Framework 6