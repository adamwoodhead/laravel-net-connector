## About Laravel NET Connector

Laravel NET Connector is a package with the intent to ultimately simplify the communication between a NET Application and a Laravel REST API.
Whilst this package is primarily designed for use with Laravel, it will also work for any other REST API that has the same naming & route structure.

## Implementation

For all the functionality to work in Laravel NET Connector, you must have certain routes, and handled parameters.

### Structure

Laravel NET Connector follows the general CRUD expectations, and your target API should adopt the following structure.

| Method             | POST           | GET (index)     | GET (single)       | PUT                | DELETE
| :-----------:      | :------------: | :------------:  | :----------------: | :----------------: | :----------------:
| Route              | /api/resource/ | /api/resource/  | /api/resource/{id} | /api/resource/{id} | /api/resource/{id}
| Action             | Create         | Retrieve All    | Retrieve Single    | Update Single      | Delete Single
| HTTP Response      | 201 / 422      | 200             | 200, 404           | 200, 422, 404      | 200, 404
| Body Response Type | JSON           | JSON            | JSON               | JSON               | -
| Body Response      | resource       | [resource, ...] | resource           | resource           | -

### Implementation

#### Controller

Add the following function to your Laravel Applications `\App\Http\Controllers\Controller.php` file, make changes if/where necessary:

```php
class Controller extends BaseController
{
    use AuthorizesRequests, DispatchesJobs, ValidatesRequests;

    const DEFAULT_PER_PAGE = 5000;

    function base_index(Request $request, $model, Array $with = array()){
        if($request->has('pagination'))
        {
            if($request->has('search') && $request->has('in'))
            {
                $needle = $request->search;
                $haystack = $request->in;

                if(!empty($with)){
                    if($needle == '' || $needle == null){
                        return $model::with($with)->whereNull($haystack)
                        ->paginate(self::DEFAULT_PER_PAGE);
                    } else {
                        return $model::with($with)->where($haystack, 'like', '%' . $needle . '%')
                        ->paginate(self::DEFAULT_PER_PAGE);
                    }
                } else {
                    if($needle == '' || $needle == null){
                        return $model::whereNull($haystack)
                        ->paginate(self::DEFAULT_PER_PAGE);
                    } else {
                        return $model::where($haystack, 'like', '%' . $needle . '%')
                        ->paginate(self::DEFAULT_PER_PAGE);
                    }
                }
            }
            else 
            {
                if(!empty($with)){
                    return $model::with($with)->paginate(self::DEFAULT_PER_PAGE);
                } else {
                    return $model::paginate(self::DEFAULT_PER_PAGE);
                }
            }
        }
        else if($request->has('search') && $request->has('in'))
        {
            $needle = $request->search;
            $haystack = $request->in;

            if(!empty($with)){
                if($needle == '' || $needle == null){
                    return $model::with($with)->whereNull($haystack)->get();
                } else {
                    return $model::with($with)->where($haystack, 'like', '%' . $needle . '%')->get();
                }
            } else {
                if($needle == '' || $needle == null){
                    return $model::whereNull($haystack)->get();
                } else {
                    return $model::where($haystack, 'like', '%' . $needle . '%')->get();
                }
            }
        }

        if(!empty($with)){
            return response()->json($model::with($with)->get(), 200);
        } else {
            return response()->json($model::all(), 200);
        }
    }
}
```

### Resource Controller Changes

The following is an example of how to handle the NET Connector requests, and shows the expected responses.

```php
class CustomerController extends Controller
{
    // GET (index / pagination)
    public function index(Request $request)
    {
        // Call inherited base_index function with our request & target class type.
        // The base_index function figures out what type of indexing the request has asked for,
        // such as indexing all, pagination, searching...
        return $this->base_index($request, Customer::class);
    }
    
    // POST (create)
    public function store(Request $request)
    {
        // Validate request
        $request->validate([
            'name' => 'required|unique:customers,name',
        ]);

        // Take only the data required to create our model.
        $data = $request->only([
            'name',
        ]);

        // Create the Customer model (and save)
        $customer = Customer::create($data);

        // Return the Customer object, with the 201 response code.
        return response()->json($customer, 201);
    }
    
    // GET (single)
    public function show(Request $request)
    {
        // Find the requested record
        $record = Customer::find($request->id);

        // If the record doesn't exist
        if($record == null){
            // Return simple error in body, and 404
            return response()->json(['error' => 'not found'], 404);
        }

        // Return the record
        return response()->json($record, 200);
    }
    
    // PUT (single)
    public function update(Request $request, Customer $customer)
    {
        // Find the requested record
        $record = Customer::find($request->id);

        // If the record doesn't exist
        if($record == null){
            // Return simple error in body, and 404
            return response()->json(['error' => 'not found'], 404);
        }

        // Validate request
        $request->validate([
            'name' => 'required|unique:customers,name,' . $request->id
        ]);

        // Make defined changes to the record
        $record->name = $request->name;

        // Save changes made to the record
        $record->save();

        // Return the changed record in response, with 200
        return response()->json($record, 200);
    }
    
    // DELETE (single)
    public function destroy(Request $request, Customer $customer)
    {
        // Find the requested record 
        $record = Customer::find($request->id);

        // If the record doesn't exist
        if($record == null){
            // Return simple error in body, and 404
            return response()->json(['error' => 'not found'], 404);
        }

        // Delete the record
        $record->delete();

        // Return simple success message, and 200
        return response()->json(['success' => 'success'], 200);
    }

    // Relationship, GET (index all)
    public function depots(Request $request){
        // Find the request record
        $record = Customer::find($request->id);

        // If the record doesn't exist
        if($record == null){
            // Return simple error in body, and 404
            return response()->json(['error' => 'not found'], 404);
        }

        // Retrieve the target relationship data of our requested model
        $data = $record->depots ?? [];

        // Return the relationship data alone, and 200
        return response()->json($data, 200);
    }
}
```

### NET Implementation

Now that your Laravel resource controllers are set to handle requests, you must make some small changes to your NET models to ensure their compatibility.

The following example is designed to match the above CustomerController.

```csharp
public interface ICustomer
{
    string Name { get; set; }
}

[JsonObject]
[Route("/customers")]
public class Customer : LaravelModel<Customer>, ICustomer
{
    [JsonProperty("name")]
    public string Name { get; set; }

    // Relationships

    public async Task<List<CustomerDepot>> GetDepotsAsync(CancellationToken cancellationToken = default)
    {
        return await GetDepotsAsync(this.ID, cancellationToken);
    }

    public static async Task<List<CustomerDepot>> GetDepotsAsync(int? id, CancellationToken cancellationToken = default)
    {
        return await GetRelatedModelListAsync<CustomerDepot>("depots", id, cancellationToken);
    }

    [JsonConstructor]
    public Customer() { }
}
```

The main things to note from this example are ` : LaravelModel<Customer>`, `[Route("/customers")]` & `[JsonProperty("name")]`, these must be correct for Laravel NET Connector to work.

1. Each class must inherit the LaravelModel class, which takes the class type as a parameter.
` : LaravelModel<Customer>`

2. Each class must include the Route Attribute, with the route address of its index.
`[Route("/customers")]`

3. Each field in the class must also be marked with `[JsonProperty("name")]` and its corresponding Laravel `$fillable` name.

### Usage

Using the above implementation, you can expect the following usage results:

##### Static Calls
```csharp
Config config = new Config()
{
    BaseURL = "https://127.0.0.1:8000/api",
    AuthenticationMode = Enums.AuthenticationType.JWT,
    AutoRefresh = true,
    Out = Console.Out,
    ThrowAuthenticationExceptions = true
};

Host.Initialize(config);

await Customer.GetAsync(); // returns List<Customer>
await Customer.GetAsync(1); // returns Customer

await Customer.PaginateAsync() // Returns Page (see Models.Page), Page.Data containing List<Customer>
await Customer.PaginateSearchAsync("name", "Billy") // Returns Page (see Models.Page), Page.Data containing List<Customer>, where name includes "Billy"

await Customer.SearchAsync("name", "Billy"); // returns List<Customer>, where customers names include "Billy"

await Customer.GetDepotsAsync(1); // Returns List<Depot>, from Customer ID 1
await Customer.DeleteAsync(1); // Returns true on success, false on failure
```

##### Reference Calls

```csharp
var customer = new Customer() { Name = "Billy" }; // Create Customer Model in regular NET
await customer.CreateAsync(); // Create Customer through API, returns created Customer
await customer.GetDepotsAsync(); // Returns List<Depot>, from Customer
customer.Name = "Ted";
await customer.UpdateAsync(); // Updates Customer Name to Ted, returns the updated Customer
await customer.DeleteAsync(); // Deletes the Customer, returns true on success, false on failure
```

## Contributing

Thank you for considering contributing. Please feel free to fork your own branch and make pull requests - explain your suggested edits & additions.

## Security Vulnerabilities

If you discover a security vulnerability within Laravel Net Connector, please send an e-mail to Adam Woodhead via [info@adamwoodhead.co.uk](mailto:info@adamwoodhead.co.uk). All security vulnerabilities will be promptly addressed.

## License

This package is open-sourced software licensed under the [MIT license](https://opensource.org/licenses/MIT).
