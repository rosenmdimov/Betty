using NUnit.Framework;

// Run features (fixtures) and scenarios (tests) in parallel.
// Every scenario gets its own browser in DIHooks, so scenarios don't share state.
[assembly: Parallelizable(ParallelScope.All)]

// The number of parallel workers is left to NUnit (defaults to the number of CPU cores).
// To set it for one run:  dotnet test -- NUnit.NumberOfTestWorkers=3
// To fix it permanently, uncomment:
//[assembly: LevelOfParallelism(4)]
