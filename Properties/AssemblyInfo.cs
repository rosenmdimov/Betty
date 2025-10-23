using NUnit.Framework;

// This attribute instructs NUnit to run tests in parallel
// at the "Features" level (or "TestFixtures", which in Reqnroll is the Feature file).
[assembly: Parallelizable(ParallelScope.All)]

// Sets the maximum number of threads that NUnit should use.
// 0 means NUnit should use as many CPU cores as are available (recommended).
//[assembly: LevelOfParallelism(4)]
/**
 * Using this parameter here, we can set the number of 
 * threads/workers statically by running the tests from 
 * the terminal with the command: dotnet test.
 * 
 */
// If you want to limit parallelism to 4 threads:
[assembly: LevelOfParallelism(0)]
