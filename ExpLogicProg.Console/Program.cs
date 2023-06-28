// See https://aka.ms/new-console-template for more information

using Guan;
using Guan.Logic;

Console.WriteLine("Hello, World!");

// External predicate types (as object instances or singletons (static single instances)) must be specified in a Guan FunctorTable object
// which is used to create the Module, which is a collection of predicate types and is required by the Query executor (see RunQueryAsync impl in GuanQueryDispatcher.cs).
var functorTable = new FunctorTable();
functorTable.Add(TestAddPredicateType.Singleton("addresult"));
functorTable.Add(TestDivPredicateType.Singleton("divresult"));
functorTable.Add(TestSubPredicateType.Singleton("subresult"));
functorTable.Add(TestAttributePredicateType.Singleton("testattr"));

// predicate name is hard-coded in the predicate's impl ("utcnow"). See GetDateTimeUtcNowPredicateType.cs.
functorTable.Add(GetDateTimeUtcNowPredicateType.Singleton());

// Create a List<string> containing logic rules (these could also be housed in a text file). These rules are very simple by design, as are the external predicates used in this sample console app.
// The rules below are simple examples of using logic in their sub rule parts and calling an external predicate.
var logicsRules = new List<string>
{
    "test(?x, ?y) :- ?x == ?y, addresult(?x, ?y)",
    "test(?x, ?y) :- ?y > 0 && ?y < ?x, divresult(?x, ?y)",
    "test(?x, ?y) :- ?x > ?y, subresult(?x, ?y)",
    "test1(1)",
    "test1(2)",
    "test2(2)",
    "test2(3)",
    "test3(q(1, ?x))",
    "test4(?x, ?y) :- test3(q(?x, ?y)), test1(?y)",
    "test5(?x, ?y) :- ?x > 1 && ?y < 3, test1(?x), test2(?y)",
    "test6(?x) :- test1(?x), not(test2(?x))",
    "test7(?x) :- not(?x < 2), test1(?x)",
    "test8(?x, ?y, ?z) :- showtype(?x, ?y), ?x = 5, showtype(?x, ?z)",
    "test9(?x, ?y, ?z) :- b_setval(v1, 0), test1(?x), getval(v1, ?y), b_setval(v1, 5), getval(v1, ?z)",
    "test10(?x, ?y, ?z) :- b_setval(v1, 0), test1(?x), getval(v1, ?y), setval(v1, 5), getval(v1, ?z)",
    "showtype(?x, 'var') :- var(?x)",
    "showtype(?x, 'nonvar') :- nonvar(?x)",
    "showtype(?x, 'atom') :- atom(?x)",
    "showtype(?x, 'compound') :- compound(?x)",
    "f1(?a, ?b, ?b, ?a)",
    "testAttr(?x, ?y) :- testattr(?x, ?y)",
    
    // TODO: Should there be separate predicates for this?
    "dyophysite(?c) :- chalcedonian(?c) ; nestorian(?c)",
    "miaphysite(?c) :- orientalOrthodox(?c)",
    "monophysite(?c) :- 1 > 1",
    "nestorian(?c) :- churchOfTheEast(?c)",
    "chalcedonian(?c) :- easternOrthodox(?c) ; catholic(?c)",
    "orientalOrthodox(copticOrthodox)",
    "orientalOrthodox(syriacOrthodox)",
    "orientalOrthodox(jacobiteSyrianOrthodox)",
    "orientalOrthodox(armenianOrthodox)",
    "orientalOrthodox(malankaraOrthodox)",
    "orientalOrthodox(ethiopianOrthodox)",
    "orientalOrthodox(eritreanOrthodox)",
    "easternOrthodox(constantinopleOrthodox)",
    "easternOrthodox(alexandrianOrthodox)",
    "easternOrthodox(antiochianOrthodox)",
    "easternOrthodox(jerusalemOrthodox)",
    "easternOrthodox(georgianOrthodox)",
    "easternOrthodox(cyprianOrthodox)",
    "easternOrthodox(bulgarianOrthodox)",
    "easternOrthodox(serbianOrthodox)",
    "easternOrthodox(russianOrthodox)",
    "easternOrthodox(greekOrthodox)",
    "easternOrthodox(polishOrthodox)",
    "easternOrthodox(romanianOrthodox)",
    "easternOrthodox(albanianOrthodox)",
    "easternOrthodox(czechSlovakianOrthodox)",
    "easternOrthodox(americanOrthodox)",
    "easternOrthodox(ukrainianOrthodox)",
    "easternOrthodox(macedonianOrthodox)",
    "churchOfTheEast(assyrianChurchOfTheEast)",
    "churchOfTheEast(indianChurchOfTheEast)",
    "churchOfTheEast(chaldeanCatholic)",
    "catholic(romanCatholic)",
};

Module module = Module.Parse("testx", logicsRules, functorTable);
var queryDispatcher = new GuanQueryDispatcher(module);

// test goal with arithmetic external predicates.
await queryDispatcher.QueryAsync("test(3, 3)").ToListAsync();
await queryDispatcher.QueryAsync("test(0, 0)").ToListAsync();
await queryDispatcher.QueryAsync("test(5, 2)").ToListAsync();
await queryDispatcher.QueryAsync("test(3, 2)").ToListAsync();
await queryDispatcher.QueryAsync("test(4, 2)").ToListAsync();
await queryDispatcher.QueryAsync("test(2, 5)").ToListAsync();
await queryDispatcher.QueryAsync("test(6, 2)").ToListAsync();
await queryDispatcher.QueryAsync("test(8, 2)").ToListAsync();
await queryDispatcher.QueryAsync("test(25, 5)").ToListAsync();
await queryDispatcher.QueryAsync("test(1, 0)").ToListAsync();

// testx goals with internal predicates.
await queryDispatcher.QueryAsync("test4(?x, ?y), test2(?y)").ToListAsync(); // (x=1,y=2)
await queryDispatcher.QueryAsync("test5(?x, ?y), test1(?y)").ToListAsync(); // (x=2, y=2)

await queryDispatcher.QueryAsync("testAttr(1, 2)").ToListAsync();
await queryDispatcher.QueryAsync("testAttr('Howdy!', 'good')").ToListAsync();

bool isMia = await queryDispatcher.BoolQueryAsync("miaphysite(copticOrthodox)");
bool isMono = await queryDispatcher.BoolQueryAsync("monophysite(copticOrthodox)");

while (true)
{
    Console.Write("?- ");
    var expression = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(expression))
        break;

    try
    {
        var results = await queryDispatcher
            .QueryAsync(expression, int.MaxValue)
            .ToListAsync();
    
        Console.WriteLine(string.Join(',', results));
        Console.WriteLine();
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine(ex);
    }
}
